using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using FireflyUtility.Structure;

namespace Firefly
{
    /// 为了解决WPF恶心的BitmapSource弄的，不必理会
    static class Bitmap2WritableBitmap
    {
        public static void BitmapToWritableBitmap(WriteableBitmap source, Bitmap bitmap)
        {
            try
            {
                CopyFrom(source, bitmap);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void BitmapToWritableBitmap(WriteableBitmap source, RgbaFloat[] bitmap)
        {
            try
            {
                CopyFrom(source, bitmap);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void CopyFrom(WriteableBitmap wb, Bitmap bitmap)
        {
            if (wb == null)
                throw new ArgumentNullException(nameof(wb));
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            int ws = wb.PixelWidth;
            int hs = wb.PixelHeight;
            int wt = bitmap.Width;
            int ht = bitmap.Height;
            if (ws != wt || hs != ht)
                throw new ArgumentException("暂时只支持相同尺寸图片的复制。");

            int width = ws;
            int height = hs;
            int bytes = ws * hs * wb.Format.BitsPerPixel / 8;

            BitmapData rBitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            wb.Lock();
            unsafe
            {
                Buffer.MemoryCopy(rBitmapData.Scan0.ToPointer(), wb.BackBuffer.ToPointer(), bytes, bytes);
            }

            wb.AddDirtyRect(new Int32Rect(0, 0, width, height));
            wb.Unlock();

            bitmap.UnlockBits(rBitmapData);
        }

        public static void CopyFrom(WriteableBitmap wb, RgbaFloat[] buff)
        {
            if (wb == null)
                throw new ArgumentNullException(nameof(wb));
            if (buff == null)
                throw new ArgumentNullException(nameof(buff));

            int ws = wb.PixelWidth;
            int hs = wb.PixelHeight;

            int width = ws;
            int height = hs;
            int bytes = ws * hs * wb.Format.BitsPerPixel / 8;

            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buff, 0);

            wb.Lock();
            unsafe
            {
                Buffer.MemoryCopy(ptr.ToPointer(), wb.BackBuffer.ToPointer(), bytes, bytes);
            }

            wb.AddDirtyRect(new Int32Rect(0, 0, width, height));
            wb.Unlock();
        }

        public static BitmapSource Bitmap2BitmapImage(Bitmap bitmap) =>
            Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
    }
}
