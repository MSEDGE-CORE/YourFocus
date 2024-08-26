using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TomatoFocus
{
    public sealed partial class Stats : Page
    {
        public Stats()
        {
            this.InitializeComponent();
            todayAlreadyFocused.Text = ((int)(Application.Current as App).AlreadyFocusedMinutes).ToString() + " 分";
            allAlreadyFocused.Text = ((int)(Application.Current as App).allAlreadyFocusedMinutes).ToString() + " 分";
            GetHistory();
        }

        public async void GetHistory()
        {
            Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile file;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HistoryBoard.Height = ActualHeight - 180;
        }
    }
}
