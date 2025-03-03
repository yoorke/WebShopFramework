using eshopBE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBLInterfaces
{
    public interface IImageConvertor
    {
        string ConvertImageToWebP(string imagePath);
    }
}
