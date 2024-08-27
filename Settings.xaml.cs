using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace TomatoFocus
{
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
            TextVersion.Text = string.Format("名称：专注印记\n版本：{0}.{1}.{2}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);
            Theme_Selection.SelectedIndex = (int)(Application.Current as App).LocalSettings.Values["Theme"];
            int[] selections = new int[241];
            selections[25] = 0; selections[30] = 1; selections[35] = 2; selections[40] = 3; selections[45] = 4; selections[50] = 5; selections[55] = 6; selections[60] = 7;
            OnceFocus_Selection.SelectedIndex = selections[(Application.Current as App).OnceFocusMinutes];
            selections[5] = 0; selections[10] = 1; selections[15] = 2;
            OnceRest_Selection.SelectedIndex = selections[(Application.Current as App).OnceRestMinutes];
            selections[30] = 0; selections[60] = 1; selections[120] = 2; selections[180] = 3; selections[240] = 4;
            DailyGoal_Selection.SelectedIndex = selections[(Application.Current as App).DailyGoalMinutes];
            ShowTasksPage_Switch.IsOn = (Application.Current as App).ShowTasksPage;
            ShowRoomPage_Switch.IsOn = (Application.Current as App).ShowRoomPage;
            BgBlur.IsChecked = (Application.Current as App).iUseAcrylicBlur;
            BgReset.IsEnabled = ((Application.Current as App).iUseCustomBackground == true);
        }

        private void Theme_Selection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (Theme_Selection.SelectedIndex == 0)
            {
                (Application.Current as App).RootFrame.RequestedTheme = ElementTheme.Default;
                TitleBar.ButtonForegroundColor = default;
                TitleBar.ButtonHoverForegroundColor = default;
                TitleBar.ButtonPressedForegroundColor = default;
                TitleBar.ButtonInactiveForegroundColor = default;
                TitleBar.ButtonHoverBackgroundColor = default;
                TitleBar.ButtonPressedBackgroundColor = default;
            }
            else if (Theme_Selection.SelectedIndex == 1)
            {
                (Application.Current as App).RootFrame.RequestedTheme = ElementTheme.Light;
                TitleBar.ButtonForegroundColor = Colors.Black;
                TitleBar.ButtonHoverForegroundColor = Colors.Black;
                TitleBar.ButtonPressedForegroundColor = Colors.Black;
                TitleBar.ButtonHoverBackgroundColor = Colors.White;
                TitleBar.ButtonPressedBackgroundColor = Colors.LightGray;
            }
            else if (Theme_Selection.SelectedIndex == 2)
            {
                (Application.Current as App).RootFrame.RequestedTheme = ElementTheme.Dark;
                TitleBar.ButtonForegroundColor = Colors.White;
                TitleBar.ButtonHoverForegroundColor = Colors.White;
                TitleBar.ButtonPressedForegroundColor = Colors.White;
                TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
                TitleBar.ButtonHoverBackgroundColor = Colors.Black;
                TitleBar.ButtonPressedBackgroundColor = Colors.Gray;
            }

            (Application.Current as App).LocalSettings.Values["Theme"] = Theme_Selection.SelectedIndex;
        }

        private void OnceFocus_Selection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int[] selections = new int[8];
            selections[0] = 25; selections[1] = 30; selections[2] = 35;  selections[3] = 40; selections[4] = 45; selections[5] = 50; selections[6] = 55; selections[7] = 60;
            (Application.Current as App).OnceFocusMinutes = selections[OnceFocus_Selection.SelectedIndex];

            (Application.Current as App).LocalSettings.Values["OnceFocusMinutes"] = (Application.Current as App).OnceFocusMinutes;
        }

        private void OnceRest_Selection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int[] selections = new int[3];
            selections[0] = 5; selections[1] = 10; selections[2] = 15;
            (Application.Current as App).OnceRestMinutes = selections[OnceRest_Selection.SelectedIndex];

            (Application.Current as App).LocalSettings.Values["OnceRestMinutes"] = (Application.Current as App).OnceRestMinutes;
        }

        private void DailyGoal_Selection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int[] selections = new int[5];
            selections[0] = 30; selections[1] = 60; selections[2] = 120; selections[3] = 180; selections[4] = 240;
            (Application.Current as App).DailyGoalMinutes = selections[DailyGoal_Selection.SelectedIndex];

            (Application.Current as App).LocalSettings.Values["DailyGoalMinutes"] = (Application.Current as App).DailyGoalMinutes;
        }

        private void ShowTasksPage_Switch_Toggled(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ShowTasksPage = ShowTasksPage_Switch.IsOn;
            ((Window.Current.Content as Frame)?.Content as MainPage).Page_SizeChanged();

            (Application.Current as App).LocalSettings.Values["ShowTasksPage"] = (Application.Current as App).ShowTasksPage;
        }

        private void ShowRoomPage_Switch_Toggled(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ShowRoomPage = ShowRoomPage_Switch.IsOn;
            ((Window.Current.Content as Frame)?.Content as MainPage).Page_SizeChanged();

            (Application.Current as App).LocalSettings.Values["ShowRoomPage"] = (Application.Current as App).ShowRoomPage;
        }

        private async void CustomizeFocusPage_Click(object sender, RoutedEventArgs e)
        {
            isProcessingBg = true;

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
            SaveSoftwareBitmapToFile(SoftwareBitmap, OutputFile);

            BgReset.IsEnabled = true;
            (Application.Current as App).iUseCustomBackground = true;
            (Application.Current as App).LocalSettings.Values["iUseCustomBackground"] = (Application.Current as App).iUseCustomBackground;

            isProcessingBg = false;
        }

        private void BgReset_Click(object sender, RoutedEventArgs e)
        {
            BgReset.IsEnabled = false;
            BgBlur.IsChecked = true;
            BgImage.Source = null;

            (Application.Current as App).iUseCustomBackground = false;
            (Application.Current as App).iUseAcrylicBlur = true;
            (Application.Current as App).LocalSettings.Values["iUseCustomBackground"] = (Application.Current as App).iUseCustomBackground;
            (Application.Current as App).LocalSettings.Values["iUseAcrylicBlur"] = (Application.Current as App).iUseAcrylicBlur;
        }

        private void BgBlur_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).iUseAcrylicBlur = (bool)BgBlur.IsChecked;
            (Application.Current as App).LocalSettings.Values["iUseAcrylicBlur"] = (Application.Current as App).iUseAcrylicBlur;
        }

        bool isProcessingBg = false;
        private async void BgShow_Click(object sender, RoutedEventArgs e)
        {
            if ((Application.Current as App).iUseCustomBackground)
                BgText.Text = "";
            else
                BgText.Text = "默认";
            GetBackground();
        }

        private async void SaveSoftwareBitmapToFile(SoftwareBitmap SoftwareBitmap, StorageFile OutputFile)
        {
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
        }

        private async void GetBackground()
        {
            if ((Application.Current as App).iUseCustomBackground && !isProcessingBg)
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
                            BgImage.Source = bitmapImage;
                        }
                    }
                }
                catch { }
            }
            else
            {
                BgImage.Source = null;
            }
        }
    }
}
