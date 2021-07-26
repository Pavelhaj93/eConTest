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
        protected readonly IOfferService ApiService;
        protected readonly IAuthenticationService AuthService;
        protected readonly ISettingsReaderService SettingsReaderService;

        private ISiteSettingsModel _siteSettings { get; set; }

        protected ISiteSettingsModel SiteSettings
        {
            get
            {
                if (this._siteSettings == null)
                {
                    this._siteSettings = this.SettingsReaderService.GetSiteSettings();
                }

                return this._siteSettings;
            }
        }

        protected long GroupResultingFileSizeLimit
        {
            get
            {
                return this.SiteSettings.GroupResultingFileSizeLimitKBytes * 1024;
            }
        }

        protected long TotalResultingFilesSizeLimit
        {
            get
            {
                return this.SiteSettings.TotalResultingFilesSizeLimitKBytes * 1024;
            }
        }
        protected long GroupFileCountLimit
        {
            get
            {
                return this.SiteSettings.GroupFileCountLimit;
            }
        }

        protected Size MaxImageSizeAfterResize
        {
            get
            {
                return new Size(this.SiteSettings.MaxImageWidthAfterResize, this.SiteSettings.MaxImageHeightAfterResize);
            }
        }

        protected Size MinImageSizeNoResize
        {
            get
            {
                return new Size(this.SiteSettings.MinImageWidthNoResize, this.SiteSettings.MinImageHeightNoResize);
            }
        }

        protected int MaxGroupOptimizationRounds
        {
            get
            {
                return this.SiteSettings.MaxGroupOptimizationRounds;
            }
        }

        protected int MaxAllGroupsOptimizationRounds
        {
            get
            {
                return this.SiteSettings.MaxAllGroupsOptimizationRounds;
            }
        }

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
            IOfferService apiService,
            IAuthenticationService authService,
            ISettingsReaderService settingsReaderService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
        }

        /// <inheritdoc/>
        public async Task<UploadGroupFileOperationResultModel> AddAsync(DbUploadGroupFileModel group, string groupKey, string fileId, string name, byte[] content, string sessionId, string guid)
        {
            PdfDocument outputPdfDocument = null;

            using (var existingPdfStream = new MemoryStream())
            {
                if (group != null)
                {
                    if (this.GroupFileCountLimit > 0 && group.OriginalFiles?.Count >= this.GroupFileCountLimit)
                    {
                        return new UploadGroupFileOperationResultModel() { IsSuccess = false, ErrorModel = ERROR_CODES.UploadedFilesGroupCountExceeded() };
                    }

                    if (group.OutputFile?.Content.Length > 0)
                    {
                        await existingPdfStream.WriteAsync(group.OutputFile.Content, 0, group.OutputFile.Content.Length);
                        outputPdfDocument = PdfReader.Open(existingPdfStream, PdfDocumentOpenMode.Modify);                        
                    }
                }
                else
                {
                    group = new DbUploadGroupFileModel();
                    group.Key = groupKey;
                    group.SessionId = sessionId;
                    group.Guid = guid;
                    group.OutputFile = new DbFileModel() { Key = groupKey, FileName = groupKey, FileExtension = "pdf", MimeType = "application/pdf" }; // attributes?   
                    group.CreateDate = DateTime.UtcNow;
                }

                return await this.ProcessFilesAsync(group, outputPdfDocument, fileId, name, content, groupKey);
            }
        }

        public async Task<bool> IsOfferTotalFilesSizeInLimitAsync(int actualTotalSize)
        {
            return actualTotalSize <= this.TotalResultingFilesSizeLimit;
        }

        /// <inheritdoc/>
        public async Task<UploadGroupFileOperationResultModel> EnforceOfferTotalFilesSizeAsync(List<DbUploadGroupFileModel> allGroups, DbUploadGroupFileModel groupLastAdded, string fileIdLastAdded)
        {
            UploadGroupFileOperationResultModel result = new UploadGroupFileOperationResultModel() { IsSuccess = true, MadeChanges = false };

            long totalGroupOutputFilesLength = allGroups.Sum(g => g.OutputFile.Content.LongLength);
            if (totalGroupOutputFilesLength > this.TotalResultingFilesSizeLimit)
            {
                if (!this.CompressGroups(allGroups, this.TotalResultingFilesSizeLimit))
                {
                    result.ErrorModel = ERROR_CODES.UploadedFilesTotalSizeExceeded(); // $"Group files size limit { this.GroupResultingFileSizeLimit} exceeeded, actual size would be { group.OutputFile.Content.LongLength } bytes.");
                    result.IsSuccess = false;

                    // nemusi se odstranovat, pokud se v controlleru vrati BadRequest pred tim, nez se zavola SetGroup pro ulozeni do database 
                    //result.DbUploadGroupFileModel = await this.RemoveFileAsync(groupLastAdded, fileIdLastAdded);
                }
                else
                {
                    result.MadeChanges = true;
                    result.DbUploadGroupFileModels = allGroups;
                }
            }

            return result;
        }

        protected internal async Task<UploadGroupFileOperationResultModel> ProcessFilesAsync(DbUploadGroupFileModel group, PdfDocument outputPdfDocument, string fileId, string name, byte[] content, string groupKey)
        {
            UploadGroupFileOperationResultModel result = new UploadGroupFileOperationResultModel() { IsSuccess = false };

            using (var newPdfDocumentStream = new MemoryStream())
            {
                // vytvor vystupni pdf dokument, pokud neexistuje
                if (outputPdfDocument == null)
                {
                    outputPdfDocument = new PdfDocument(newPdfDocumentStream);
                }

                byte[] fileByteContent = null;

                // otoc obrazek podle EXIF orientace
                if (this.IsImage(name))
                {
                    using (var memoryStream = new MemoryStream(content))
                    {
                        try
                        {
                            using (Image image = Image.FromStream(memoryStream))
                            {
                                // pokud bylo potreba otocit podle pritomne exif informace 
                                if (this.NormalizeOrientation(image))
                                {
                                    using (var imageStream = new MemoryStream())
                                    {
                                        try
                                        {
                                            image.Save(imageStream, image.RawFormat);
                                        }
                                        catch (Exception imgSaveEx)
                                        {
                                            // u nekterych obrazku nefgunguje ukladani v jejich originalnim formatu/kodeku, tak zkus ulozit jako jpeg
                                            var encoderParameters = new EncoderParameters(1);
                                            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                                            image.Save(imageStream, GetEncoder(ImageFormat.Jpeg), encoderParameters);
                                        }

                                        fileByteContent = imageStream.ToArray();
                                    }
                                }
                                else
                                {
                                    // pokud se obrazek neotacel, nech ho, jak je a zbytecne neprevadej na image. Muze narust na velikosti.
                                    fileByteContent = content;
                                }
                            }
                        }
                        catch (ArgumentException argex)
                        {
                            result.IsSuccess = false;
                            result.ErrorModel = ERROR_CODES.UploadedFileUnrecognizedFormat();
                            return result;
                        }
                    }
                }
                else
                {
                    fileByteContent = content;
                }

                // pridej file do db modelu
                var fileModel = new DbFileModel()
                {
                    Key = fileId,
                    FileName = name,
                    FileExtension = Path.GetExtension(name).TrimStart('.'),
                    MimeType = MimeMapping.GetMimeMapping(name),
                    Content = fileByteContent,
                };

                group.OriginalFiles.Add(fileModel);

                // zapis file do pdfka v groupe
                try
                {
                    AddFileToOutputPdfDocument(outputPdfDocument, fileByteContent, name);
                }
                catch (InvalidOperationException invex)
                {
                    if (invex.Message != null && invex.Message.Contains("The file is not a valid PDF document."))
                    {
                        {
                            result.IsSuccess = false;
                            result.ErrorModel = ERROR_CODES.UploadedFileUnrecognizedFormat();
                            return result;
                        }
                    }
                    else
                        throw;
                }

                // ulozim vyslednou podobu OriginalFiles (pridany novy soubor k tem predchazejicim)
                this.SaveOutputPdfDocumentToGroup(group, outputPdfDocument);

                CompressGroup(group, outputPdfDocument, this.GroupResultingFileSizeLimit);


                // pokud i po pokusu o kompresi vysledny soubor presahuje limit, odstran tenhle posledni nahrany soubor
                if (group.OutputFile.Content.LongLength > this.GroupResultingFileSizeLimit)
                {
                    result.ErrorModel = ERROR_CODES.UploadedFilesGroupSizeExceeded(); // $"Group files size limit { this.GroupResultingFileSizeLimit} exceeeded, actual size would be { group.OutputFile.Content.LongLength } bytes.");
                    result.IsSuccess = false;

                    // nemusi se odstranovat, pokud se v controlleru vrati BadRequest pred tim, nez se zavola SetGroup pro ulozeni do database 
                    //await this.RemoveFileAsync(group, fileId);
                }
                else
                {                    
                    result.IsSuccess = true;
                }
                result.DbUploadGroupFileModel = group;
                return result;
            }
        }

        protected internal void CompressGroup(DbUploadGroupFileModel group, PdfDocument outputPdfDocument, long groupResultingFileSizeLimit)
        {
            // zmensi soubory, pokud ses nevesel do limitu vysledneho pdfka
            int compressionRounds = 0;

            while (compressionRounds < this.MaxGroupOptimizationRounds)
            {
                if (group.OutputFile.Content.LongLength > groupResultingFileSizeLimit && group.OriginalFiles.Any(f => this.IsCompressable(f.FileName)))
                {
                    using (var newPdfDocumentStream2 = new MemoryStream())
                    {
                        outputPdfDocument = new PdfDocument(newPdfDocumentStream2);

                        this.CompressFiles(group.OriginalFiles, this.GroupResultingFileSizeLimit, compressionRounds, this.MaxGroupOptimizationRounds);

                        foreach (var fileInGroup in group.OriginalFiles)
                        {
                            AddFileToOutputPdfDocument(outputPdfDocument, fileInGroup.Content, fileInGroup.FileName);
                        }

                        // ulozim novy vysledny pdf a zmensene OriginalFiles 
                        this.SaveOutputPdfDocumentToGroup(group, outputPdfDocument);
                        compressionRounds++;
                    }
                }
                else
                    break;
            }
        }

        protected internal bool CompressGroups(List<DbUploadGroupFileModel> groups, long totalSizeLimit)
        {
            // zmensi soubory, pokud ses nevesel do celkoveho limitu vyslednych pdfek
            int compressionRounds = 0;            

            while (compressionRounds < this.MaxGroupOptimizationRounds)
            {
                long totalGroupOutputFilesLength = groups.Sum(g => g.OutputFile.Content.LongLength);
                if (totalGroupOutputFilesLength > totalSizeLimit)
                {                    
                    foreach (var group in groups)
                    {
                        float ratioToTryCompressThisGroup = ((float)totalSizeLimit / (float)totalGroupOutputFilesLength) / ((float)compressionRounds + (float) 1);
                                        // this.CompressFiles(group.OriginalFiles, (long)(group.OutputFile.Content.LongLength * ratioToTryCompressThisGroup), compressionRounds, this.MaxAllGroupsOptimizationRounds);
                        this.CompressGroup(group, null, (long)(group.OutputFile.Content.LongLength * ratioToTryCompressThisGroup));
                    }
                }
                else
                {
                    break;                     
                }
                compressionRounds++;
            }

            long resultingTotalGroupOutputFilesLength = groups.Sum(g => g.OutputFile.Content.LongLength);
            return (resultingTotalGroupOutputFilesLength <= totalSizeLimit);
        }


        protected internal void SaveOutputPdfDocumentToGroup(DbUploadGroupFileModel group, PdfDocument outputPdfDocument)
        {
            using (var outputFileWriteMemoryStream = new MemoryStream())
            {
                // je potreba nastavit verzi, jinak se Acrobat uzivatele pri zavirani souboru pta, jestli chce ulozit zmeny (je rozdil, kdyz se uklada pdf file a stream)
                if (outputPdfDocument.Version == 0 )
                    outputPdfDocument.Version = 14;

                outputPdfDocument.Save(outputFileWriteMemoryStream);
                group.OutputFile.Content = outputFileWriteMemoryStream.ToArray();
                //group.OutputFile.Size = group.OutputFile.Content.Length;
            }
        }

        /// <inheritdoc/>
        public async Task<DbUploadGroupFileModel> RemoveFileAsync(DbUploadGroupFileModel group, string fileId)
        {
            if (group != null)
            {
                if (group.OriginalFiles.Count > 0)
                {
                    var fileToRemove = group.OriginalFiles.FirstOrDefault(f => f.Key == fileId);
                    if (fileToRemove != null)
                    {
                        group.OriginalFiles.Remove(fileToRemove);

                        if (group.OriginalFiles.Count > 0)
                        {
                            // nove vygeneruju vysledne pdfko, protoze konkretni soubor nevim jak z nej vymazat

                            using (var newPdfStream = new MemoryStream())
                            {
                                var outputPdfDocument = new PdfDocument(newPdfStream);

                                foreach (var fileInGroup in group.OriginalFiles)
                                {
                                    AddFileToOutputPdfDocument(outputPdfDocument, fileInGroup.Content, fileInGroup.FileName);
                                }

                                this.SaveOutputPdfDocumentToGroup(group, outputPdfDocument);
                            }
                        }
                    }
                // TODO: mam neco vyhazovat, kdyz je tu neco divnyho? file neexistuje, kolekce prazdna,.. ?
                }
            }

            return group;
        }

        protected internal void AddFileToOutputPdfDocument(PdfDocument outputPdfDocument, byte[] fileByteContent, string name)
        {
            if (this.IsPdf(name))
            {
                this.AppendPdfToPdf(fileByteContent, outputPdfDocument);
            }
            else if (this.IsImage(name))
            {
                //PdfPage pdfPageForImage = new PdfPage(outputPdfDocument);
                this.AppendImageToPdf(fileByteContent, outputPdfDocument, 0, 0, 1, name);
            }
        }

        protected internal void CompressFiles(List<DbFileModel> originalFiles, long totalFileSizeLimit,  int compressionRoundsElapsed, int compressionRoundsMax)
        {
            if (originalFiles.Count == 0)
            {
                return;
            }

            long compressableFilesSize = originalFiles.Where(f => this.IsCompressable(f.FileExtension)).Sum(f => f.Size);
            long uncompressableFilesSize = originalFiles.Where(f => !this.IsCompressable(f.FileExtension)).Sum(f => f.Size);
            long totalLimitForCompressableFiles = totalFileSizeLimit - uncompressableFilesSize;

            float needToCompressTimes = 1;
            if (totalLimitForCompressableFiles > 0)
            {
                needToCompressTimes = compressableFilesSize / totalLimitForCompressableFiles;


                foreach (var originalFile in originalFiles.Where(f => this.IsCompressable(f.FileExtension)))
                {
                    if (this.IsCompressable(originalFile.FileExtension) && this.IsImage(originalFile.FileExtension))
                    {
                        using (var memoryStream = new MemoryStream(originalFile.Content))
                        {
                            using (Image imageOriginal = Image.FromStream(memoryStream))
                            {
                                // TODO: lepsi odhad potrebneho pomeru komprese datova velikost vs rozmer
                                float imageReductionByNeeded = ((float)Math.Sqrt(needToCompressTimes) + (float)0.0 + (float)Math.Pow(compressionRoundsElapsed, 2));
                                // sichr je sichr, hlavne nezvetsovat
                                if (imageReductionByNeeded < 1)
                                    imageReductionByNeeded = 1;

                                float ratio = 1 / (imageReductionByNeeded);
                                using (Image resizedImage = this.ResizeImage(imageOriginal, this.MinImageSizeNoResize.Width, this.MinImageSizeNoResize.Height, ratio, this.MaxImageSizeAfterResize.Width, this.MaxImageSizeAfterResize.Height))
                                {
                                    using (MemoryStream resizedImageMemoryStream = new MemoryStream())
                                    {
                                        resizedImage.Save(resizedImageMemoryStream, resizedImage.RawFormat);
                                        if (resizedImageMemoryStream.Length < originalFile.Content.Length)
                                        {
                                            // prepis puvodni obraze jedine pokud je novy datove mensi
                                            originalFile.Content = resizedImageMemoryStream.ToArray();
                                           // originalFile.Size = originalFile.Content.Length;
                                        }
                                        else
                                        {
                                            // jinak nasad natrdo jpeg a cim dal brutalnejsi ztratovost
                                            using (MemoryStream resizedImageMemoryStreamJpeg = new MemoryStream())
                                            {
                                                var encoderParameters = new EncoderParameters(1);
                                                long quality = (90L - 10L * (long)compressionRoundsElapsed) > 0 ? 90L - (long)10L * compressionRoundsElapsed : 30L;
                                                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                                                resizedImage.Save(resizedImageMemoryStreamJpeg, GetEncoder(ImageFormat.Jpeg), encoderParameters);

                                                if (resizedImageMemoryStreamJpeg.Length < originalFile.Content.Length)
                                                {
                                                    originalFile.Content = resizedImageMemoryStreamJpeg.ToArray();
                                         //           originalFile.Size = originalFile.Content.Length;
                                                }
                                                else
                                                {
                                                    // tak to uz fakt nevim. Treba pri pristim pruchodu.
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        protected internal ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        protected internal bool IsCompressable(string filename)
        {
            return this.IsImage(filename);
        }

        protected internal bool IsPdf(string filename)
        {
            return filename?.ToLower().EndsWith("pdf")?? false;
        }

        protected internal bool IsImage(string filename)
        {
            return !IsPdf(filename);
        }

        /// <summary>
        /// Otoci obrazek podle Exif tagu, pokud tam je
        /// </summary>
        /// <param name="image"></param>
        /// <returns>True, pokud Image nejak otocil. Jinak false.</returns>
        protected internal bool NormalizeOrientation(Image image)
        {
            const int ExifOrientationTagId = 274;
            bool retval = false;

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
                    retval = true;
                }
            }
            return retval;
        }

        protected internal Size GetMinimalDimensions(int origwidth, int origheight, int minwidth, int minheight)
        {
            double scale1, scale2;
            scale1 = (double)origwidth / minwidth;
            scale2 = (double)origheight / minheight;
            if (scale1 > scale2)
            {
                return new Size((int)Math.Round(origwidth / scale2), minheight);
            }
            return new Size(minwidth, (int)Math.Round(origheight / scale1));
        }


        /// <summary>
        /// Zmensi bitmap obrazek
        /// </summary>
        /// <param name="image"></param>
        /// <param name="minWidth"></param>
        /// <param name="minHeight"></param>
        /// <param name="ratio">Desetinne cislo, jak moc se ma obrazek zmensit</param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        protected internal Image ResizeImage(Image image, int minWidth, int minHeight, float? ratio = 1, int? maxWidth = int.MaxValue, int? maxHeight = int.MaxValue)
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

            // pokud bych zmensoval pod minima
            Size newSize = GetMinimalDimensions(newWidth, newHeight, minWidth, minHeight);

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newSize.Width, newSize.Height);
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
        protected internal Bitmap ResizeImage(Image mg, Size newSize)
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
        protected internal void AppendPdfToPdf(byte[] pdf, PdfDocument outputDocument)
        {
            using (var msPdf = new MemoryStream(pdf))
            {
                var inputDocument = PdfReader.Open(msPdf, PdfDocumentOpenMode.Import);
                // pridam stranku po strance
                var count = inputDocument.PageCount;
                for (var i = 0; i < count; i++)
                {
                    var page = inputDocument.Pages[i];
                    outputDocument.AddPage(page);
                }
            }
        }

        protected internal void AppendImageToPdf(byte[] img, PdfDocument outputPdfDocument, int xPosition, int yPosition, double scale, string footerText)
        {
            var pdf = new PdfPage(outputPdfDocument);
            var gfx = XGraphics.FromPdfPage(pdf);

            {
                using (var msImg = new MemoryStream(img))
                {
                    //var image = Image.FromStream(msImg);
                    //var ximg = XImage.FromGdiPlusImage(image);
                    var ximg = XImage.FromStream(msImg);

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

            outputPdfDocument.AddPage(pdf);
        }

        /// <summary>
        /// Vytvori bitmap obrazek z bytoveho pole
        /// </summary>
        /// <param name="imgBytes"></param>
        protected internal Image CreateBitmap(byte[] imgBytes)
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
