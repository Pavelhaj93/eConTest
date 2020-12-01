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

        private const string sourceFileNameComponentsSeparator = "___";
        private const string jointFileNamePrefix = "___jointFile___";
        private const string resizedFileNamePrefix = "___resizedFile___";

        public string FileStorageRoot { get; set; }


        public FileOptimizer(
            ILogger logger,
            ISitecoreContext context,
            IApiService apiService,
            IAuthenticationService authService,
            ISettingsReaderService settingsReaderService
            )
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));


            // TODO: sitecore config
            this.GroupResultingFileSizeLimit = 25 * 1024 * 1024;
            this.MaxImageSizeAfterResize = new Size(8096, 8096);
        }




        public Task<OptimizedFileGroupModel> AddAsync(string groupKey, string fileId, string name, byte[] content)
        {
            string fileNameWithGroupIdentifier = $"{groupKey}{sourceFileNameComponentsSeparator}{fileId}{sourceFileNameComponentsSeparator}{name}";
            string fileNameAndPath = this.FileStorageRoot + fileNameWithGroupIdentifier;

            File.WriteAllBytes(fileNameAndPath, content);

            if (this.IsImage(new FileInfo(fileNameAndPath)))
            {
                using (Image image = Image.FromFile(fileNameAndPath))
                {
                    this.NormalizeOrientation(image);
                    image.Save(fileNameAndPath);
                }
            }

            var originalFilesInGroup = this.GetOriginalFilesInGroup(groupKey);
            List<FileInfo> filesToJoinIntoPdf = new List<FileInfo>();

            long totalSizeWithoutCompression = this.GetOriginalFilesInGroup(groupKey).Sum(f => f.Length);                                             
            if (totalSizeWithoutCompression <= GroupResultingFileSizeLimit)
            {
                filesToJoinIntoPdf.AddRange(originalFilesInGroup);
            }
            else
            {
                filesToJoinIntoPdf.AddRange(CompressFiles(originalFilesInGroup));
            }
            
            JoinFilesToPdf(filesToJoinIntoPdf, GetJointPdfFileName(groupKey));

            // TODO: aby se nedelal novy dotaz do filesystemu, ale naplnily se jen ty soubory, ktere se prave zpracovaly
            return this.GetInternalAsync(groupKey);
        }

        public Task<OptimizedFileGroupModel> GetAsync(string groupKey)
        {
            return this.GetInternalAsync(groupKey);
        }


        public Task<bool> RemoveAsync(string groupKey, string fileId)
        {
            throw new NotImplementedException();
        }

        private Task<OptimizedFileGroupModel> GetInternalAsync(string groupKey)
        {            
            List<FileInOptimizedGroupModel> filesinGroupModels = new List<FileInOptimizedGroupModel>();
            var originalFilesInGroup = this.GetOriginalFilesInGroup(groupKey);
            foreach (var file in originalFilesInGroup)
            {
                FileInOptimizedGroupModel fileModel = new FileInOptimizedGroupModel(
                                                                    GetGroupfileOriginalName(file),
                                                                    GetGroupfileKey(file),
                                                                     MimeMapping.GetMimeMapping(file.Name),
                                                                     file.Length);
                filesinGroupModels.Add(fileModel);
            }
            string jointPdfFile = this.GetJointPdfFileName(groupKey);

            OptimizedFileGroupModel optimizedFileGroupModel = new OptimizedFileGroupModel(groupKey, filesinGroupModels, File.ReadAllBytes(jointPdfFile));
            return Task.FromResult<OptimizedFileGroupModel>(optimizedFileGroupModel);
        }

        private string GetGroupfileOriginalName(FileInfo file)
        {
            string fileOriginalName = string.Empty;
            try
            {
                fileOriginalName = file.Name.Substring(file.Name.LastIndexOf(sourceFileNameComponentsSeparator) + sourceFileNameComponentsSeparator.Length);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Warn("GetGroupfileOriginalName: Chyba pri zjisteni puvodniho jmena souboru: ", ex, this);
            }
            return fileOriginalName;
        }
        private string GetGroupfileKey(FileInfo file)
        {
            string fileOriginalName = string.Empty;
            try
            {
                fileOriginalName = file.Name.Substring(file.Name.IndexOf(sourceFileNameComponentsSeparator) + sourceFileNameComponentsSeparator.Length,
                                                      file.Name.Length-file.Name.LastIndexOf(sourceFileNameComponentsSeparator));
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Warn("GetGroupfileKey: Chyba pri zjisteni identifikatoru z nazvu souboru: ", ex, this);
            }
            return fileOriginalName;
        }
        private string GetJointPdfFileName(string groupKey)
        {
            return this.FileStorageRoot + jointFileNamePrefix + groupKey + ".pdf";
        }


        private void JoinFilesToPdf(List<FileInfo> files, string resultingPdfFileNameWithPath)
        {
            PdfDocument outputPDFDocument = new PdfDocument();

            foreach (var file in files)
            {
                if (this.IsPdf(file))
                {
                    AppendPdfToPdf(File.ReadAllBytes(file.FullName), outputPDFDocument);
                }
                if (this.IsImage(file))
                {
                    PdfPage pdfPageForImage = new PdfPage(outputPDFDocument);                                     
                    AppendImageToPdf(pdfPageForImage, File.ReadAllBytes(file.FullName),0,0,1, this.GetGroupfileOriginalName(file));
                }
            }

            if (File.Exists(resultingPdfFileNameWithPath))
            {
                File.Delete(resultingPdfFileNameWithPath);
            }
            outputPDFDocument.Save(resultingPdfFileNameWithPath);

        }

        private List<FileInfo> CompressFiles(List<FileInfo> originalFilesInGroup)
        {
            List<FileInfo> result = new List<FileInfo>();
            long compressableFilesSize = originalFilesInGroup.Where(f => this.IsCompressable(f)).Sum(f => f.Length);
            long uncompressableFilesSize = originalFilesInGroup.Where(f => !this.IsCompressable(f)).Sum(f => f.Length);
            long totalLimitForCompressableFiles = this.GroupResultingFileSizeLimit - uncompressableFilesSize;

            float needToCompressTimes = 1;
            int compressionRoundsElapsed = 0;

            needToCompressTimes = compressableFilesSize / totalLimitForCompressableFiles;

            // 5 je pojistka proti nekonecne smyccce, kdyby to tu nekdo zprasil
            while (compressionRoundsElapsed < 5)
            {
                foreach (var originalFile in originalFilesInGroup)
                {
                    if (this.IsCompressable(originalFile) && this.IsImage(originalFile))
                    {
                        using (Image imageOriginal = Image.FromFile(originalFile.FullName))
                        {
                            // TODO: lepsi odhad potrebneho pomeru komprese datova velikost vs rozmer
                            float imageReductionByNeeded = (needToCompressTimes - (float)0.5 + compressionRoundsElapsed);
                            // sichr je sichr, hlavne nezvetsovat
                            if (imageReductionByNeeded < 1)
                                imageReductionByNeeded = 1; 

                            using (Image resizedImage = this.ResizeImage(imageOriginal, 1 / (imageReductionByNeeded), this.MaxImageSizeAfterResize.Width, this.MaxImageSizeAfterResize.Height))
                            {
                                string resizedImageFileNameWoPath = resizedFileNamePrefix + originalFile.Name;
                                string resizedImageFileName = originalFile.DirectoryName + resizedImageFileNameWoPath;
                                resizedImage.Save(resizedImageFileName, ImageFormat.Jpeg);
                                result.Add(new FileInfo(resizedImageFileName));
                            }
                        }
                    }
                    else
                    {
                        result.Add(originalFile);
                    }

                    compressionRoundsElapsed++;
                }
            }

            return result;
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

        private bool IsCompressable(FileInfo fi)
        {
            return this.IsImage(fi);
        }
        private bool IsPdf(FileInfo fi)
        {
            return fi.Extension.Trim().ToLower().Equals("pdf");
        }

        private bool IsImage(FileInfo fi)
        {
            return !fi.Extension.Trim().ToLower().Equals("pdf");            
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
        private Image ResizeImage(Image image, float? ratio = 1, int? maxWidth = int.MaxValue, int? maxHeight = int.MaxValue)
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
            var bitmapImage = CreateBitmap(img);
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
                var resizeBitmap = ResizeImage(bitmapImage, new Size((int)(0.75f * bitmapImage.Width), (int)(0.75f * bitmapImage.Height)));

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
