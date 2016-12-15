using ImageConverter.Enms;
using ImageConverter.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverter
{
    public static class ImageProcessor
    {
        // TODO: Dynamisk bestemmelse af projektnavn
        private static string solutionName = "WeatherApp";
        private static List<ConversionDestination> destinations = new List<ConversionDestination>
        {
            new ConversionDestination
            {
                Name = "Andriod drawable",
                Resolution = TargetResolutionOptions.Mdpi,
                SubDirectory = String.Format("\\{0}\\{0}.Driod\\Resources\\drawable\\", solutionName),
                Suffix = ""
            },

            new ConversionDestination
            {
                Name = "Andriod drawable-hdpi",
                Resolution = TargetResolutionOptions.Hdpi,
                SubDirectory = String.Format("\\{0}\\{0}.Driod\\Resources\\drawable-hdpi\\", solutionName),
                Suffix = ""
            },

            new ConversionDestination
            {
                Name = "Andriod drawable-xhdpi",
                Resolution = TargetResolutionOptions.Xhdpi,
                SubDirectory = String.Format("\\{0}\\{0}.Driod\\Resources\\drawable-xhdpi\\", solutionName),
                Suffix = ""
            },

            new ConversionDestination
            {
                Name = "Andriod drawable-xxhdpi",
                Resolution = TargetResolutionOptions.Xxhdpi,
                SubDirectory = String.Format("\\{0}\\{0}.Driod\\Resources\\drawable-xxhdpi\\", solutionName),
                Suffix = ""
            },

            new ConversionDestination
            {
                Name = "IOS standard size",
                Resolution = TargetResolutionOptions.Mdpi,
                SubDirectory = String.Format("\\{0}\\{0}.IOS\\Resources\\", solutionName),
                Suffix = ""
            },

            new ConversionDestination
            {
                Name = "IOS 2X",
                Resolution = TargetResolutionOptions.Xhdpi,
                SubDirectory = String.Format("\\{0}\\{0}.IOS\\Resources\\", solutionName),
                Suffix = "@2x"
            },

            new ConversionDestination
            {
                Name = "IOS 3X",
                Resolution = TargetResolutionOptions.Xxhdpi,
                SubDirectory = String.Format("\\{0}\\{0}.IOS\\Resources\\", solutionName),
                Suffix = "@3x"
            },

        };



        public static void ProcessQueue(List<ImageConversion> queue, DirectoryInfo targetDirectory)
        {

            foreach(ImageConversion ic in queue)
            {
                //TODO: Async / await

                // Calculate the relative size of the DPI factors.
                HdpiFactorResult factor;
                Image sourceImage = Image.FromFile(ic.SourceFile.FullName);

                if (ic.ResizeBy == ImageConversionOptions.Width)
                    factor = CalculateFactor(ic.TargetWidth, sourceImage.Width, ic.TargetResolution);
                else
                    factor = CalculateFactor(ic.TargetHeight, sourceImage.Height, ic.TargetResolution);

                // Get cropped image data from source image
                int cropSize;

                if (ic.Crop)
                    cropSize = ic.CropSize;
                else
                    cropSize = 0;

                Bitmap croppedImageData = CropImage(sourceImage, cropSize);

                

                foreach(ConversionDestination dest in destinations)
                {
                    double resizeFactor;

                    switch (dest.Resolution)
                    {
                        case TargetResolutionOptions.Mdpi:

                            resizeFactor = factor.MdpiFactor;
                            break;

                        case TargetResolutionOptions.Hdpi:

                            resizeFactor = factor.HdpiFactor;
                            break;

                        case TargetResolutionOptions.Xhdpi:

                            resizeFactor = factor.XhdpiFactor;
                            break;

                        case TargetResolutionOptions.Xxhdpi:

                            resizeFactor = factor.XxhdpiFactor;
                            break;

                        case TargetResolutionOptions.Xxxhdpi:

                            resizeFactor = factor.XxxhdpiFactor;
                            break;

                        default:
                            throw new Exception("FEJ!!!");
                    }

                    // Resize
                    Bitmap resizeImage = ResizeImage(croppedImageData, resizeFactor);

                    // Save the resulting image in the correct project folders.
                    StringBuilder saveDestination = new StringBuilder();

                    saveDestination.Append(targetDirectory);
                    saveDestination.Append(dest.SubDirectory);
                    saveDestination.Append(Path.GetFileNameWithoutExtension(ic.TargetName));
                    saveDestination.Append(dest.Suffix);
                    saveDestination.Append(Path.GetExtension(ic.TargetName));

                    // TODO: Promt user on irregularities...
                    if (!Directory.Exists(Path.GetDirectoryName(saveDestination.ToString())))
                        Directory.CreateDirectory(Path.GetDirectoryName(saveDestination.ToString()));

                    if (File.Exists(saveDestination.ToString()))
                        File.Delete(saveDestination.ToString());

                    resizeImage.Save(saveDestination.ToString());

                }

            }
        }

        private static Bitmap CropImage(Image sourceImage, int cropSize)
        {
            // Crop the image.
            var cropSourceRect = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
            var cropDestRect = new Rectangle(cropSize, cropSize, sourceImage.Width - (cropSize * 2), sourceImage.Height - (cropSize * 2));
            var croppedImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            using (var graphics = Graphics.FromImage(croppedImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(sourceImage, cropSourceRect, cropDestRect, GraphicsUnit.Pixel);
            }

            return croppedImage;
        }

        private static Bitmap ResizeImage(Image sourceImage, double resizeFactor)
        {
            int targetWith = (int) Math.Round(sourceImage.Width * resizeFactor);
            int targetHeight = (int) Math.Round(sourceImage.Height * resizeFactor);

            var resizeDestRect = new Rectangle(0, 0, targetWith, targetHeight);
            var resizedImage = new Bitmap(targetWith, targetHeight);

            resizedImage.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);

            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(sourceImage, resizeDestRect, 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return resizedImage;
        }

        private static HdpiFactorResult CalculateFactor(int targetSize, int originalSize, TargetResolutionOptions resolution)
        {
            // Well I coudnt really find any clever way to this this but just
            // make a branc for each size and "bruteforce" the problem.

            // Hdpi sizes based on https://developer.android.com/guide/practices/screens_support.html#DesigningResources
            // IOS standards: http://sebastien-gabriel.com/designers-guide-to-dpi/

            // Alternative proposal: Calculate the mdpi or hdpi size as a baseline and use a single code block to multiply to other sizes.

            double baseFactor = (double) targetSize / originalSize;


            var result = new HdpiFactorResult();

            switch(resolution)
            {
                case TargetResolutionOptions.Mdpi:

                    result.MdpiFactor = baseFactor;
                    result.HdpiFactor = baseFactor * 1.5;
                    result.XhdpiFactor = baseFactor * 2;
                    result.XxhdpiFactor = baseFactor * 3;
                    result.XxxhdpiFactor = baseFactor * 4;

                    break;

                case TargetResolutionOptions.Hdpi:

                    result.MdpiFactor = baseFactor * 0.6666;
                    result.HdpiFactor = baseFactor;
                    result.XhdpiFactor = baseFactor * 1.3333;
                    result.XxhdpiFactor = baseFactor * 2;
                    result.XxxhdpiFactor = baseFactor * 2.666;

                    break;

                case TargetResolutionOptions.Xhdpi:

                    result.MdpiFactor = baseFactor * 0.5;
                    result.HdpiFactor = baseFactor * 0.75;
                    result.XhdpiFactor = baseFactor;
                    result.XxhdpiFactor = baseFactor * 1.5;
                    result.XxxhdpiFactor = baseFactor * 2;

                    break;

                case TargetResolutionOptions.Xxhdpi:

                    result.MdpiFactor = baseFactor * 0.3333;
                    result.HdpiFactor = baseFactor * 0.5;
                    result.XhdpiFactor = baseFactor * 0.6666;
                    result.XxhdpiFactor = baseFactor;
                    result.XxxhdpiFactor = baseFactor * 1.333;

                    break;

                case TargetResolutionOptions.Xxxhdpi:

                    result.MdpiFactor = baseFactor * 0.25;
                    result.HdpiFactor = baseFactor * 0.375;
                    result.XhdpiFactor = baseFactor * 0.5;
                    result.XxhdpiFactor = baseFactor * 0.75;
                    result.XxxhdpiFactor = baseFactor;

                    break;

                default:

                    throw new Exception("CalculateFactor: Not a valid resolution");
            }

            return result;
        }
    }
}
