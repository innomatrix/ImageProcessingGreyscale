using ImageProcessingLib;
using Microsoft.Win32;
using PerformanceCheck;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfImageProcessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private BitmapImage orginalImage;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
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


        private void BtnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select an image";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";

            if (op.ShowDialog() == true)
            {
                orginalImage = new BitmapImage(new Uri(op.FileName));
                imgPhotoOrginal.Source = orginalImage;
                //imgPhotoConverted.Source = orginalImage;
                btnConvert.IsEnabled = true;
                lblSteps.Content = "Now you can convert the image!";
            }

        }

        //TODO: implement
        //- PROGRESS BAR: injecting new Progress<int>(p => pbDownloadProgress.Value = p)
        //- CANCELLATION ACTION: 'Convert' button changes a label("Cancel") and to CLICL is attached a RoutedEventHandler CancelConversion_Click() while TASK in progress
        private async void BtnConvertImage_Click(object sender, RoutedEventArgs e)
        {
            string nano, mili;
            bool isHighResolution;

            OperationsTimer.StartMeasurement();

            btnLoad.IsEnabled = false;

            var originalImage = (imgPhotoOrginal.Source as BitmapImage);

            try
            {
                if (cbAsync.IsChecked == false)
                {
                    imgPhotoConverted.Source = originalImage.ToBitmap().ToGrayscale(cts.Token).ToBitmapImage();
                }
                else
                {
                    cts.CancelAfter(TimeSpan.FromSeconds(1));
                    BitmapImage result = await Task.Run(() => {
                        return originalImage.ToBitmap().ToGrayscale(cts.Token).ToBitmapImage();
                    });
                    imgPhotoConverted.Source = result;

                    //<!--!> BELOW is other 'working' implementation using ThreadPool in scenario when TASK(Promise) is returned from diff Thread/Context - it is less elegant(requires a delegation of following tasks into UI SynchronizationContext) but I think it is worth mentioning:

                    //var sc = SynchronizationContext.Current;

                    //ThreadPool.QueueUserWorkItem(async delegate
                    //{
                    //    // work on ThreadPool
                    //    var cts = new CancellationTokenSource();
                    //    BitmapImage result = await ImageProcessing.GreyscaleAsync(orginalImage, cts.Token);
                    //    result.Freeze();

                    //    sc.Post(delegate
                    //    {
                    //        // work on the original context (UI)
                    //        imgPhotoConverted.Source = result;
                    //        cts.Cancel();

                    //        // here we finish the performance tests (!sic)
                    //        string nano, mili;
                    //        bool isHighResolution;

                    //        (nano, mili, isHighResolution) = OperationsTimer.StopMeasurement();

                    //        lblPerformance.Content = String.Concat("Total time: ", String.Concat(nano, " ns | "), String.Concat(mili, " ms | "), String.Concat("HiRes: ", isHighResolution ? "Enabled" : "Disabled"));
                    //    }, null);
                    //});

                }
            }
            catch (OperationCanceledException)
            {
                lblPerformance.Content = "The ASYNC function was cancelled!";
            }
            finally
            {
                (nano, mili, isHighResolution) = OperationsTimer.StopMeasurement();

                lblPerformance.Content = String.Concat("Total time: ", String.Concat(nano, " ns | "), String.Concat(mili, " ms | "), String.Concat("HiRes: ", isHighResolution ? "Enabled" : "Disabled"));
                }

                btnLoad.IsEnabled = true;
        }

        private void CancelConversion_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }
    }
}
