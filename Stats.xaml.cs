using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
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
            ListFocus.ItemsSource = (Application.Current as App).FocusedList;
            RefreshTodayFocus();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HistoryBoard.Height = ActualHeight - 150;
        }

        private async void RefreshTodayFocus()
        {
            string theDate = DateTime.Now.Year.ToString() + " 年 " + DateTime.Now.Month.ToString() + " 月 " + DateTime.Now.Day.ToString() + " 日";
            if ((Application.Current as App).FocusedList[0].Date == theDate)
            {
                (Application.Current as App).FocusedList[0].Time = ((int)(Application.Current as App).AlreadyFocusedMinutes).ToString() + " 分";
                (Application.Current as App).FocusedList[0].Percent = ((int)((Application.Current as App).AlreadyFocusedMinutes * 100 / (Application.Current as App).DailyGoalMinutes)).ToString() + "%";
            }
        }
    }
}
