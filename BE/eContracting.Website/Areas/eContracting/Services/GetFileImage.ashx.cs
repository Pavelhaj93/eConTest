using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using eContracting.Kernel.Services;
using Sitecore.Diagnostics;

namespace eContracting.Website
{
    /// <summary>
    /// Summary description for GetFile
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GetFileImage : IHttpHandler, IRequiresSessionState
    {
        /// <summary>
        /// Request processing.
        /// </summary>
        /// <param name="context">Request context.</param>
        public void ProcessRequest(HttpContext context)
        {
            // DateTime functionbeginTime = DateTime.UtcNow;
            if (context.Session["UserFiles"] != null)
            {
                // DateTime sessionBeginTime = DateTime.UtcNow;
                var files = context.Session["UserFiles"] as List<FileToBeDownloaded>;

                // DateTime sessionEndTime = DateTime.UtcNow;
                if (files != null)
                {
                    var file = context.Request.QueryString["file"];

                    if (file != null)
                    {
                        var pdfFile = null as FileToBeDownloaded;

                        var availableFiles = files.Where(xx => xx.Index == file);
                        if (availableFiles.Count() > 1)
                        {
                            pdfFile = availableFiles.FirstOrDefault(xx => xx.SignedVersion == true);
                            if (pdfFile == null)
                            {
                                pdfFile = availableFiles.FirstOrDefault();
                            }
                        }
                        else
                        {
                            pdfFile = availableFiles.FirstOrDefault();
                        }

                        if (pdfFile != null)
                        {
                            context.Response.Clear();

                            using (var imageStream = new MemoryStream())
                            {
                                this.PrintPdfToImage(new MemoryStream(pdfFile.FileContent.ToArray()), imageStream);

                                var imageArray = imageStream.ToArray();
                                context.Response.Cache.SetExpires(DateTime.Now.AddMinutes(1));
                                context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                                context.Response.Cache.SetValidUntilExpires(true);
                                context.Response.ContentType = "image/png";
                                context.Response.AddHeader("Content-Disposition", $"inline; filename={pdfFile.FileName}.png");
                                context.Response.AddHeader("Content-Length", imageArray.Length.ToString());
                                context.Response.Buffer = true;

                                imageStream.WriteTo(context.Response.OutputStream);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts PDF file to the PNG stream
        /// </summary>
        /// <param name="memoryStream"></param>
        private void PrintPdfToImage(MemoryStream pdfStream, MemoryStream imageStream)
        {
            try
            {
                var pdfImages = new List<Image>();
                using (var document = PdfiumViewer.PdfDocument.Load(pdfStream))
                {
                    for (var i = 0; i < document.PageCount; i++)
                    {
                        var image = document.Render(i, 600, 600, false);
                        pdfImages.Add(image);
                    }
                }
                var bitmap = CombineBitmap(pdfImages);
                bitmap.Save(imageStream, ImageFormat.Png);
            }
            catch (Exception e)
            {
                Log.Error("Unable to generate image from PDF", e, this);
            }
        }

        /// <summary>
        /// Mergne obrazky
        /// https://stackoverflow.com/questions/465172/merging-two-images-in-c-net
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private Bitmap CombineBitmap(IEnumerable<Image> files)
        {
            //read all images into memory
            var images = new List<Bitmap>();
            Bitmap finalImage = null;

            try
            {
                var width = 0;
                var height = 0;

                foreach (var image in files)
                {
                    //create a Bitmap from the file and add it to the list
                    var bitmap = new Bitmap(image);

                    //update the size of the final bitmap
                    width = bitmap.Width > width ? bitmap.Width : width;
                    height += bitmap.Height;

                    images.Add(bitmap);
                }

                //create a bitmap to hold the combined image
                finalImage = new Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (var g = Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(Color.Black);

                    //go through each image and draw it on the final image
                    var offset = 0;
                    foreach (var image in images)
                    {
                        g.DrawImage(image, new Rectangle(0, offset, image.Width, image.Height));
                        offset += image.Height;
                    }
                }

                return finalImage;
            }
            catch (Exception)
            {
                finalImage?.Dispose();
                return null;
            }
            finally
            {
                //clean up memory
                foreach (var image in images)
                {
                    image.Dispose();
                }
            }
        }
        /// <summary>
        /// Gets a value indicating whether another request can use the System.Web.IHttpHandler instance.
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
