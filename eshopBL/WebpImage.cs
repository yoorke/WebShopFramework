using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imazen.WebP;

namespace eshopBL
{
    public static class WebpImage
    {
        public static Image FromFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);

            Image image = new SimpleDecoder().DecodeFromBytes(bytes, bytes.Length);

            return image;
        }
    }
}
