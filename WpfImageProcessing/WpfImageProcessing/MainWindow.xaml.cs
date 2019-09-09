using ImageProcessingLib;
using Microsoft.Win32;
using PerformanceCheck;
using System;
using System.Threading;
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

            //lblPerformance.Content = String.Concat("CPP: ", ImageProcessing.DisplayHelloFromDLL());     // testing C++ connection
        }

        private BitmapImage orginalImage;
        private CancellationTokenSource cts = new CancellationTokenSource();


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
        private void BtnConvertImage_Click(object sender, RoutedEventArgs e)
        {
            string nano, mili;
            bool isHighResolution;

            btnLoad.IsEnabled = false;

            OperationsTimer.StartMeasurement();

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
                    //BitmapImage result = await Task.Run(() => {
                    //    return originalImage.ToBitmap().ToGrayscale(cts.Token).ToBitmapImage();
                    //});
                    //imgPhotoConverted.Source = result;

                    //<!--!> BELOW is other 'working' implementation using ThreadPool in scenario when TASK(Promise) is returned from diff Thread/Context - it is less elegant(requires a delegation of following tasks into UI SynchronizationContext) but I think it is worth mentioning:

                    var sc = SynchronizationContext.Current;

                    ThreadPool.QueueUserWorkItem(async delegate
                    {
                        // work on ThreadPool
                        var cts = new CancellationTokenSource();
                        BitmapImage result = await ImageProcessing.GreyscaleAsync(orginalImage, cts.Token);
                        result.Freeze();

                        sc.Post(delegate
                        {
                            // work on the original context (UI)
                            imgPhotoConverted.Source = result;
                            cts.Cancel();

                            (nano, mili, isHighResolution) = OperationsTimer.StopMeasurement();

                            lblPerformance.Content = String.Concat("Total time: ", String.Concat(nano, " ns | "), String.Concat(mili, " ms | "), String.Concat("HiRes: ", isHighResolution ? "Enabled" : "Disabled"));
                        }, null);
                    });

                }
            }
            catch (OperationCanceledException)
            {
                lblPerformance.Content = "The ASYNC function was cancelled!";
            }
            finally
            {
                if (cbAsync.IsChecked == false) // procedural approach
                {
                    (nano, mili, isHighResolution) = OperationsTimer.StopMeasurement();

                    lblPerformance.Content = String.Concat("Total time: ", String.Concat(nano, " ns | "), String.Concat(mili, " ms | "), String.Concat("HiRes: ", isHighResolution ? "Enabled" : "Disabled"));
                }

                btnLoad.IsEnabled = true;
            }
        }

        //public void CancelConversion_Click(object sender, RoutedEventArgs e)
        //{
        //    cts.Cancel();
        //}
    }
}
