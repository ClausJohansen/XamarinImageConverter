using ImageConverter.Enms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverter.Entities
{
    public class ConversionDestination
    {
        public string Name { get; set; }
        public TargetResolutionOptions Resolution { get; set; }
        public string SubDirectory { get; set; }
        public string Suffix { get; set; }
    }
}
