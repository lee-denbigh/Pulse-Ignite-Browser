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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Pulse_Ignite_WB_Tutorial
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {


        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void SettingsNavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            string tag = ((string)selectedItem.Tag);

            if (!args.IsSettingsSelected)
            {
                if (tag == "accountSetMenu")
                {
                    ContentFrame.Navigate(typeof(AccountSettings), null, args.RecommendedNavigationTransitionInfo);
                }
                else if (tag == "bookmarkSetMenu")
                {
                    ContentFrame.Navigate(typeof(BookmarksSettings), null, args.RecommendedNavigationTransitionInfo);
                }
                else if (tag == "historySetMenu")
                {
                    ContentFrame.Navigate(typeof(History), null, args.RecommendedNavigationTransitionInfo);
                }
                else if (tag == "searchSetMenu")
                {
                    ContentFrame.Navigate(typeof(SearchSettings), null, args.RecommendedNavigationTransitionInfo);
                }
                else if(tag == "launchSettingsFile")
                {
                    DataTransfer dt = new DataTransfer();
                    dt.LoadXmlFile();
                }
            }
        }

        private void settingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            SettingsNavView.SelectedItem = SettingsNavView.MenuItems[0];
        }
    }
}
