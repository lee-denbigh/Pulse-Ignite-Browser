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
    public sealed partial class BookmarksSettings : Page
    {
        public BookmarksSettings()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetBookmarks();
        }

        private async void GetBookmarks()
        {
            BookmarkList_Settings.Items.Clear();
            DataTransfer dt = new DataTransfer();

            List<BookmarkDetails> bookmarkDetails = await dt.GetBookmarkList();

            for (int i = 0; i < bookmarkDetails.Count; i++)
            {
                ListBoxItem item = new ListBoxItem();
                item.Style = Application.Current.Resources["pi_bookmarkSettingsListStyle"] as Style;
                item.DataContext = bookmarkDetails[i];

                BookmarkList_Settings.Items.Add(item);
            }
        }
    }
}
