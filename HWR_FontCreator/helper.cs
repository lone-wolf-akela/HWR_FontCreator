using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HWR_FontCreator
{
    public static class helper
    {
        public static Bitmap type2img(Homeworld2.RCF.Image img)
        {
            Bitmap ret = new Bitmap(img.Width, img.Height, img.Width, PixelFormat.Format8bppIndexed,
                Marshal.UnsafeAddrOfPinnedArrayElement(img.Data, 0));
            var palette = ret.Palette;
            for (int i = 0; i < 256; i++)
                palette.Entries[i] = Color.FromArgb(i, i, i);
            ret.Palette = palette;
            return ret;
        }

        public static byte[] img2type(Bitmap bmp)
        {
            var ret = new byte[bmp.Height * bmp.Width];
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    ret[y * bmp.Width + x] = (byte) (bmp.GetPixel(x, y).GetBrightness() * 255);
                }
            }
            return ret;
        }
    }
}
