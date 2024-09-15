using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using YourFocus;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media.Animation;

namespace TomatoFocus
{
    public sealed partial class SettingsFocusPage : Page
    {
        public SettingsFocusPage()
        {
            this.InitializeComponent();

            Bg_Selection.SelectedIndex = (Application.Current as App).iUseCustomBackground ? 1:0;
            SetBgBlur_Switch.IsOn = (Application.Current as App).iUseAcrylicBlur;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if(Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async void FileCustomBackground()
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileOpenPicker.FileTypeFilter.Add(".png");
            fileOpenPicker.FileTypeFilter.Add(".jpeg");
            fileOpenPicker.FileTypeFilter.Add(".jpg");
            fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
            var inputFile = await fileOpenPicker.PickSingleFileAsync();
            if (inputFile == null)
                return;
            SoftwareBitmap SoftwareBitmap;
            using (IRandomAccessStream stream = await inputFile.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                SoftwareBitmap = await decoder.GetSoftwareBitmapAsync();
            }
            Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var OutputFile = await StorageFolder.CreateFileAsync("ImmersiveFocusing\\Background.png", CreationCollisionOption.OpenIfExists);

            using (IRandomAccessStream stream = await OutputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetSoftwareBitmap(SoftwareBitmap);
                encoder.BitmapTransform.ScaledWidth = (uint)SoftwareBitmap.PixelWidth;
                encoder.BitmapTransform.ScaledHeight = (uint)SoftwareBitmap.PixelHeight;
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
                encoder.IsThumbnailGenerated = true;
                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception err)
                {
                    const int WINCODEC_ERR_UNSUPPORTEDOPERATION = unchecked((int)0x88982F81);
                    switch (err.HResult)
                    {
                        case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                            encoder.IsThumbnailGenerated = false;
                            break;
                        default:
                            throw;
                    }
                }
                if (encoder.IsThumbnailGenerated == false)
                {
                    await encoder.FlushAsync();
                }
            }

            GetCustomBackground();
        }

        private async void GetCustomBackground()
        {
            if ((Application.Current as App).iUseCustomBackground)
            {
                try
                {
                    Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    StorageFile file = await StorageFolder.GetFileAsync("ImmersiveFocusing\\Background.png");
                    if (file != null)
                    {
                        using (IRandomAccessStream FileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                        {
                            BitmapImage bitmapImage = new BitmapImage();
                            await bitmapImage.SetSourceAsync(FileStream);
                            BgPreview.Source = bitmapImage;
                            bitmapImage = null;
                        }
                    }
                    file = null;

                    OStoryBoardDoubleAnimation.From = 0;
                    OStoryBoardDoubleAnimation.To = 1;
                    OStoryBoard.Begin();
                }
                catch { }
            }
            else
            {

            }
        }

        private void RefreshBgPreview()
        {
            if ((Application.Current as App).iUseCustomBackground && (Application.Current as App).iUseAcrylicBlur)
            {
                BackgroundBlack.Visibility = Visibility.Collapsed;
                BackgroundBlur.Visibility = Visibility.Visible;
            }
            else if((Application.Current as App).iUseCustomBackground && !(Application.Current as App).iUseAcrylicBlur)
            {
                BackgroundBlur.Visibility = Visibility.Collapsed;
                BackgroundBlack.Visibility = Visibility.Visible;
            }
            else
            {
                BackgroundBlur.Visibility = Visibility.Collapsed;
                BackgroundBlack.Visibility = Visibility.Collapsed;
            }
        }

        private void Bg_Selection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Bg_Selection.SelectedIndex == 0)
            {
                GridSetBgBlur.Visibility = Visibility.Collapsed;
                GridSetBgFile.Visibility = Visibility.Collapsed;
                SetBgBlur_Switch.IsOn = true;

                (Application.Current as App).iUseCustomBackground = false;
                (Application.Current as App).iUseAcrylicBlur = true;
                (Application.Current as App).LocalSettings.Values["iUseCustomBackground"] = (Application.Current as App).iUseCustomBackground;
                (Application.Current as App).LocalSettings.Values["iUseAcrylicBlur"] = (Application.Current as App).iUseAcrylicBlur;

                BgPreview.Source = null;
            }
            else if(Bg_Selection.SelectedIndex == 1)
            {
                GridSetBgBlur.Visibility = Visibility.Visible;
                GridSetBgFile.Visibility = Visibility.Visible;

                (Application.Current as App).iUseCustomBackground = true;
                (Application.Current as App).LocalSettings.Values["iUseCustomBackground"] = (Application.Current as App).iUseCustomBackground;

                GetCustomBackground();
            }

            RefreshBgPreview();
        }

        private void SetBgBlur_Switch_Toggled(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).iUseAcrylicBlur = SetBgBlur_Switch.IsOn;
            (Application.Current as App).LocalSettings.Values["iUseAcrylicBlur"] = (Application.Current as App).iUseAcrylicBlur;

            RefreshBgPreview();
        }

        private void SetBgFile_Click(object sender, RoutedEventArgs e)
        {
            FileCustomBackground();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SPanel.Width = (ActualWidth >= 800) ? 720 : ActualWidth - 80;
        }
    }
}
