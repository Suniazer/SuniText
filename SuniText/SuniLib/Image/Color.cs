using SuniText.SuniLib.WinSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuniText.SuniLib.Image
{
    public static class Color
    {
        public static void ColorByPoint(out System.Windows.Media.Color c, out System.Windows.Media.Color nc, out System.Drawing.Point p)
        {
            System.Drawing.Point point = new System.Drawing.Point();
            _ = Win32Api.GetCursorPos(ref point);
            IntPtr displayDC = Win32Api.CreateDC("DISPLAY", null, null, IntPtr.Zero);
            uint color = Win32Api.GetPixel(displayDC, point.X, point.Y);
            _ = Win32Api.DeleteDC(displayDC);
            byte Red = (byte)color;
            byte Green = (byte)(((short)color) >> 8);
            byte Blue = (byte)((color) >> 16);
            _ = (byte)((color) >> 24); ;
            c = System.Windows.Media.Color.FromArgb(255, Red, Green, Blue);
            nc = System.Windows.Media.Color.FromArgb(255, (byte)(byte.MaxValue - c.R), (byte)(byte.MaxValue - c.G), (byte)(byte.MaxValue - c.B));
            p = point;
        }
    }
}
