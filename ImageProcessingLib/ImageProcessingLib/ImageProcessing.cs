using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace ImageProcessingLib
{
    public static class ImageProcessing
    {

    public static Bitmap ToGrayscale(this Bitmap source, CancellationToken cancellationToken = default)
    {
        Bitmap output = new Bitmap(source.Width, source.Height);
        for (int i = 0; i<source.Width; i++ )
        {
                for (int x = 0; x<source.Height; x++ )
            {
                cancellationToken.ThrowIfCancellationRequested();
                var imageColor = source.GetPixel(i, x);
                int grayScale = (int)((imageColor.R * 0.21) + (imageColor.G * 0.72) + (imageColor.B * 0.07));
                var newColor = Color.FromArgb(imageColor.A, grayScale, grayScale, grayScale);
                output.SetPixel(i, x, newColor);
            }
}
        return output;
    }

    public static Bitmap ToBitmap(this BitmapImage source)
    {
        using (MemoryStream outStream = new MemoryStream())
        {
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(source));
            enc.Save(outStream);
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

            return new Bitmap(bitmap);
        }
    }

    public static BitmapImage ToBitmapImage(this Bitmap source)
    {
        using (var memory = new MemoryStream())
        {
            source.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }
    }

        ///// OTHER ASYNC(TASK) IMPLEMENTATION(s) -> ThreadPool implementation solution

        ////1a) return BitmapSource
        //public static async Task<BitmapSource> GreyscaleAsync(
        //                       Bitmap inputBitmap, CancellationToken cancellationToken)
        //{
        //    return await Task.Run(() =>
        //    {
        //        return inputBitmap.ToBitmap().ToGrayscale(cancellationToken.Token).ToBitmapSource();
        //    }, cancellationToken);
        //}

        ////1b) return BitmapImage (@overload)
        //public static async Task<BitmapImage> GreyscaleAsync(
        //              BitmapImage inputBitmapImage, CancellationToken cancellationToken)
        //{
        //    return await Task.Run(() =>
        //        {
        //            return inputBitmap.ToBitmap().ToGrayscale(cancellationToken.Token).ToBitmapImage();
        //        }, cancellationToken);
        //}

        ////2-1) return BitmapSource + for wrapper implementation
        //public static Task<BitmapImage> ConvertTask(BitmapImage inputBitmapImage, CancellationToken cancellationToken)
        //{
        //    return Task.Run(() =>
        //    {
        //        Bitmap inputBitmap = ToBitmap(inputBitmapImage);
        //        Bitmap outputImage = new Bitmap(inputBitmap.Width, inputBitmap.Height);
        //        for (int i = 0; i < inputBitmap.Width; i++)
        //        {
        //            for (int x = 0; x < inputBitmap.Height; x++)
        //            {
        //                cancellationToken.ThrowIfCancellationRequested();
        //                Color imageColor = inputBitmap.GetPixel(i, x);
        //                int grayScale = (int)((imageColor.R * 0.21) + (imageColor.G * 0.72) + (imageColor.B * 0.07));
        //                Color newColor = Color.FromArgb(imageColor.A, grayScale, grayScale, grayScale);
        //                outputImage.SetPixel(i, x, newColor);
        //            }
        //        }
        //        return ToBitmapImage(outputImage);
        //    }, cancellationToken);
        //}

        ////2) Wrapper ConvertAsync to be invoked in the ThreadPool
        //public static async Task<BitmapImage> ConvertAsync(BitmapImage inputBitmapImage, CancellationToken cancellationToken)
        //{
        //    return await ConvertTask(inputBitmapImage, cancellationToken).ConfigureAwait(false);
        //}

        //// not the best transformation :(
        //public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        //{
        //    var bitmapData = bitmap.LockBits(
        //        new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
        //        System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

        //    var bitmapSource = BitmapSource.Create(
        //        bitmapData.Width, bitmapData.Height,
        //        bitmap.HorizontalResolution, bitmap.VerticalResolution,
        //        PixelFormats.Bgr24, null,
        //        bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

        //    bitmap.UnlockBits(bitmapData);
        //    return bitmapSource;
        //}
    }
}
