using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using eContracting.Models;
using Glass.Mapper.Sc;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using System.Drawing.Imaging;
using System.Web;
using Sitecore.DependencyInjection;
using Sitecore.Data.Serialization.Exceptions;

namespace eContracting.Services
{
    /// <inheritdoc/>
    /// <seealso cref="IFileOptimizer" />
    public class FileOptimizer : IFileOptimizer
    {
        protected readonly ILogger Logger;
        protected readonly ISitecoreContext Context;
        protected readonly IApiService ApiService;
        protected readonly IAuthenticationService AuthService;
        protected readonly ISettingsReaderService SettingsReaderService;

        private readonly long GroupResultingFileSizeLimit;
        private readonly Size MaxImageSizeAfterResize;
        private readonly Size MinImageSizeNoResize;

        private const string sourceFileNameComponentsSeparator = "___";
        private const string jointFileNamePrefix = "___jointFile___";
        private const string resizedFileNamePrefix = "___resizedFile___";

        public string FileStorageRoot { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileOptimizer"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="context">The context.</param>
        /// <param name="apiService">The API service.</param>
        /// <param name="authService">The authentication service.</param>
        /// <param name="settingsReaderService">The settings reader service.</param>
        /// <exception cref="ArgumentNullException">
        /// logger
        /// or
        /// context
        /// or
        /// apiService
        /// or
        /// authService
        /// or
        /// settingsReaderService
        /// </exception>
        public FileOptimizer(
            ILogger logger,
            ISitecoreContext context,
            IApiService apiService,
            IAuthenticationService authService,
            ISettingsReaderService settingsReaderService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));

            // TODO: sitecore config
            this.GroupResultingFileSizeLimit = 25 * 1024 * 1024;
            this.MaxImageSizeAfterResize = new Size(8096, 8096);
            this.MinImageSizeNoResize = new Size(768, 768);
        }

        /// <inheritdoc/>
        public async Task<DbUploadGroupFileModel> AddAsync(DbUploadGroupFileModel group, string groupKey, string fileId, string name, byte[] content, string sessionId, string guid)
        {
            var originalFiles = new List<DbFileModel>();
            //DbFileModel outputFile = null;
            PdfDocument outputPdfDocument = null;

            if (group != null)
            {
                if (group.OriginalFiles?.Length > 0)
                {
                    originalFiles.AddRange(group.OriginalFiles);
                }
                //outputFile = group.OutputFile;
                if (group.OutputFile != null && group.OutputFile.Content!=null && group.OutputFile.Content.Length > 0)
                {
                    using (Stream existingPdfStream = new MemoryStream(group.OutputFile.Content))
                    {
                        outputPdfDocument = PdfReader.Open(existingPdfStream, PdfDocumentOpenMode.Modify);
                    }                    
                }
            }
            else
            {
                group = new DbUploadGroupFileModel();
                group.Key = groupKey;
                group.SessionId = sessionId;
                group.Guid = guid;
                group.OutputFile = new DbFileModel() { Key = groupKey, FileName = groupKey, FileExtension = "pdf", MimeType = "application/pdf", Size = 0 }; // attributes?   
            }

            // vytvor vystupni pdf dokument, pokud neexistuje
            if (outputPdfDocument == null)
            {
                outputPdfDocument = this.CreatePdfDocument(groupKey);
            }


            byte[] fileByteContent = null;

            // otoc obrazek podle EXIF orientace
            if (this.IsImage(name))
            {
                using (var memoryStream = new MemoryStream(content))
                {
                    using (Image image = Image.FromStream(memoryStream))
                    {
                        this.NormalizeOrientation(image);

                        using (var imageStream = new MemoryStream())
                        {
                            image.Save(imageStream, image.RawFormat);
                            fileByteContent = imageStream.ToArray();
                        }
                    }
                }
            }
            else
            {
                fileByteContent = content;
            }

            // pridej file do db modelu
            DbFileModel fileModel = new DbFileModel()
            {
                Key = fileId,
                FileName = name,
                FileExtension = Path.GetExtension(name),
                MimeType = MimeMapping.GetMimeMapping(name),
                Content = fileByteContent,
            };
            originalFiles.Add(fileModel);


            // zapis file do pdfka v groupe
            AddFileToOutputPdfDocument(outputPdfDocument, fileByteContent, name);

            // zmensi soubory, pokud ses nevesel do limitu vysledneho pdfka
            int compressionRounds = 0;
            while (compressionRounds<5)
            {
                if (outputPdfDocument.FileSize <= this.GroupResultingFileSizeLimit)
                {
                    outputPdfDocument = this.CreatePdfDocument(groupKey);
                    this.CompressFiles(group, compressionRounds);
                    foreach (var fileInGroup in originalFiles)
                    {
                        AddFileToOutputPdfDocument(outputPdfDocument, fileInGroup.Content, fileInGroup.FileName);
                    }
                    compressionRounds++;
                }
                else 
                    break;
            }

            // ulozim vyslednou podobu OriginalFiles (pridany novy soubor a pripadne zmensene ty predchazejici)
            this.SaveOutputPdfDocumentToGroup(group, outputPdfDocument);
            group.OriginalFiles = originalFiles.ToArray();

            // tady to musíme zase uložit
            // ne, ukladame to prece v controlleru
            //await this.UserFileCache.SetAsync(dbGroup);
            return group;
        }

        private void SaveOutputPdfDocumentToGroup(DbUploadGroupFileModel group, PdfDocument outputPdfDocument)
        {
            using (MemoryStream outputFileWriteMemoryStream = new MemoryStream())
            {
                outputPdfDocument.Save(outputFileWriteMemoryStream);
                group.OutputFile.Content = outputFileWriteMemoryStream.ToArray();                
            }
        }

        /// <inheritdoc/>
        public async Task<DbUploadGroupFileModel> RemoveFileAsync(DbUploadGroupFileModel group, string fileId)
        {
            if (group != null)
            {
                if (group.OriginalFiles?.Length > 0)
                {
                    var originalFiles = new List<DbFileModel>();
                    originalFiles.AddRange(group.OriginalFiles);

                    var fileToRemove = originalFiles.FirstOrDefault(f => f.Key == fileId);
                    if (fileToRemove != null)
                    {
                        originalFiles.Remove(fileToRemove);
                        group.OriginalFiles = originalFiles.ToArray();

                        // nove vygeneruju vysledne pdfko, protoze konkretni soubor nevim jak z nej vymazat
                        PdfDocument outputPdfDocument = this.CreatePdfDocument(group.Key);
                        foreach (var fileInGroup in originalFiles)
                        {
                            AddFileToOutputPdfDocument(outputPdfDocument, fileInGroup.Content, fileInGroup.FileName);
                        }
                        this.SaveOutputPdfDocumentToGroup(group, outputPdfDocument);
                        
                    }
                // TODO: mam neco vyhazovat, kdyz je tu neco divnyho? file neexistuje, kolekce prazdna,.. ?
                }
            }

            return group;
        }

        private void AddFileToOutputPdfDocument(PdfDocument outputPdfDocument, byte[] fileByteContent, string name)
        {
            if (this.IsPdf(name))
            {
                this.AppendPdfToPdf(fileByteContent, outputPdfDocument);
            }
            else if (this.IsImage(name))
            {
                PdfPage pdfPageForImage = new PdfPage(outputPdfDocument);
                this.AppendImageToPdf(pdfPageForImage, fileByteContent, 0, 0, 1, name);
            }
        }



        private PdfDocument CreatePdfDocument(string groupKey)
        {
            PdfDocument document = new PdfDocument(groupKey);
            return document;
        }

        private void CompressFiles(DbUploadGroupFileModel group, int compressionRoundsElapsed)
        {
            long compressableFilesSize = group.OriginalFiles.Where(f => this.IsCompressable(f.FileExtension)).Sum(f => f.Size);
            long uncompressableFilesSize = group.OriginalFiles.Where(f => !this.IsCompressable(f.FileExtension)).Sum(f => f.Size);
            long totalLimitForCompressableFiles = this.GroupResultingFileSizeLimit - uncompressableFilesSize;

            float needToCompressTimes = 1;

            needToCompressTimes = compressableFilesSize / totalLimitForCompressableFiles;

            foreach (var originalFile in group.OriginalFiles.Where(f => this.IsCompressable(f.FileExtension)))
            {
                if (this.IsCompressable(originalFile.FileExtension) && this.IsImage(originalFile.FileExtension))
                {
                    using (var memoryStream = new MemoryStream(originalFile.Content))
                    {
                        using (Image imageOriginal = Image.FromStream(memoryStream))
                        {
                            // TODO: lepsi odhad potrebneho pomeru komprese datova velikost vs rozmer
                            float imageReductionByNeeded = (needToCompressTimes - (float)0.5 + compressionRoundsElapsed);
                            // sichr je sichr, hlavne nezvetsovat
                            if (imageReductionByNeeded < 1)
                                imageReductionByNeeded = 1;

                            using (Image resizedImage = this.ResizeImage(imageOriginal, this.MinImageSizeNoResize.Width, this.MinImageSizeNoResize.Height, 1 / (imageReductionByNeeded), this.MaxImageSizeAfterResize.Width, this.MaxImageSizeAfterResize.Height))
                            {
                                using (MemoryStream resizedImageMemoryStream = new MemoryStream())
                                {
                                    resizedImage.Save(resizedImageMemoryStream, resizedImage.RawFormat);
                                    originalFile.Content = resizedImageMemoryStream.ToArray();
                                }
                            }
                        }
                    }
                }
            }
        }


        private List<FileInfo> GetOriginalFilesInGroup(string groupKey)
        {
            // TODO: predelat na pouziti session misto file storage
            //var cache = ServiceLocator.ServiceProvider.GetRequiredService<ICache>();
            //cache.AddToSession(Constants.CacheKeys.OFFER_IDENTIFIER, data);


            List<FileInfo> result = new List<FileInfo>();
            var originalFiles = Directory.GetFiles(this.FileStorageRoot, $"{groupKey}{sourceFileNameComponentsSeparator}*");
            if (originalFiles != null && originalFiles.Any())
            {
                result = originalFiles.Select(of => new FileInfo(of)).ToList();
            }
            return result;
        }

        private bool IsCompressable(string filename)
        {
            return this.IsImage(filename);
        }
        private bool IsPdf(string filename)
        {
            return filename?.ToLower().EndsWith("pdf")?? false;
        }

        private bool IsImage(string filename)
        {
            return !IsPdf(filename);
        }




        /// <summary>
        /// Otoci obrazek podle Exif tagu, pokud tam je
        /// </summary>
        /// <param name="image"></param>
        private void NormalizeOrientation(Image image)
        {
            const int ExifOrientationTagId = 274;

            if (Array.IndexOf(image.PropertyIdList, ExifOrientationTagId) > -1)
            {
                int orientation;

                orientation = image.GetPropertyItem(ExifOrientationTagId).Value[0];

                if (orientation >= 1 && orientation <= 8)
                {
                    switch (orientation)
                    {
                        case 2:
                            image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case 3:
                            image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 4:
                            image.RotateFlip(RotateFlipType.Rotate180FlipX);
                            break;
                        case 5:
                            image.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        case 6:
                            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 7:
                            image.RotateFlip(RotateFlipType.Rotate270FlipX);
                            break;
                        case 8:
                            image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                    }

                    image.RemovePropertyItem(ExifOrientationTagId);
                }
            }
        }

        /// <summary>
        /// Zmensi bitmap obrazek
        /// </summary>
        /// <param name="image"></param>
        /// <param name="ratio">Desetinne cislo, jak moc se ma obrazek zmensit</param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        private Image ResizeImage(Image image, int minWidth, int minHeight, float? ratio = 1, int? maxWidth = int.MaxValue, int? maxHeight = int.MaxValue)
        {
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // pokud neni potreba zmensovat, nic nekoduj, proste vrat original
            if ((ratio == null || ratio == 1) && maxWidth.Value >= originalWidth && maxHeight >= originalHeight)
                return image;

            // To preserve the aspect ratio
            float ratioX = (float)(Math.Min(maxWidth.Value, originalWidth)) / (float)originalWidth;
            float ratioY = (float)(Math.Min(maxHeight.Value, originalHeight)) / (float)originalHeight;
            float resultingRatio = Math.Min(ratio ?? 1, Math.Min(ratioX, ratioY));

            // New width and height based on aspect ratio            
            int newWidth = (int)(originalWidth * resultingRatio);
            int newHeight = (int)(originalHeight * resultingRatio);

            // TODO: pokud bych zmensoval pod minima
            if (newWidth < minWidth || newHeight < minHeight)
                return image;

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            MemoryStream mStream = new MemoryStream();
            newImage.Save(mStream, ImageFormat.Jpeg);
            Image resizedImageFromMemoryStream = Image.FromStream(mStream);
            newImage.Dispose();

            return resizedImageFromMemoryStream;
        }

        /// <summary>
        /// Zmensi bitmap obrazek
        /// </summary>
        /// <param name="mg"></param>
        /// <param name="newSize"></param>        
        private Bitmap ResizeImage(Image mg, Size newSize)
        {
            double ratio;

            if (mg.Width / Convert.ToDouble(newSize.Width) > mg.Height / Convert.ToDouble(newSize.Height))
            {
                ratio = Convert.ToDouble(mg.Width) / Convert.ToDouble(newSize.Width);
            }
            else
            {
                ratio = Convert.ToDouble(mg.Height) / Convert.ToDouble(newSize.Height);
            }

            var myThumbHeight = Math.Ceiling(mg.Height / ratio);
            var myThumbWidth = Math.Ceiling(mg.Width / ratio);

            var thumbSize = new Size((int)myThumbWidth, (int)myThumbHeight);
            var bp = new Bitmap(newSize.Width, newSize.Height);
            var x = (newSize.Width - thumbSize.Width) / 2;
            var y = newSize.Height - thumbSize.Height;

            var g = Graphics.FromImage(bp);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            var rect = new Rectangle(x, y, thumbSize.Width, thumbSize.Height);
            g.DrawImage(mg, rect, 0, 0, mg.Width, mg.Height, GraphicsUnit.Pixel);

            //// ulozeni img z DB na disk TODO jen pro moje testovani, pak smazat
            //bp.Save(@"c:\Users\lnovak\Downloads\!!!!\newBitmapResize.jpg");
            //// ulozeni img z DB na disk TODO jen pro moje testovani, pak smazat

            return bp;
        }

        /// <summary>
        /// Vlozi pdf do pdf dokumentu
        /// </summary>
        /// <param name="pdf"></param>
        /// <param name="outputDocument"></param>
        private void AppendPdfToPdf(byte[] pdf, PdfDocument outputDocument)
        {
            using (var msPdf = new MemoryStream(pdf))
            {
                var inputDocument = PdfReader.Open(msPdf, PdfDocumentOpenMode.Import);
                // pridam stranku po strance
                var count = inputDocument.PageCount;
                for (var idx = 0; idx < count; idx++)
                {
                    var page = inputDocument.Pages[idx];
                    outputDocument.AddPage(page);
                }
            }
        }

        /// <summary>
        /// Vlozi obrazek do pdf dokumentu
        /// </summary>
        /// <param name="pdf"></param>
        /// <param name="img"></param>
        /// <param name="xPosition"></param>
        /// <param name="yPosition"></param>
        /// <param name="scale"></param>
        /// <param name="footerText"></param>
        private void AppendImageToPdf(PdfPage pdf, byte[] img, int xPosition, int yPosition, double scale, string footerText)
        {
            var gfx = XGraphics.FromPdfPage(pdf);
            var bitmapImage = this.CreateBitmap(img);
            float verticalResolution;
            float horizontalResolution;

            using (var msImg = new MemoryStream(img))
            {
                var image = Image.FromStream(msImg);
                verticalResolution = image.VerticalResolution;
                horizontalResolution = image.HorizontalResolution;
            }

            // pred pripojenim obrazku do pdf ho preulozim, protoze obrazky s velkym dpi se do pdf vlozi spatne
            if (verticalResolution > 96 || horizontalResolution > 96
                && bitmapImage != null)
            {
                // zmensim preulozeny obrazek
                var resizeBitmap = this.ResizeImage(bitmapImage, new Size((int)(0.75f * bitmapImage.Width), (int)(0.75f * bitmapImage.Height)));

                // vlozim do pdf preulozeny obrazek
                var ximg = XImage.FromGdiPlusImage(resizeBitmap);

                // prepocitam velikosti obrazku
                var width = (XUnit)ximg.PixelWidth;
                var height = (XUnit)ximg.PixelHeight;

                if (width > pdf.Width)
                {
                    var ratioX = pdf.Width / ximg.PixelWidth;
                    var ratioY = pdf.Height / ximg.PixelHeight;
                    var ratio = Math.Min(ratioX, ratioY);

                    width = width * ratio;
                    height = height * ratio;
                }

                gfx.DrawImage(ximg, xPosition, yPosition, width * scale, height * scale);
            }
            else
            {
                using (var msImg = new MemoryStream(img))
                {
                    var image = Image.FromStream(msImg);
                    var ximg = XImage.FromGdiPlusImage(image);

                    // prepocitam velikosti obrazku
                    var width = (XUnit)ximg.PixelWidth;
                    var height = (XUnit)ximg.PixelHeight;

                    if (width > pdf.Width)
                    {
                        var ratioX = pdf.Width / ximg.PixelWidth;
                        var ratioY = pdf.Height / ximg.PixelHeight;
                        var ratio = Math.Min(ratioX, ratioY);

                        width = width * ratio;
                        height = height * ratio;
                    }

                    gfx.DrawImage(ximg, xPosition, yPosition, width * scale, height * scale);
                }
            }

            if (!string.IsNullOrEmpty(footerText))
            {
                XFont font = new XFont("Verdana", 8, XFontStyle.Bold);
                XStringFormat pageFooterFormat = new XStringFormat();
                pageFooterFormat.Alignment = XStringAlignment.Center;
                pageFooterFormat.LineAlignment = XLineAlignment.Far;
                XRect box = new XRect(0, 0, pdf.Width / 2, pdf.Height);
                box.Inflate(0, -10);
                gfx.DrawString(String.Format("- {0} -", footerText), font, XBrushes.Black, box, pageFooterFormat);
            }
        }

        /// <summary>
        /// Vytvori bitmap obrazek z bytoveho pole
        /// </summary>
        /// <param name="imgBytes"></param>
        private Image CreateBitmap(byte[] imgBytes)
        {
            var ic = new ImageConverter();
            var newImg = (Image)ic.ConvertFrom(imgBytes);
            if (newImg == null)
            {
                return null;
            }

            var newBitmap = new Bitmap(newImg);
            newBitmap.SetResolution(96.0F, 96.0F);
            //// ulozeni img z DB na disk TODO jen pro moje testovani, pak smazat
            //newBitmap.Save(@"c:\Users\lnovak\Downloads\!!!!\newBitmap.jpg");
            //// ulozeni img z DB na disk TODO jen pro moje testovani, pak smazat

            return newBitmap;
        }





    }
}
