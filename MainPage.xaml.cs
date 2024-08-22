using System;
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
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Animation;

namespace TomatoFocus
{
    public sealed partial class MainPage : Page
    {
        public DispatcherTimer Timer1;

        public MainPage()
        {
            this.InitializeComponent();
            var CoreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            CoreTitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
            TitleBar.ButtonBackgroundColor = Colors.Transparent;
            TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            Window.Current.SetTitleBar(AppTitleBar);

            ContentFrame.Navigate(typeof(Focus));
            NavFocusButton.IsChecked = true;
            NavTasksButton.IsChecked = false;
            NavRoomButton.IsChecked = false;
            NavStatsButton.IsChecked = false;
            NavSettingsButton.IsChecked = false;

            Timer1 = new DispatcherTimer();
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 100);
            Timer1.Tick += Timer_Tick;
            Timer1.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            if (AcrylicBoard.Visibility == Visibility.Visible && AcrylicBoard.Opacity == 0)
            {
                AcrylicBoard.Visibility = Visibility.Collapsed;
            }
        }

        void CloseTaskWebView()
        {
            if(ContentFrame.CurrentSourcePageType == typeof(Tasks))
                (ContentFrame.Content as Tasks).CloseWebView();
        }

        private void NavFocusButton_Click(object sender, RoutedEventArgs e)
        {
            CloseTaskWebView();
            if (ContentFrame.CurrentSourcePageType != typeof(Focus))
            {
                ContentFrame.Navigate(typeof(Focus), null, new EntranceNavigationTransitionInfo());
            }
            NavFocusButton.IsChecked = true;
            NavTasksButton.IsChecked = false;
            NavRoomButton.IsChecked = false;
            NavStatsButton.IsChecked = false;
            NavSettingsButton.IsChecked = false;
        }

        private void NavTaskButton_Click(object sender, RoutedEventArgs e)
        {
            CloseTaskWebView();
            if (ContentFrame.CurrentSourcePageType != typeof(Tasks))
            {
                ContentFrame.Navigate(typeof(Tasks), null, new EntranceNavigationTransitionInfo());
            }
            NavFocusButton.IsChecked = false;
            NavTasksButton.IsChecked = true;
            NavRoomButton.IsChecked = false;
            NavStatsButton.IsChecked = false;
            NavSettingsButton.IsChecked = false;
        }

        private void NavRoomButton_Click(object sender, RoutedEventArgs e)
        {
            CloseTaskWebView();
            if (ContentFrame.CurrentSourcePageType != typeof(Room))
            {
                ContentFrame.Navigate(typeof(Room), null, new EntranceNavigationTransitionInfo());
            }
            NavFocusButton.IsChecked = false;
            NavTasksButton.IsChecked = false;
            NavRoomButton.IsChecked = true;
            NavStatsButton.IsChecked = false;
            NavSettingsButton.IsChecked = false;
        }

        private void NavStatsButton_Click(object sender, RoutedEventArgs e)
        {
            CloseTaskWebView();
            if (ContentFrame.CurrentSourcePageType != typeof(Stats))
            {
                ContentFrame.Navigate(typeof(Stats), null, new EntranceNavigationTransitionInfo());
            }
            NavFocusButton.IsChecked = false;
            NavTasksButton.IsChecked = false;
            NavRoomButton.IsChecked = false;
            NavStatsButton.IsChecked = true;
            NavSettingsButton.IsChecked = false;
        }

        public void NavSettingsButton_Click(object sender = null, RoutedEventArgs e = null)
        {
            CloseTaskWebView();
            if (ContentFrame.CurrentSourcePageType != typeof(Settings))
            {
                ContentFrame.Navigate(typeof(Settings), null, new EntranceNavigationTransitionInfo());
            }
            NavFocusButton.IsChecked = false;
            NavTasksButton.IsChecked = false;
            NavRoomButton.IsChecked = false;
            NavStatsButton.IsChecked = false;
            NavSettingsButton.IsChecked = true;
        }

        public void Page_SizeChanged(object sender = null, SizeChangedEventArgs e = null)
        {
            if (ActualWidth >= 680 && ActualHeight >= 344)
            {
                LeftBar.Visibility = Visibility.Visible;
                BottomBar.Visibility = Visibility.Collapsed;
                NavigationButtons.HorizontalAlignment = HorizontalAlignment.Left;
                NavigationButtons.VerticalAlignment = VerticalAlignment.Center;
                NavigationButtons.Orientation = Orientation.Vertical;
                NavigationButtons.Margin = new Thickness(2, 0, 0, 0);
                ContentFrame.Margin = new Thickness(68, 0, 0, 0);
            }
            else
            {
                LeftBar.Visibility = Visibility.Collapsed;
                BottomBar.Visibility = Visibility.Visible;
                NavigationButtons.HorizontalAlignment = HorizontalAlignment.Center;
                NavigationButtons.VerticalAlignment = VerticalAlignment.Bottom;
                NavigationButtons.Orientation = Orientation.Horizontal;
                NavigationButtons.Margin = new Thickness(0, 0, 0, -2);
                ContentFrame.Margin = new Thickness(0, 0, 0, 68);
            }

            if((Application.Current as App).ShowTasksPage)
            {
                NavTasksButton.Visibility = Visibility.Visible;
            }
            else
            {
                NavTasksButton.Visibility = Visibility.Collapsed;
            }
            if ((Application.Current as App).ShowRoomPage)
            {
                NavRoomButton.Visibility = Visibility.Visible;
            }
            else
            {
                NavRoomButton.Visibility = Visibility.Collapsed;
            }
        }

        public void ShowAcrylicBoard(bool isShow = false)
        {
            if(isShow)
            {
                AcrylicBoard.Visibility = Visibility.Visible;
                AcrylicStoryBoardDoubleAnimation.From = 0.001;
                AcrylicStoryBoardDoubleAnimation.To = 1;
                AcrylicStoryBoard.Begin();
            }
            else
            {
                AcrylicStoryBoardDoubleAnimation.From = 1;
                AcrylicStoryBoardDoubleAnimation.To = 0;
                AcrylicStoryBoard.Begin();
            }
        }
    }
}
