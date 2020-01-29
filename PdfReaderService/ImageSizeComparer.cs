using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliClap.Core.PdfReaderService
{
    public class ImageSizeComparer : IComparer<Image>
    {
        public int Compare(Image a, Image b)
        {
            var sizeA = (a.Height * a.Width);
            var sizeB = (b.Height * b.Width);

            if (sizeA == sizeB) return 0;
            return sizeA > sizeB ? -1 : 1;
        }
    }
}
