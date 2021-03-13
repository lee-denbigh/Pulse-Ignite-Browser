using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;
using static Windows.Web.Http.HttpClient;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pulse_Ignite_WB_Tutorial
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public string prefix = string.Empty;
        int settingTabCount = 0;
        string homePage = "";
        string homeTitle = "";

        muxc.TabViewItem currentSelectedTab = null;
        WebView currentSelectedWebView = null;

        public MainPage()
        {
            this.InitializeComponent();

            DataAccess dataAccess = new DataAccess();

            dataAccess.CreateSettingsFile();

            GetHome();
        }

        private async void GetHome()
        {
            DataTransfer dt = new DataTransfer();
            homePage = await dt.GetHome("url");
            homeTitle = await dt.GetHome("name");

            NavigateHome();
        }

        private void NavigateHome()
        {
            webBrowser.Navigate(new Uri(homePage));
            defaultTab.Header = homeTitle;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            if (webBrowser.CanGoBack)
            {
                webBrowser.GoBack(); 
            }
        }

        private void frdBtn_Click(object sender, RoutedEventArgs e)
        {
            if (webBrowser.CanGoForward)
            {
                webBrowser.GoForward();
            }
        }

        private void SearchBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                Search();
            }
        }

        private async void Search()
        {
            DataTransfer dt = new DataTransfer();

            bool hasUrlType = await dt.HasUrlType(SearchBar.Text);

            if (hasUrlType)
            {
                if (!SearchBar.Text.Contains("http://") || !SearchBar.Text.Contains("https://"))
                {
                    currentSelectedWebView.Navigate(new Uri("https://www." + SearchBar.Text));
                }
                else
                {
                    SearchBar.Text = "https://www." + SearchBar.Text;
                }

                SearchBar.Text = currentSelectedWebView.Source.AbsoluteUri;
            }
            else
            {
                
                prefix = await dt.GetSelectedEngineAttribute("prefix");

                if (currentSelectedTab.Name != "settingsTab")
                {
                    if (currentSelectedWebView == null)
                    {
                        webBrowser.Source = new Uri(prefix + SearchBar.Text);
                    }
                    else
                    {
                        currentSelectedWebView.Source = new Uri(prefix + SearchBar.Text);
                    }
                }
            }

            
        }

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            webBrowser.Refresh();
        }

        private void settingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(SettingsPage));
            AddSettingsTab();
            settingTabCount++;
        }

        private void AddSettingsTab()
        {
                var settingsTab = new muxc.TabViewItem();
                settingsTab.Header = "Settings";
                settingsTab.Name = "settingsTab";
                settingsTab.IconSource = new muxc.SymbolIconSource() { Symbol = Symbol.Setting };

                Frame frame = new Frame();

                settingsTab.Content = frame;

                frame.Navigate(typeof(SettingsPage));

                TabControl.TabItems.Add(settingsTab);

                TabControl.SelectedItem = settingsTab;
            
        }

        private void MainBrowserWindow_Loading(FrameworkElement sender, object args)
        {
            
        }

        private void webBrowser_Loading(FrameworkElement sender, object args)
        {
            statusText.Text = webBrowser.Source.AbsoluteUri;
        }


        private void webBrowser_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            browserProgress.IsActive = false;
            try
            {
                statusText.Text = webBrowser.Source.AbsoluteUri;

                AppTitle.Text = "Pulse Ignite Browser" + " | " + webBrowser.DocumentTitle;

                DataTransfer dataTransfer = new DataTransfer();

                if (!string.IsNullOrEmpty(SearchBar.Text))
                {
                    dataTransfer.SaveSearchTerm(SearchBar.Text, webBrowser.DocumentTitle, webBrowser.Source.AbsoluteUri, DateTime.Now); 
                }
            }
            catch
            {

            }

            CheckSSL();

            if (!statusText.Text.Contains("BlankPage"))
            {
                statusText.Text = currentSelectedWebView.Source.AbsoluteUri; // Error
            }
            else
            {
                statusText.Text = "Blank Page";
            }
        }

        private void CheckSSL()
        {
            if (currentSelectedWebView != null)
            {
                if (currentSelectedWebView.Source.AbsoluteUri.Contains("https"))
                {
                    // Change icon image
                    sslIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    sslIcon.Glyph = "\xE72E";

                    ToolTip toolTip = new ToolTip();
                    toolTip.Content = "This website has a SSL certificate.";
                    ToolTipService.SetToolTip(sslButton, toolTip);
                }
                else
                {
                    // Change icon image.
                    sslIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    sslIcon.Glyph = "\xE785";

                    ToolTip toolTip = new ToolTip();
                    toolTip.Content = "This website is unsafe and doesn't have a SSL certificate.";
                    ToolTipService.SetToolTip(sslButton, toolTip);
                }
            }
            else
            {
                if (webBrowser.Source.AbsoluteUri.Contains("https"))
                {
                    // Change icon image
                    sslIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    sslIcon.Glyph = "\xE72E";

                    ToolTip toolTip = new ToolTip();
                    toolTip.Content = "This website has a SSL certificate.";
                    ToolTipService.SetToolTip(sslButton, toolTip);
                }
                else
                {
                    // Change icon image.
                    sslIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    sslIcon.Glyph = "\xE785";

                    ToolTip toolTip = new ToolTip();
                    toolTip.Content = "This website is unsafe and doesn't have a SSL certificate.";
                    ToolTipService.SetToolTip(sslButton, toolTip);
                }
            }
        }

        private void webBrowser_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            browserProgress.IsActive = true;
        }

        private void TabControl_AddTabButtonClick(muxc.TabView sender, object args)
        {
            var newTab = new muxc.TabViewItem();
            newTab.IconSource = new muxc.SymbolIconSource() { Symbol = Symbol.Document };
            newTab.Header = homeTitle;

            WebView webView = new WebView();
            webView.IsRightTapEnabled = true;

            string path = homePage;
            //string path = $"ms-appx-web:///Assets/BlankPage.html";

            newTab.Content = webView;
            webView.Navigate(new Uri(path));

            sender.TabItems.Add(newTab);

            sender.SelectedItem = newTab;

            webView.NavigationCompleted += BrowserNaviagted;
            SearchBar.Focus(FocusState.Pointer);
        }

        private void BrowserNaviagted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            var view = sender as WebView;
            var tab = view.Parent as muxc.TabViewItem;

            tab.Header = view.DocumentTitle;

            if (!statusText.Text.Contains("BlankPage"))
            {
                statusText.Text = currentSelectedWebView.Source.AbsoluteUri; // Error
            }
            else
            {
                statusText.Text = "Blank Page";
            }

            tab.IconSource = new muxc.BitmapIconSource() { UriSource = new Uri(view.Source.Host + "/favicon.ico") };
            CheckSSL();
        }

        private void TabControl_TabCloseRequested(muxc.TabView sender, muxc.TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
            currentSelectedTab = null;
            currentSelectedWebView = null;

            if (args.Tab.Name == "settingsTab")
            {
                settingTabCount = 0;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Search
            Search();
        }


        private void SearchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentSelectedTab = TabControl.SelectedItem as muxc.TabViewItem;
            if (currentSelectedTab != null)
            {
                currentSelectedWebView = currentSelectedTab.Content as WebView; 
            }

            if (currentSelectedWebView != null)
            {
                AppTitle.Text = "Pulse Ignite Browser | " + currentSelectedWebView.DocumentTitle; 
            }

            if (!statusText.Text.Contains("BlankPage") && currentSelectedWebView != null)
            {
                statusText.Text = currentSelectedWebView.Source.AbsoluteUri;
            }
            else
            {
                statusText.Text = "Blank Page";
            }
        }

        private async void MainBrowserWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTransfer dt = new DataTransfer();

                string searchEngineName = await dt.GetSelectedEngineAttribute("name");
                prefix = await dt.GetSelectedEngineAttribute("prefix");

                SearchBar.PlaceholderText = "Search With " + searchEngineName + "...";
            }
            catch
            {

            }
        }

        private void webBrowser_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            DownloadHelper dlHelper = new DownloadHelper();


            if (args.Uri != null && args.Uri.OriginalString.ToLower().Contains(".zip")
                || args.Uri.OriginalString.ToLower().Contains(".exe")
                || args.Uri.OriginalString.ToLower().Contains(".mp4")
                || args.Uri.OriginalString.ToLower().Contains(".mp3"))
            { 

            }
        }

        private void homeBtn_Click(object sender, RoutedEventArgs e)
        {
            currentSelectedWebView.Navigate(new Uri(homePage));
            currentSelectedTab.Header = homeTitle;
            SearchBar.Text = string.Empty;
        }
    }
}
