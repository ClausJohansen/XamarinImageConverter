using ImageConverter.Enms;
using System.IO;

namespace ImageConverter.Entities
{
    /// <summary>
    /// Entity class: Holds information about a single image in the processing queue.
    /// </summary>
    public class ImageConversion
    {
        public FileInfo SourceFile { get; }
        public string TargetName { get; set; }
        public ImageConversionOptions resizeBy { get; set; }
        public int TargetWidth { get; set; }
        public int TargetHeight { get; set; }
        public TargetResolutionOptions TargetResolution { get; set; }


        /// <summary>
        /// Constructor: Create a new ImageConversion instance.
        /// </summary>
        /// <param name="file">Information about the source file of the image</param>
        public ImageConversion(FileInfo sourceFile)
        {
            this.SourceFile = sourceFile;
            resizeBy = ImageConversionOptions.Width;
        }


        /// <summary>
        /// The filename of the image.
        /// </summary>
        public override string ToString()
        {
            return SourceFile.Name;
        }
    }
}
