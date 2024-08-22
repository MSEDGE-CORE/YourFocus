using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
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
    sealed partial class App : Application
    {
        public Frame RootFrame;
        public ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public int DefFocusMode = 0;
        public int FocusMinutes = 60;
        public int AlreadyFocusedMinutes = 0;
        public int DailyGoalMinutes = 60;
        public int OnceFocusMinutes = 25;
        public int OnceRestMinutes = 5;
        public bool FocusRepeated = false;

        public int StopWatch_IsStart = 0;
        public long StopWatch_StartTick = 0;
        public long StopWatch_PauseTick = 0;

        public int Timer_IsStart = 0;
        public long Timer_StartTick = 0;
        public long Timer_PauseTick = 0;
        public long Timer_TotalTick = 0;

        public bool ShowTasksPage = true;
        public bool ShowRoomPage = true;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            RootFrame = Window.Current.Content as Frame;

            if (RootFrame == null)
            {
                RootFrame = new Frame();
                RootFrame.NavigationFailed += OnNavigationFailed;
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {

                }
                Window.Current.Content = RootFrame;
            }
            GetSettings();
            if (e.PrelaunchActivated == false)
            {
                if (RootFrame.Content == null)
                {
                    RootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                Window.Current.Activate();
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        public async void GetSettings()
        {
            try
            {
                ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (LocalSettings.Values["Theme"] == null || (int)LocalSettings.Values["Theme"] == 0)
                {
                    (Application.Current as App).RootFrame.RequestedTheme = ElementTheme.Default;
                    TitleBar.ButtonForegroundColor = default;
                    TitleBar.ButtonHoverForegroundColor = default;
                    TitleBar.ButtonPressedForegroundColor = default;
                    TitleBar.ButtonInactiveForegroundColor = default;
                    TitleBar.ButtonHoverBackgroundColor = default;
                    TitleBar.ButtonPressedBackgroundColor = default;
                    LocalSettings.Values["Theme"] = 0;
                }
                else if ((int)LocalSettings.Values["Theme"] == 1)
                {
                    (Application.Current as App).RootFrame.RequestedTheme = ElementTheme.Light;
                    TitleBar.ButtonForegroundColor = Colors.Black;
                    TitleBar.ButtonHoverForegroundColor = Colors.Black;
                    TitleBar.ButtonPressedForegroundColor = Colors.Black;
                    TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                    TitleBar.ButtonHoverBackgroundColor = Colors.White;
                    TitleBar.ButtonPressedBackgroundColor = Colors.LightGray;
                }
                else if ((int)LocalSettings.Values["Theme"] == 2)
                {
                    (Application.Current as App).RootFrame.RequestedTheme = ElementTheme.Dark;
                    TitleBar.ButtonForegroundColor = Colors.White;
                    TitleBar.ButtonHoverForegroundColor = Colors.White;
                    TitleBar.ButtonPressedForegroundColor = Colors.White;
                    TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
                    TitleBar.ButtonHoverBackgroundColor = Colors.Black;
                    TitleBar.ButtonPressedBackgroundColor = Colors.Gray;
                }

                if(LocalSettings.Values["DailyGoalMinutes"] == null)
                {
                    LocalSettings.Values["DailyGoalMinutes"] = 60;
                }
                else
                {
                    DailyGoalMinutes = (int)LocalSettings.Values["DailyGoalMinutes"];
                }
                if (LocalSettings.Values["OnceFocusMinutes"] == null)
                {
                    LocalSettings.Values["OnceFocusMinutes"] = 25;
                }
                else
                {
                    OnceFocusMinutes = (int)LocalSettings.Values["OnceFocusMinutes"];
                }
                if (LocalSettings.Values["OnceRestMinutes"] == null)
                {
                    LocalSettings.Values["OnceRestMinutes"] = 25;
                }
                else
                {
                    OnceRestMinutes = (int)LocalSettings.Values["OnceRestMinutes"];
                }
                if (LocalSettings.Values["FocusMinutes"] == null)
                {
                    LocalSettings.Values["FocusMinutes"] = 60;
                }
                else
                {
                    FocusMinutes = (int)LocalSettings.Values["FocusMinutes"];
                }
                if (LocalSettings.Values["ShowTasksPage"] == null)
                {
                    LocalSettings.Values["ShowTasksPage"] = true;
                }
                else
                {
                    ShowTasksPage = (bool)LocalSettings.Values["ShowTasksPage"];
                }
                if (LocalSettings.Values["ShowRoomPage"] == null)
                {
                    LocalSettings.Values["ShowRoomPage"] = true;
                }
                else
                {
                    ShowRoomPage = (bool)LocalSettings.Values["ShowRoomPage"];
                }
            }
            catch { }
        }
     }
}
