using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageProcessingLib
{
    public interface IGreyscalable<T>
    {
        BitmapImage ConvertToBitmapImage(T obj);
        Bitmap ConvertToBitmap(T obj);
    }

    public abstract class ImgProcessor : IGreyscalable<Bitmap>
    {
        public abstract BitmapImage ConvertToBitmapImage(Bitmap obj);
        public abstract Bitmap ConvertToBitmap(Bitmap obj);
    }

    //public static class GreyscaleProcessorToBitmapImage : ImgProcessor
    //{
    //    public static BitmapImage ConvertToBitmapImage(Bitmap inputBitmap)
    //    {
    //        return new BitmapImage();
    //    }

    //    public static BitmapImage ConvertToBitmapImage(BitmapImage inputBitmapImage)
    //    {
    //        return new BitmapImage();
    //    }
    //}
}
