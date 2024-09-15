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
using System.Collections.ObjectModel;

namespace YourFocus
{
    public class Focused_List
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string Percent { get; set; }
    }

    sealed partial class App : Application
    {
        public ObservableCollection<Focused_List> FocusedList { get; } = new ObservableCollection<Focused_List>();

        public Frame RootFrame;
        public ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public int DefFocusMode = 0;
        public int FocusMinutes = 60;
        public double AlreadyFocusedMinutes = 0;
        public double allAlreadyFocusedMinutes = 0;
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
        public bool ShowRoomPage = false;

        public bool iUseCustomBackground = false;
        public bool iUseAcrylicBlur = true;

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
            GetFocuseHistory();
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

                if (LocalSettings.Values["DefFocusMode"] == null)
                {
                    LocalSettings.Values["DefFocusMode"] = 0;
                }
                else
                {
                    DefFocusMode = (int)LocalSettings.Values["DefFocusMode"];
                }
                if (LocalSettings.Values["DailyGoalMinutes"] == null)
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
                if (LocalSettings.Values["FocusRepeated"] == null)
                {
                    LocalSettings.Values["FocusRepeated"] = false;
                }
                else
                {
                    FocusRepeated = (bool)LocalSettings.Values["FocusRepeated"];
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
                    LocalSettings.Values["ShowRoomPage"] = false;
                }
                else
                {
                    ShowRoomPage = (bool)LocalSettings.Values["ShowRoomPage"];
                }

                if (LocalSettings.Values["iUseCustomBackground"] == null)
                {
                    LocalSettings.Values["iUseCustomBackground"] = false;
                }
                else
                {
                    iUseCustomBackground = (bool)LocalSettings.Values["iUseCustomBackground"];
                }
                if (LocalSettings.Values["iUseAcrylicBlur"] == null)
                {
                    LocalSettings.Values["iUseAcrylicBlur"] = true;
                }
                else
                {
                    iUseAcrylicBlur = (bool)LocalSettings.Values["iUseAcrylicBlur"];
                }

                if (LocalSettings.Values["Timer_IsStart"] != null)
                {
                    (Application.Current as App).Timer_IsStart = (int)(Application.Current as App).LocalSettings.Values["Timer_IsStart"];
                    (Application.Current as App).Timer_StartTick = (long)(Application.Current as App).LocalSettings.Values["Timer_StartTick"];
                    (Application.Current as App).Timer_PauseTick = (long)(Application.Current as App).LocalSettings.Values["Timer_PauseTick"];
                    (Application.Current as App).Timer_TotalTick = (long)(Application.Current as App).LocalSettings.Values["Timer_TotalTick"];
                }
                else
                {
                    (Application.Current as App).LocalSettings.Values["Timer_IsStart"] = (Application.Current as App).Timer_IsStart;
                    (Application.Current as App).LocalSettings.Values["Timer_StartTick"] = (Application.Current as App).Timer_StartTick;
                    (Application.Current as App).LocalSettings.Values["Timer_PauseTick"] = (Application.Current as App).Timer_PauseTick;
                    (Application.Current as App).LocalSettings.Values["Timer_TotalTick"] = (Application.Current as App).Timer_TotalTick;
                }
                if (LocalSettings.Values["StopWatch_IsStart"] != null)
                {
                    (Application.Current as App).StopWatch_IsStart = (int)(Application.Current as App).LocalSettings.Values["StopWatch_IsStart"];
                    (Application.Current as App).StopWatch_StartTick = (long)(Application.Current as App).LocalSettings.Values["StopWatch_StartTick"];
                    (Application.Current as App).StopWatch_PauseTick = (long)(Application.Current as App).LocalSettings.Values["StopWatch_PauseTick"];
                }
                else
                {
                    (Application.Current as App).LocalSettings.Values["StopWatch_IsStart"] = (Application.Current as App).StopWatch_IsStart;
                    (Application.Current as App).LocalSettings.Values["StopWatch_StartTick"] = (Application.Current as App).StopWatch_StartTick;
                    (Application.Current as App).LocalSettings.Values["StopWatch_PauseTick"] = (Application.Current as App).StopWatch_PauseTick;
                }

                string theDate = DateTime.Now.Year.ToString() + "," + DateTime.Now.Month.ToString() + "," + DateTime.Now.Day.ToString();
                Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile file;
                try
                {
                    file = await StorageFolder.GetFileAsync("FocusHistory\\AllFocused.txt");
                    allAlreadyFocusedMinutes = double.Parse((await Windows.Storage.FileIO.ReadLinesAsync(file))[0]);
                    file = await StorageFolder.GetFileAsync("FocusHistory\\" + theDate + ".txt");
                    AlreadyFocusedMinutes = double.Parse((await Windows.Storage.FileIO.ReadLinesAsync(file))[0]);
                }
                catch { }
                file = null;
            }
            catch { }
        }

        public async void GetFocuseHistory()
        {
            Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile file;
            List<string> Dates = new List<string>();
            try
            {
                file = await StorageFolder.GetFileAsync("FocusHistory\\CountFocused.txt");
                int c = 0;
                var FileList = await Windows.Storage.FileIO.ReadLinesAsync(file);
                c = int.Parse(FileList[0]);
                for (int i = 1; i <= c; i++)
                    Dates.Add(FileList[i].ToString());
            }
            catch { }
            FocusedList.Clear();
            foreach (string i in Dates)
            {
                string Time = "";
                string Percent = "";
                try
                {
                    file = await StorageFolder.GetFileAsync("FocusHistory\\" + i + ".txt");
                    var file2 = await Windows.Storage.FileIO.ReadLinesAsync(file);
                    Time = ((int)double.Parse(file2[0])).ToString() + " 分";
                    Percent = ((int)(double.Parse(file2[1]))).ToString() + "%";
                }
                catch { }
                FocusedList.Add(new Focused_List { Date = (i.Split(','))[0] + " 年 " + (i.Split(','))[1] + " 月 " + (i.Split(','))[2] + " 日", Time = Time, Percent = Percent });
            }
        }
    }
}
