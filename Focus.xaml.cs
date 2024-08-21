using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TomatoFocus
{
    public sealed partial class Focus : Page
    {
        public Focus()
        {
            this.InitializeComponent();

            if ((Application.Current as App).DefFocusMode == 0)
            {
                TitleDropDown.Content = "倒计时";
            }
            else if((Application.Current as App).DefFocusMode == 1)
            {
                TitleDropDown.Content = "正计时";
            }
            MinuteSet.Text = (Application.Current as App).FocusMinutes.ToString();
            GoalProgressRing.Value = 0;

            TextAlreadyFocused.Text = string.Format("已专注 {0} 分\n目标 {1} 分", (Application.Current as App).AlreadyFocusedMinutes, (Application.Current as App).TargetFocusdMinutes);
            TextPercentFocused.Text = string.Format("{0}%", (int)((Application.Current as App).AlreadyFocusedMinutes * 1.0 / (Application.Current as App).TargetFocusdMinutes * 100));
        }

        private void TitleCountDown_Click(object sender, RoutedEventArgs e)
        {
            TitleDropDown.Content = "倒计时";
            (Application.Current as App).DefFocusMode = 0;
        }

        private void TitleCountUp_Click(object sender, RoutedEventArgs e)
        {
            TitleDropDown.Content = "正计时";
            (Application.Current as App).DefFocusMode = 1;
        }

        private void MinuteUp_Click(object sender, RoutedEventArgs e)
        {
            MinuteSet.Text = ((int)(double.Parse(MinuteSet.Text) + 10)).ToString();
            (Application.Current as App).FocusMinutes = (int)double.Parse(MinuteSet.Text);
        }

        private void MinuteDown_Click(object sender, RoutedEventArgs e)
        {
            if((double.Parse(MinuteSet.Text) - 10) > 0)
            {
                MinuteSet.Text = ((int)(double.Parse(MinuteSet.Text) - 10)).ToString();
                (Application.Current as App).FocusMinutes = (int)double.Parse(MinuteSet.Text);
            }
        }

        private void MinuteSet_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender = null, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args = null)
        {
            if(double.Parse(MinuteSet.Text) <= 0)
            {
                MinuteSet.Text = "1";
            }
            if(double.Parse(MinuteSet.Text) >= 1000)
            {
                MinuteSet.Text = "999";
            }
            if(double.Parse(MinuteSet.Text) != (int)(double.Parse(MinuteSet.Text)))
            {
                MinuteSet.Text = ((int)(double.Parse(MinuteSet.Text))).ToString();
            }
            if (double.Parse(MinuteSet.Text) <= 0)
            {
                MinuteSet.Text = "1";
            }
            (Application.Current as App).FocusMinutes = (int)double.Parse(MinuteSet.Text);
        }

        private void MinuteSet_LostFocus(object sender, RoutedEventArgs e)
        {
            MinuteSet_ValueChanged();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GoalProgressRing_Loaded(object sender = null, RoutedEventArgs e = null)
        {
            GoalProgressRing.Value = ((Application.Current as App).AlreadyFocusedMinutes * 1.0 / (Application.Current as App).TargetFocusdMinutes * 100) + 1;
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
