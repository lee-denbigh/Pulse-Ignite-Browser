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
    public sealed partial class History : Page
    {
        int LBICount = 0;
        int LBISTCount = 0;

        public History()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AddListBoxItems();
        }

        private async void AddListBoxItems()
        {
            DataTransfer dataTransfer = new DataTransfer();
            List<string> historyUrlItems = await dataTransfer.Fetch("url");
            
            foreach (var item in historyUrlItems)
            {
                ListBoxItem newLBI = new ListBoxItem();

                newLBI.Name = "LBI" + LBICount;
                LBICount++;

                Style style = Application.Current.Resources["HistoryList"] as Style;
                newLBI.Style = style;

                newLBI.Content = item;

                listory.Items.Add(newLBI);
            }
        }


    }

    public class URL
    {
        public string Url { get; set; }
    }
}
