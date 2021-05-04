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
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pulse_Ignite_WB_Tutorial
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Search Engine Prefix
        public string prefix = string.Empty;

        // The number of tabs open
        int settingTabCount = 0;

        // Home button Url and Home Name
        string homeUrl = string.Empty, homeName = string.Empty;

        // The current selected Tab
        muxc.TabViewItem currentSelectedTab = null;
        // The current selected Web View
        WebView currentSelectedWebView = null;

        public MainPage()
        {
            // Initialise component.
            this.InitializeComponent();

            // Instance of Data Access class
            DataAccess dataAccess = new DataAccess();

            // Create the settings file function in the data access class.
            dataAccess.CreateSettingsFile();

            // On start navigate the default browser to home that is set in the xml file.
            GetHome();
        }

        /// <summary>
        /// Navigate the webBrowser home.
        /// </summary>
        private async void GetHome()
        {
            // Try navigate home
            try
            {
                DataTransfer dt = new DataTransfer();

                homeName = await dt.GetHomeAttribute("name");
                homeUrl = await dt.GetHomeAttribute("url");
            }
            catch (Exception ex)
            {
                // Show a error message if there is an issue.
                MessageDialog msg = new MessageDialog(ex.Message);
                await msg.ShowAsync();
            }

            // See if the homeUrl and homeName are not null.
            if (!string.IsNullOrEmpty(homeUrl) || !string.IsNullOrEmpty(homeName))
            {
                // Naviaget home if they aren't.
                NavigateHome(); 
            }

            currentSelectedTab = defaultTab;
            currentSelectedWebView = webBrowser;
        }

        /// <summary>
        /// Navigate the current selected browser to the home url and set the current selected
        /// tab header to the home name.
        /// </summary>
        private void NavigateHome()
        {
            if (currentSelectedWebView == null)
            {
                webBrowser.Navigate(new Uri(homeUrl));
                defaultTab.Header = webBrowser.DocumentTitle;
            }
            else
            {
                // Navigate the current selected web view to the home url.
                currentSelectedWebView.Navigate(new Uri(homeUrl));
                // Set the current selected tab header to home name.
                currentSelectedTab.Header = currentSelectedWebView.DocumentTitle;
            }
        }

        /// <summary>
        /// Navigate the default browser to go back.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            // Navigate backwards.
            if (webBrowser.CanGoBack) // <<Fix Needed>>
            {
                webBrowser.GoBack(); 
            }
        }

        private void frdBtn_Click(object sender, RoutedEventArgs e)
        {
            // Navigate forwards.
            if (webBrowser.CanGoForward) // <<Fix Needed>>
            {
                webBrowser.GoForward();
            }
        }

        /// <summary>
        /// When a key is pressed while the search bar has focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // If the key pressed is the enter key
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                // Call the search function.
                Search();
            }
        }

        /// <summary>
        /// Does a search with the search engine selected in settings, or goes to a Url.
        /// </summary>
        private async void Search()
        {
            // Get an instance of the Data Transfer class.
            DataTransfer dt = new DataTransfer();

            // Returns a true or false if the url bar has a host type (set in the xml settings file).
            bool hasUrlType = await dt.HasUrlType(SearchBar.Text);

            // If there is a type the navigate the current selected web view to the destination and adds https to the beginning.
            if (hasUrlType)
            {
                // If the url doesn't contain http or https the add it to the beginning.
                if (!SearchBar.Text.Contains("http://") || !SearchBar.Text.Contains("https://"))
                {
                    currentSelectedWebView.Navigate(new Uri("https://www." + SearchBar.Text));
                }
                else
                {
                    SearchBar.Text = "https://www." + SearchBar.Text;
                }

                // Change the search text to the url.
                SearchBar.Text = currentSelectedWebView.Source.AbsoluteUri;
            }
            else
            {
                // Set the global veriable "prefix" to the selected engine.
                prefix = await dt.GetSelectedEngineAttribute("prefix");

                if (currentSelectedTab != null)
                {
                    // add the prefix if it's not a settings page.
                    if (currentSelectedTab.Name != "settingsTab")
                    {
                        if (currentSelectedWebView == null) // Posible error if no tab, could cause crash. <<Fix Needed>>
                        {
                            // search with the prefix of the selected search engine on the default.
                            webBrowser.Source = new Uri(prefix + SearchBar.Text);
                        }
                        else
                        {
                            // search with the prefix of the selected search engine on the current
                            currentSelectedWebView.Source = new Uri(prefix + SearchBar.Text);
                        }
                    }
                }
                else
                {
                    
                }
            }

            
        }

        /// <summary>
        /// Refresh Web Browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            // Refresh current selected web view.
            webBrowser.Refresh(); // <<Fix Needed>>
        }

        private void settingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(SettingsPage));
            AddSettingsTab();
            settingTabCount++; // Increment tab (settings) count.
        }

        /// <summary>
        /// Adds a tab to the tab control that contains the settings page.
        /// </summary>
        private void AddSettingsTab()
        {
            // New tab
            var settingsTab = new muxc.TabViewItem();
            // Name the header "Settings"
            settingsTab.Header = "Settings";
            // name the tab
            settingsTab.Name = "settingsTab";
            // Change the tab icon.
            settingsTab.IconSource = new muxc.SymbolIconSource() { Symbol = Symbol.Setting };

            // Create a frame instance
            Frame frame = new Frame();

            // Add the frame to the tab
            settingsTab.Content = frame;

            // Navigate the frame to the settings page.
            frame.Navigate(typeof(SettingsPage));

            // Add the tab to the tab control.
            TabControl.TabItems.Add(settingsTab);

            // Set the newly created tab as the selected tab.
            TabControl.SelectedItem = settingsTab;
            
        }

        private void MainBrowserWindow_Loading(FrameworkElement sender, object args)
        {
            
        }

        private void webBrowser_Loading(FrameworkElement sender, object args)
        {
            // Set the status text at the bottom-left of the browser window.
            statusText.Text = webBrowser.Source.AbsoluteUri;
        }


        private void webBrowser_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            // Stop the progress ring from spinning.
            browserProgress.IsActive = false;
            try
            {
                // Change the status text to the url
                statusText.Text = webBrowser.Source.AbsoluteUri;

                // Change the app title at the top left of the window.
                AppTitle.Text = "Pulse Ignite Browser" + " | " + webBrowser.DocumentTitle;

                // Instance of the dataTransfer class
                DataTransfer dataTransfer = new DataTransfer();

                // Check if the search bar doesn't contain text.
                if (!string.IsNullOrEmpty(SearchBar.Text))
                {
                    // if it doesn't then save the search term in history.
                    dataTransfer.SaveSearchTerm(SearchBar.Text, webBrowser.DocumentTitle, webBrowser.Source.AbsoluteUri, DateTime.Now); // Error
                }
            }
            catch
            {

            }

            // Call the check ssl function.
            CheckSSL();

            // check if the status text contains "BlankPage".
            if (!statusText.Text.Contains("BlankPage"))
            {
                if (currentSelectedWebView == null)
                {
                    statusText.Text = webBrowser.Source.AbsoluteUri;
                }
                else
                {
                    // Sets the status text to the current selected web view url
                    statusText.Text = currentSelectedWebView.Source.AbsoluteUri; // Error 
                }
            }
            else
            {
                // Set the staus text to Blank Page
                statusText.Text = "Blank Page";
            }

            // Default header = the default web view doc title.
            defaultTab.Header = webBrowser.DocumentTitle;
        }

        /// <summary>
        /// Do a check to see if the current selected view has a ssl cert.
        /// </summary>
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
            AddNewTab(new Uri(homeUrl));
        }

        private void NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            AddNewTab(args.Uri);
            args.Handled = true;
        }

        private void BrowserNaviagted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            browserProgress.IsActive = false;
            var view = sender as WebView;
            var tab = view.Parent as muxc.TabViewItem;

            if (view != null) // Part 26 Changed
            {
                tab.Header = view.DocumentTitle; 
            }

            if (!statusText.Text.Contains("BlankPage"))
            {
                statusText.Text = currentSelectedWebView.Source.AbsoluteUri; // Error
            }
            else
            {
                statusText.Text = "Blank Page";
            }

            tab.IconSource = new muxc.BitmapIconSource() { UriSource = new Uri(view.Source.Host + "favicon.ico") };
            CheckSSL();
            tab.Header = view.DocumentTitle;
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
            AddNewTab(args.Uri);
            args.Handled = true;
        }

        private void homeBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateHome();
        }

        private void favButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransfer dt = new DataTransfer();
            dt.SaveBookmark(currentSelectedWebView.Source.AbsoluteUri, currentSelectedWebView.DocumentTitle);

            bookmarkFeedbackTip.Subtitle = $"'{currentSelectedWebView.DocumentTitle}' was added to your favourites.";
            bookmarkFeedbackTip.IsOpen = true;
        }

        private void AddNewTab(Uri Url)
        {
            var newTab = new muxc.TabViewItem();
            newTab.IconSource = new muxc.SymbolIconSource() { Symbol = Symbol.Document };
            newTab.Header = "New Tab";

            WebView webView = new WebView();
            webView.IsRightTapEnabled = true;

            newTab.Content = webView;
            webView.Navigate(Url);

            TabControl.TabItems.Add(newTab);

            TabControl.SelectedItem = newTab;

            webView.NavigationCompleted += BrowserNaviagted;
            webView.NewWindowRequested += NewWindowRequested;

            currentSelectedTab = newTab;
            currentSelectedWebView = webView;
        }
    }
}
