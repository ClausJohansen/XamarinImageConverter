using ImageConverter.Enms;
using ImageConverter.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverter
{
    public static class ImageProcessor
    {
        public static void ProcessQueue(List<ImageConversion> queue, DirectoryInfo targetDirectory)
        {

            foreach(ImageConversion image in queue)
            {
                //TODO: Async / await


                HdpiFactorResult factor;

                if (image.ResizeBy == ImageConversionOptions.Width)
                {
                    factor = CalculateFactor(image.TargetWidth, image.TargetResolution);
                }
                else
                {
                    factor = CalculateFactor(image.TargetHeight, image.TargetResolution);
                }

                if(image.TargetResolution == TargetResolutionOptions.Hdpi)
                {
                    Bitmap hdpiBitmap = new Bitmap(image.SourceFile.FullName);
                    Bitmap xhdpiBitmap = hdpiBitmap.

                }

                


                

                // if(image.resizeBy = Im)

            }
        }

        private static HdpiFactorResult CalculateFactor(int source, TargetResolutionOptions resolution)
        {
            // Well I coudnt really find any clever way to this this but just
            // make a branc for each size and "bruteforce" the problem.

            // Hdpi sizes based on https://developer.android.com/guide/practices/screens_support.html#DesigningResources

            // Alternative proposal: Calculate the mdpi or hdpi size as a baseline and use a single code block to multiply to other sizes.


            var result = new HdpiFactorResult();

            switch(resolution)
            {
                case TargetResolutionOptions.Hdpi:

                    result.HdpiSize = source;
                    result.XhdpiSize = (int)Math.Round(source * 1.3333);
                    result.XxhdpiSize = source * 2;

                    break;

                case TargetResolutionOptions.Xhdpi:

                    result.HdpiSize = (int)Math.Round(source * 0.75);
                    result.XhdpiSize = source;
                    result.XxhdpiSize = (int)Math.Round(source * 1.5);

                    break;

                case TargetResolutionOptions.Xxhdpi:

                    result.HdpiSize = (int) Math.Round(source * 0.5);
                    result.XhdpiSize = (int)Math.Round(source * 0.6666);
                    result.XxhdpiSize = source;

                    break;

                default:

                    throw new Exception("CalculateFactor: Not a valid resolution");
            }

            return result;
        }
    }
}
