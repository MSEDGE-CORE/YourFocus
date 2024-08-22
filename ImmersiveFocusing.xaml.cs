﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections;

namespace TomatoFocus
{
    public sealed partial class ImmersiveFocusing : Page
    {
        DispatcherTimer Timer1;
        int TimeHour = 0, TimeMinute = 0, TimeSecond = 0;
        int nowFoucsStatus = 1; //0down 1up
        int nowAlreadyFocused = 0;

        public ImmersiveFocusing()
        {
            this.InitializeComponent();
            var CoreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            CoreTitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
            Window.Current.SetTitleBar(AppTitleBar);

            FocusRepeatButton.IsChecked = (Application.Current as App).FocusRepeated;
            if ((Application.Current as App).DefFocusMode == 0)
                FocusRepeatButton.IsEnabled = false;

            Timer1 = new DispatcherTimer();
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 5);
            Timer1.Tick += Timer_Tick;
            Timer1.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            if ((Application.Current as App).DefFocusMode == 0 || nowFoucsStatus == 1)//Down
            {
                if ((Application.Current as App).Timer_IsStart == 1)
                {
                    FocusStartButtonIcon.Glyph = "\uF8AE";

                    if (ActualHeight >= TimeDisplay.FontSize + 56 * 2 + 100 && TextH1.FontSize == 50)
                    {
                        OTimerShow.Visibility = Visibility.Visible;
                        ProgressGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        OTimerShow.Visibility = Visibility.Collapsed;
                        ProgressGrid.Visibility = Visibility.Collapsed;
                    }
                }
                else if ((Application.Current as App).Timer_IsStart == 2)
                {
                    FocusStartButtonIcon.Glyph = "\uF5B0";

                    if (ActualHeight >= TimeDisplay.FontSize + 56 * 2 + 100 && TextH1.FontSize == 50)
                    {
                        OTimerShow.Visibility = Visibility.Visible;
                        ProgressGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        OTimerShow.Visibility = Visibility.Collapsed;
                        ProgressGrid.Visibility = Visibility.Collapsed;
                    }
                }
                else if ((Application.Current as App).Timer_IsStart == 0)
                {
                    FocusStartButtonIcon.Glyph = "\uF5B0";

                    OTimerShow.Visibility = Visibility.Collapsed;
                    ProgressGrid.Visibility = Visibility.Collapsed;
                }

                //TimeDisplay
                if ((Application.Current as App).Timer_IsStart == 1)
                {
                    long NowTicks = ((Application.Current as App).Timer_StartTick - DateTime.Now.Ticks / 10000);

                    if (NowTicks <= 0)
                    {
                        NowTicks = 0;
                        if (nowFoucsStatus == 0)
                        {
                            new ToastContentBuilder()
                                .AddArgument("Action", "Timer")
                                .AddText("专注时段完成，休息一下吧")
                                .AddText(DateTime.Now.ToLongTimeString().ToString())
                                .Show();
                        }
                        else if (nowFoucsStatus == 1)
                        {
                            new ToastContentBuilder()
                                .AddArgument("Action", "Timer")
                                .AddText("休息时段结束，继续努力吧")
                                .AddText(DateTime.Now.ToLongTimeString().ToString())
                                .Show();
                        }

                        if (!(Application.Current as App).FocusRepeated && nowFoucsStatus == 1)
                        {
                            ExitFocus();
                            return;
                        }
                        else
                        {
                            PresetFocus();
                        }
                        return;
                    }

                    int Hour = (int)(NowTicks / 1000 / 60 / 60);
                    int Minute = (int)((NowTicks % (1000 * 60 * 60)) / 1000 / 60);
                    int Second = (int)((NowTicks % (1000 * 60)) / 1000);

                    TextHour.Text = Hour.ToString();
                    TextMinute.Text = Minute.ToString();
                    TextSecond.Text = Second.ToString();

                    ProgressBar.Value = (((Application.Current as App).Timer_StartTick - DateTime.Now.Ticks / 10000) * 100.0) / (Application.Current as App).Timer_TotalTick;
                }
                else if ((Application.Current as App).Timer_IsStart == 2)
                {
                    long NowTicks = ((Application.Current as App).Timer_PauseTick);

                    if (NowTicks <= 0)
                    {
                        NowTicks = 0;
                        return;
                    }

                    int Hour = (int)(NowTicks / 1000 / 60 / 60);
                    int Minute = (int)((NowTicks % (1000 * 60 * 60)) / 1000 / 60);
                    int Second = (int)((NowTicks % (1000 * 60)) / 1000);

                    TextHour.Text = Hour.ToString();
                    TextMinute.Text = Minute.ToString();
                    TextSecond.Text = Second.ToString();

                    ProgressBar.Value = ((Application.Current as App).Timer_PauseTick) * 100.0 / (Application.Current as App).Timer_TotalTick;
                }
            }
            else if ((Application.Current as App).DefFocusMode == 1)//Up
            {
                if ((Application.Current as App).StopWatch_IsStart == 1)
                {
                    FocusStartButtonIcon.Glyph = "\uF8AE";
                }
                else if ((Application.Current as App).StopWatch_IsStart == 2)
                {
                    FocusStartButtonIcon.Glyph = "\uF5B0";
                }
                OTimerShow.Visibility = Visibility.Collapsed;
                ProgressGrid.Visibility = Visibility.Collapsed;

                //TimeDisplay
                if ((Application.Current as App).StopWatch_IsStart == 1)
                {
                    long NowTicks = (DateTime.Now.Ticks / 10000 - (Application.Current as App).StopWatch_StartTick);

                    if (NowTicks <= 0)
                    {
                        NowTicks = 0;
                        return;
                    }

                    int Hour = (int)(NowTicks / 1000 / 60 / 60);
                    int Minute = (int)((NowTicks % (1000 * 60 * 60)) / 1000 / 60);
                    int Second = (int)((NowTicks % (1000 * 60)) / 1000);

                    TextHour.Text = Hour.ToString();
                    TextMinute.Text = Minute.ToString();
                    TextSecond.Text = Second.ToString();
                }
                else if ((Application.Current as App).StopWatch_IsStart == 2)
                {
                    long NowTicks = ((Application.Current as App).StopWatch_PauseTick);

                    if (NowTicks <= 0)
                    {
                        NowTicks = 0;
                        return;
                    }

                    int Hour = (int)(NowTicks / 1000 / 60 / 60);
                    int Minute = (int)((NowTicks % (1000 * 60 * 60)) / 1000 / 60);
                    int Second = (int)((NowTicks % (1000 * 60)) / 1000);

                    TextHour.Text = Hour.ToString();
                    TextMinute.Text = Minute.ToString();
                    TextSecond.Text = Second.ToString();
                }
            }
        }

        private void FileFocus()
        {
            (Application.Current as App).LocalSettings.Values["FocusMinutes"] = (Application.Current as App).FocusMinutes;
            (Application.Current as App).LocalSettings.Values["Timer_IsStart"] = (Application.Current as App).Timer_IsStart;
            (Application.Current as App).LocalSettings.Values["Timer_StartTick"] = (Application.Current as App).Timer_StartTick;
            (Application.Current as App).LocalSettings.Values["Timer_PauseTick"] = (Application.Current as App).Timer_PauseTick;
            (Application.Current as App).LocalSettings.Values["Timer_TotalTick"] = (Application.Current as App).Timer_TotalTick;
            (Application.Current as App).LocalSettings.Values["StopWatch_IsStart"] = (Application.Current as App).StopWatch_IsStart;
            (Application.Current as App).LocalSettings.Values["StopWatch_StartTick"] = (Application.Current as App).StopWatch_StartTick;
            (Application.Current as App).LocalSettings.Values["StopWatch_PauseTick"] = (Application.Current as App).StopWatch_PauseTick;
        }

        int toFocusMinutes = 0, allFocusSteps = 0;
        List<int> FocusQ = new List<int>();
        private void PresetCountDownNotRepeated()
        {
            if(allFocusSteps == 0)
            {
                int[] FocusIndex = new int[2] { (Application.Current as App).OnceFocusMinutes, (Application.Current as App).OnceRestMinutes };
                int i = 0, n = (Application.Current as App).FocusMinutes;
                while(n >= FocusIndex[i])
                {
                    FocusQ.Add(FocusIndex[i]);
                    n -= FocusIndex[i];
                    i = 1 - i;
                }
                if(n != 0)
                {
                    FocusQ.Add(n);
                }
                allFocusSteps = FocusQ.Count;
                toFocusMinutes = FocusQ[0];
            }
            else
            {
                nowFoucsStatus = allFocusSteps - FocusQ.Count;
                toFocusMinutes = FocusQ[0];
                FocusQ.Remove(0);
            }
        }
        private void PresetFocus()
        {
            if((Application.Current as App).DefFocusMode == 0 && !(Application.Current as App).FocusRepeated)//
            {
                PresetCountDownNotRepeated();
                TextHour.Text = (toFocusMinutes / 60).ToString();
                TextMinute.Text = (toFocusMinutes % 60).ToString();
                TextSecond.Text = "0";

                (Application.Current as App).Timer_IsStart = 1;
                (Application.Current as App).Timer_StartTick = (long)DateTime.Now.Ticks / 10000 + toFocusMinutes * 60 * 1000 + 1000;
                (Application.Current as App).Timer_TotalTick = toFocusMinutes * 60 * 1000;
            }
            else if(nowFoucsStatus == 1)
            {
                nowFoucsStatus = 0;
                if ((Application.Current as App).DefFocusMode == 0)//Down
                {
                    if ((Application.Current as App).FocusRepeated)
                    {
                        TextHour.Text = ((Application.Current as App).OnceFocusMinutes / 60).ToString();
                        TextMinute.Text = ((Application.Current as App).OnceFocusMinutes % 60).ToString();
                        TextSecond.Text = "0";

                        (Application.Current as App).Timer_IsStart = 1;
                        (Application.Current as App).Timer_StartTick = (long)DateTime.Now.Ticks / 10000 + (Application.Current as App).OnceFocusMinutes * 60 * 1000 + 1000;
                        (Application.Current as App).Timer_TotalTick = (Application.Current as App).OnceFocusMinutes * 60 * 1000;
                    }
                }
                else if ((Application.Current as App).DefFocusMode == 1)//Up
                {
                    TextHour.Text = "0";
                    TextMinute.Text = "0";
                    TextSecond.Text = "0";

                    (Application.Current as App).StopWatch_IsStart = 1;
                    (Application.Current as App).StopWatch_StartTick = (long)DateTime.Now.Ticks / 10000;
                }
            }
            else if(nowFoucsStatus == 0)
            {
                nowFoucsStatus = 1;

                TextHour.Text = ((Application.Current as App).OnceRestMinutes / 60).ToString();
                TextMinute.Text = ((Application.Current as App).OnceRestMinutes % 60).ToString();
                TextSecond.Text = "0";

                (Application.Current as App).Timer_IsStart = 1;
                (Application.Current as App).Timer_StartTick = (long)DateTime.Now.Ticks / 10000 + (Application.Current as App).OnceRestMinutes * 60 * 1000 + 1000;
                (Application.Current as App).Timer_TotalTick = (Application.Current as App).OnceRestMinutes * 60 * 1000;
            }
            FileFocus();
        }

        private void FocusStartButton_Click(object sender, RoutedEventArgs e)
        {
            if ((Application.Current as App).DefFocusMode == 0 || nowFoucsStatus == 1)
            {
                if ((Application.Current as App).Timer_IsStart == 1)
                {
                    (Application.Current as App).Timer_IsStart = 2;
                    (Application.Current as App).Timer_PauseTick = (Application.Current as App).Timer_StartTick - (long)DateTime.Now.Ticks / 10000;
                }
                else if ((Application.Current as App).Timer_IsStart == 2)
                {
                    (Application.Current as App).Timer_IsStart = 1;
                    (Application.Current as App).Timer_StartTick = (long)DateTime.Now.Ticks / 10000 + (Application.Current as App).Timer_PauseTick;
                    (Application.Current as App).Timer_PauseTick = 0;
                }
            }
            else if ((Application.Current as App).DefFocusMode == 1)
            {
                if ((Application.Current as App).StopWatch_IsStart == 1)
                {
                    (Application.Current as App).StopWatch_IsStart = 2;
                    (Application.Current as App).StopWatch_PauseTick = (long)DateTime.Now.Ticks / 10000 - (Application.Current as App).StopWatch_StartTick;
                }
                else if ((Application.Current as App).StopWatch_IsStart == 2)
                {
                    (Application.Current as App).StopWatch_IsStart = 1;
                    (Application.Current as App).StopWatch_StartTick = (long)DateTime.Now.Ticks / 10000 - (Application.Current as App).StopWatch_PauseTick;
                    (Application.Current as App).StopWatch_PauseTick = 0;
                }
            }
            FileFocus();
        }

        private async void FocusStopButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog Dialog = new ContentDialog();
            Dialog.XamlRoot = this.XamlRoot;
            Dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            Dialog.Title = "离开专注？";
            Dialog.Content = "清选择您的操作";
            Dialog.SecondaryButtonText = "离开专注";
            Dialog.CloseButtonText = "取消";
            if ((Application.Current as App).DefFocusMode == 1 || (Application.Current as App).FocusRepeated == true)
            {
                Dialog.PrimaryButtonText = "跳过本次";
                Dialog.DefaultButton = ContentDialogButton.Primary;
            }
            else
            {
                Dialog.DefaultButton = ContentDialogButton.Close;
            }
            var result = await Dialog.ShowAsync();

            if (result != ContentDialogResult.None)
            {
                if(result == ContentDialogResult.Primary)
                {
                    if((Application.Current as App).DefFocusMode == 0)
                    {

                        PresetFocus();
                    }
                    else if((Application.Current as App).DefFocusMode == 1)
                    {

                        PresetFocus();
                    }
                }
                else if(result == ContentDialogResult.Secondary)
                {
                    ExitFocus();
                }
            }
        }

        private void ExitFocus()
        {
            (Application.Current as App).Timer_IsStart = 0;
            (Application.Current as App).Timer_StartTick = 0;
            (Application.Current as App).Timer_PauseTick = 0;
            (Application.Current as App).StopWatch_IsStart = 0;
            (Application.Current as App).StopWatch_StartTick = 0;
            (Application.Current as App).StopWatch_PauseTick = 0;



            FileFocus();
            Timer1.Stop();
            Frame.GoBack();
        }

        private void FocusRepeatButton_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).FocusRepeated = (bool)FocusRepeatButton.IsChecked;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                ConnectedAnimation AFocusControls = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("FocusControls", FocusControls);
                AFocusControls.Configuration = new DirectConnectedAnimationConfiguration();
                ConnectedAnimation AGridTime = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("GridTime", GridTime);
                AGridTime.Configuration = new DirectConnectedAnimationConfiguration();
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var AFocusSetControls = ConnectedAnimationService.GetForCurrentView().GetAnimation("FocusSetControls");
            if (AFocusSetControls != null)
            {
                AFocusSetControls.Configuration = new DirectConnectedAnimationConfiguration();
                AFocusSetControls.TryStart(FocusControls);
            }
            var AGridTime = ConnectedAnimationService.GetForCurrentView().GetAnimation("GridTime");
            if (AGridTime != null)
            {
                Page_SizeChanged();
                PresetFocus();
                AGridTime.Configuration = new DirectConnectedAnimationConfiguration();
                AGridTime.TryStart(GridTime);
            }
        }

        public void SetFontSize()
        {
            if (ActualWidth > 0 && ActualHeight - 60 - 32 > 0)
            {
                if ((ActualHeight - 60 - 32) / 1.5 > (ActualWidth) / 7)
                {
                    TimeDisplay.FontSize = (ActualWidth) * 1.0 / 7;
                }
                else
                {
                    TimeDisplay.FontSize = (ActualHeight - 60 - 32) / 1.5;
                }
                if (!(TimeDisplay.FontSize > 0))
                {
                    TimeDisplay.FontSize = 1;
                }
            }
            if (ActualWidth <= 600 || (ActualHeight - 60 - 32) <= 100)
            {
                TextH1.FontSize = TextH2.FontSize = TextH3.FontSize = 20;
                TextH1.Margin = new Thickness(10, 0, 20, TimeDisplay.FontSize / 2.0 - 10);
                TextH2.Margin = new Thickness(10, 0, 20, TimeDisplay.FontSize / 2.0 - 10);
                TextH3.Margin = new Thickness(10, 0, 0, TimeDisplay.FontSize / 2.0 - 10);
            }
            else
            {
                TextH1.FontSize = TextH2.FontSize = TextH3.FontSize = 50;
                TextH1.Margin = new Thickness(10, 0, 20, TimeDisplay.FontSize / 3.0 - 10);
                TextH2.Margin = new Thickness(10, 0, 20, TimeDisplay.FontSize / 3.0 - 10);
                TextH3.Margin = new Thickness(10, 0, 0, TimeDisplay.FontSize / 3.0 - 10);
            }
            TextHour.FontSize = TimeDisplay.FontSize;
            TextMinute.FontSize = TimeDisplay.FontSize;
            TextSecond.FontSize = TimeDisplay.FontSize;
            GridHour.Height = TimeDisplay.FontSize * 1.2;
            GridMinute.Height = TimeDisplay.FontSize * 1.2;
            GridSecond.Height = TimeDisplay.FontSize * 1.2;
            GridTime.Margin = new Thickness(0, 32 + TimeDisplay.FontSize / 10.0, 0, 60);
        }

        private void Page_SizeChanged(object sender = null, SizeChangedEventArgs e = null)
        {
            SetFontSize();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
