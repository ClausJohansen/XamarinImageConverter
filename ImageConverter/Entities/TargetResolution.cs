using ImageConverter.Enms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverter.Entities
{
    public class TargetResolution
    {
        public string Name { get; set; }
        public TargetResolutionOptions Value { get; set; }

        //TODO: image conversion factor?

        public override string ToString()
        {
            return Name;
        }
    }
}
