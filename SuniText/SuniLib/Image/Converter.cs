using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Web.UI.WebControls;
using System.Web.UI;
using SuniText.SuniClass;

namespace SuniText.SuniLib.Image
{
    public static class Converter
    {
        [DllImport("gdi32")]
        public static extern int DeleteObject(IntPtr o);
        private static readonly ImageFormat format = ImageFormat.Png;
        public static string SaveBmpImgByMD5(BitmapImage bmpimg)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmpimg));
            string filePath = Pairs.DataFolder.ImageFile;
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            encoder.Save(fileStream);
            fileStream.Dispose();
            return filePath;
        }
        public static BitmapSource Bytes2BmpSrc(string name)
        {
            byte[] resbmp = (byte[])new ResourceManager(typeof(Properties.Resources)).GetObject(name);
            using (Stream stream = new MemoryStream(resbmp))
            {
                BitmapImage image = new BitmapImage();
                stream.Position = 0;
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }
        public static BitmapSource ResBmp2BmpSrc(string resname)
        {
            IntPtr resbmp = ((Bitmap)new ResourceManager(typeof(Properties.Resources)).GetObject(resname)).GetHbitmap();
            BitmapSource bmpsrc = Imaging.CreateBitmapSourceFromHBitmap
                (
                resbmp,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
                );
            _ = DeleteObject(resbmp);
            return bmpsrc;
        }
        public static BitmapImage BmpSrc2BmpImg(BitmapSource bmpsrc)
        {
            return Bmp2BmpImg(BmpSrc2Bmp(bmpsrc));
        }
        public static BitmapSource Bmp2BmpSrc(Bitmap bmp)
        {
            IntPtr ip = bmp.GetHbitmap();
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                ip, IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ip);//释放对象
            return bitmapSource;
        }
        public static Bitmap BmpSrc2Bmp(BitmapSource bmpsrc)
        {
            MemoryStream outStream = new MemoryStream();
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bmpsrc));
            enc.Save(outStream);
            return new Bitmap(outStream);
        }
        public static BitmapImage Bmp2BmpImg(Bitmap bmp)
        {
            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, format);
            bmp.Dispose();
            stream.Position = 0;
            BitmapImage bmpimg = new BitmapImage();
            bmpimg.BeginInit();
            bmpimg.CacheOption = BitmapCacheOption.OnLoad;
            bmpimg.StreamSource = stream;
            bmpimg.EndInit();
            bmpimg.Freeze();
            return bmpimg;
        }
        public static Bitmap BmpImg2Bmp(BitmapImage bmpimg)
        {
            MemoryStream stream = new MemoryStream();
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bmpimg));
            enc.Save(stream);
            Bitmap bitmap = new Bitmap(stream);
            return new Bitmap(bitmap);
        }
        public static BitmapImage Read2BmpImg(string path)
        {
            return new BitmapImage(new Uri(path, UriKind.Absolute));//"../Images/test.png"
        }
        public static void SaveBmpImg(BitmapImage bmpimg, string path)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmpimg));
            FileStream fileStream = new FileStream(path, FileMode.Create);
            encoder.Save(fileStream);
            fileStream.Dispose();
        }
        public static string BmpImg2Base64(BitmapImage bmpimg)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmpimg));
            MemoryStream stream = new MemoryStream();
            encoder.Save(stream);
            _ = stream.Seek(0, SeekOrigin.Begin);
            return Convert.ToBase64String(stream.ToArray());
        }
        public static BitmapImage Base642BmpImg(string base64)
        {
            byte[] b = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(b);
            Bitmap bitmap = new Bitmap(ms);
            BitmapImage bmpimg = new BitmapImage();
            MemoryStream newms = new MemoryStream();
            bitmap.Save(newms, bitmap.RawFormat);
            bmpimg.BeginInit();
            bmpimg.StreamSource = newms;
            bmpimg.CacheOption = BitmapCacheOption.OnLoad;
            bmpimg.EndInit();
            bmpimg.Freeze();
            return bmpimg;
        }
    }
}
