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
using Windows.UI.Xaml.Media.Animation;
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
            isRepeatButton.IsChecked = (Application.Current as App).FocusRepeated;
            GoalProgressRing.Value = 0;
            RelayCounterFrame();
        }

        private void RelayCounterFrame()
        {
            if((Application.Current as App).DefFocusMode == 0 && !(Application.Current as App).FocusRepeated)
                CountDownMinuteSet.Visibility = Visibility.Visible;
            else if((Application.Current as App).DefFocusMode == 1 || (Application.Current as App).FocusRepeated)
                CountDownMinuteSet.Visibility = Visibility.Collapsed;
        }
        private void TitleCountDown_Click(object sender, RoutedEventArgs e)
        {
            TitleDropDown.Content = "倒计时";
            (Application.Current as App).DefFocusMode = 0;
            RelayCounterFrame();
            (Application.Current as App).LocalSettings.Values["DefFocusMode"] = 0;
        }
        private void TitleCountUp_Click(object sender, RoutedEventArgs e)
        {
            TitleDropDown.Content = "正计时";
            (Application.Current as App).DefFocusMode = 1;
            RelayCounterFrame();
            (Application.Current as App).LocalSettings.Values["DefFocusMode"] = 1;
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
            (Application.Current as App).LocalSettings.Values["FocusMinutes"] = (Application.Current as App).FocusMinutes;
        }

        private void MinuteSet_LostFocus(object sender, RoutedEventArgs e)
        {
            MinuteSet_ValueChanged();
        }

        private void StartButton_Click(object sender = null, RoutedEventArgs e = null)
        {
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("FocusSetControls", FocusSetControls);
            if ((Application.Current as App).DefFocusMode == 0 && !(Application.Current as App).FocusRepeated)
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("GridTime", CountDownMinuteSet);
            else if((Application.Current as App).DefFocusMode == 1 || (Application.Current as App).FocusRepeated)
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("GridTime", CountTransparentTransition);
            ((Window.Current.Content as Frame)?.Content as MainPage).Frame.Navigate(typeof(ImmersiveFocusing),null, new DrillInNavigationTransitionInfo());
        }

        public void GoalProgressRing_Loaded(object sender = null, RoutedEventArgs e = null)
        {
            TextAlreadyFocused.Text = string.Format("已专注 {0} 分\n目标 {1} 分", (int)(Application.Current as App).AlreadyFocusedMinutes, (Application.Current as App).DailyGoalMinutes);
            TextPercentFocused.Text = string.Format("{0}%", (int)((Application.Current as App).AlreadyFocusedMinutes * 1.0 / (Application.Current as App).DailyGoalMinutes * 100));
            GoalProgressRing.Value = ((Application.Current as App).AlreadyFocusedMinutes * 1.0 / (Application.Current as App).DailyGoalMinutes * 100) + 1;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
           ((Window.Current.Content as Frame)?.Content as MainPage).NavSettingsButton_Click();
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).FocusRepeated = (bool)isRepeatButton.IsChecked;
            RelayCounterFrame();
            (Application.Current as App).LocalSettings.Values["FocusRepeated"] = (Application.Current as App).FocusRepeated;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ConnectedAnimation AFocusSetControls = ConnectedAnimationService.GetForCurrentView().GetAnimation("FocusControls");
            if (AFocusSetControls != null)
            {
                AFocusSetControls.Configuration = new DirectConnectedAnimationConfiguration();
                AFocusSetControls.TryStart(FocusSetControls);
            }
            ConnectedAnimation AGridTime = ConnectedAnimationService.GetForCurrentView().GetAnimation("GridTime");
            if (AFocusSetControls != null)
            {
                AGridTime.Configuration = new DirectConnectedAnimationConfiguration();
                if ((Application.Current as App).DefFocusMode == 0 && !(Application.Current as App).FocusRepeated)
                    AGridTime.TryStart(CountDownMinuteSet);
                else if ((Application.Current as App).DefFocusMode == 1 || (Application.Current as App).FocusRepeated)
                    AGridTime.TryStart(CountTransparentTransition);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if ((Application.Current as App).StopWatch_IsStart != 0 || (Application.Current as App).Timer_IsStart != 0)
            {
                StartButton_Click();
            }
        }
    }
}
