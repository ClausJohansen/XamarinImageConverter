using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverter.Entities
{
    public class HdpiFactorResult
    {
        /// <summary>
        /// IOS: standard, Android: "drawable-hdpi"
        /// </summary>
        public double MdpiFactor { get; internal set; }

        /// <summary>
        /// IOS: none, Android: "drawable-hdpi"
        /// </summary>
        public double HdpiFactor { get; internal set; }

        /// <summary>
        /// IOS: @2x, Android: "drawable-xhdpi"
        /// </summary>
        public double XhdpiFactor { get; internal set; }

        /// <summary>
        /// IOS: @3x, Android: "drawable-xxhdpi"
        /// </summary>
        public double XxhdpiFactor { get; internal set; }

        /// <summary>
        /// IOS: @4x, Android: "drawable-xxxhdpi"
        /// </summary>
        public double XxxhdpiFactor { get; internal set; }
    }
}
