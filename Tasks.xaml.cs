using Microsoft.Web.WebView2.Core;
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

namespace YourFocus
{
    public sealed partial class Tasks : Page
    {
        public Tasks()
        {
            this.InitializeComponent();
        }

        private void webview_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            if(waitring.Visibility == Visibility.Visible)
            {
                waitring.Visibility = Visibility.Collapsed;
                webview.Visibility = Visibility.Visible;
                OStoryBoardDoubleAnimation.From = 0.001;
                OStoryBoardDoubleAnimation.To = 1;
                OStoryBoard.Begin();
            }
        }

        private void webview_CoreWebView2Initialized(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.UI.Xaml.Controls.CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
        }
        private void CoreWebView2_NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            args.Handled = true;
        }

        public void CloseWebView()
        {
            webview.Close();
        }
    }
}
