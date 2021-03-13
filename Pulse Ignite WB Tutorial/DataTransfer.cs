using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Pulse_Ignite_WB_Tutorial
{
    public class DataTransfer
    {
        // The file name
        string fileName = "settings.xml";

        public async void SaveSearchTerm(string SearchTerm, string title, string url, DateTime dateTime)
        {
            var doc = await DocumentLoad().AsAsyncOperation(); // Load the Xml file

            var history = doc.GetElementsByTagName("history");

            XmlElement elSearchTerm = doc.CreateElement("searchterm");
            XmlElement elSiteName = doc.CreateElement("sitename");
            XmlElement elUrl = doc.CreateElement("url");
            XmlElement elDateTime = doc.CreateElement("datetime");

            var historyItem = history[0].AppendChild(doc.CreateElement("historyitem"));

            historyItem.AppendChild(elSearchTerm);
            historyItem.AppendChild(elSiteName);
            historyItem.AppendChild(elUrl);
            historyItem.AppendChild(elDateTime);

            elSearchTerm.InnerText = SearchTerm;
            elSiteName.InnerText = title;
            elUrl.InnerText = url;
            elDateTime.InnerText = dateTime.ToString();

            SaveDoc(doc);
        }

        private async Task<XmlDocument> DocumentLoad()
        {
            XmlDocument result = null;

            await Task.Run(async () =>
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                XmlDocument doc = await XmlDocument.LoadFromFileAsync(file);
                result = doc;
            });

            return result;
        }

        private async void SaveDoc(XmlDocument doc)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            await doc.SaveToFileAsync(file);
        }

        public async Task<List<string>> Fetch(string Source)
        {
            List<string> list = new List<string>();

            await Task.Run(async () =>
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                XmlDocument doc = await XmlDocument.LoadFromFileAsync(file);

                var historyItem = doc.GetElementsByTagName("historyitem");

                for (int i = 0; i < historyItem.Count; i++)
                {
                    var historyItemChild = historyItem[i].ChildNodes;

                    for (int j = 0; j < historyItemChild.Count; j++)
                    {
                        if (historyItemChild[j].NodeName == Source)
                        {
                            list.Add(historyItemChild[j].InnerText);
                        }
                    }
                }
            });

            return list;
        }

        public async void LoadXmlFile()
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            await Launcher.LaunchFileAsync(file);
        }

        public async Task<List<string>> SearchEngineList(string AttributeSource)
        {
            List<string> list = new List<string>();

            await Task.Run(async () =>
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                XmlDocument doc = await XmlDocument.LoadFromFileAsync(file);

                var searchengine = doc.GetElementsByTagName("searchengine");

                var searchChild = searchengine[0].ChildNodes;

                for (int j = 0; j < searchChild.Count; j++)
                {
                    if (searchChild[j].NodeName == "engine")
                    {
                        list.Add(searchChild[j].Attributes.GetNamedItem(AttributeSource).InnerText);
                    }
                }
            });
            return list;
        }

        public async void SetSearchEngine(string EngineName)
        {
            var doc = await DocumentLoad();

            var searchEngine = doc.GetElementsByTagName("searchengine");

            var engines = searchEngine[0].ChildNodes;

            for (int i = 0; i < engines.Count; i++)
            {
                if (engines[i].NodeName == "engine")
                {
                    if (engines[i].Attributes.GetNamedItem("name").InnerText == EngineName)
                    {
                        engines[i].Attributes.GetNamedItem("selected").InnerText = true.ToString();
                    }
                    else
                    {
                        engines[i].Attributes.GetNamedItem("selected").InnerText = false.ToString();
                    }
                }
            }

            SaveDoc(doc);
        }

        public async Task<string> GetSelectedEngineAttribute(string AttributeName)
        {
            string value = string.Empty;

            await Task.Run(async () =>
            {
                var doc = await DocumentLoad();

                var searchEngine = doc.GetElementsByTagName("searchengine");

                var engines = searchEngine[0].ChildNodes;


                for (int i = 0; i < engines.Count; i++)
                {
                    if (engines[i].NodeName == "engine")
                    {
                        if (engines[i].Attributes.GetNamedItem("selected").InnerText == true.ToString())
                        {
                            value = engines[i].Attributes.GetNamedItem(AttributeName).InnerText;
                        }
                    }
                }

            });

            return value;
        }

        public async Task<bool> HasUrlType(string searchString)
        {
            bool result = false;

            await Task.Run(async () =>
            {
                var doc = await DocumentLoad();

                var types = doc.GetElementsByTagName("types");

                var typeChildren = types[0].ChildNodes;

                for (int i = 0; i < typeChildren.Count; i++)
                {
                    if (typeChildren[i].NodeName == "type")
                    {
                        if (searchString.Contains(typeChildren[i].Attributes.GetNamedItem("name").InnerText))
                        {
                            result = true;
                        } 
                    }
                }

            });

            return result;
        }

        public async void SetHome(WebView webView)
        {
            var doc = await DocumentLoad();

            var home = doc.GetElementsByTagName("home");

            home[0].Attributes.GetNamedItem("name").InnerText = webView.DocumentTitle;
            home[0].Attributes.GetNamedItem("url").InnerText = webView.Source.AbsoluteUri;

            SaveDoc(doc);
        }

        public async Task<string> GetHome(string Source)
        {
            string result = string.Empty;

            await Task.Run(async () =>
            {
                var doc = await DocumentLoad();

                var home = doc.GetElementsByTagName("home");

                result = home[0].Attributes.GetNamedItem(Source).InnerText;

            });

            return result;
        }

    }

}
