using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Pulse_Ignite_WB_Tutorial
{
    public class DataAccess
    {
        public async void CreateSettingsFile()
        {
            
            try
            {
                var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("settings.xml");

                using (IRandomAccessStream writeStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    Stream s = writeStream.AsStreamForWrite();
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Async = true;
                    settings.Indent = true;

                    using(XmlWriter writer = XmlWriter.Create(s, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("settings");
                        writer.WriteStartElement("history");
                        writer.WriteEndElement();
                        writer.WriteStartElement("bookmarks");
                        writer.WriteEndElement();
                        writer.WriteStartElement("searchengine");
                        writer.WriteStartElement("google");
                        writer.WriteAttributeString("prefix", "https://www.google.com/search?q=");
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                        writer.Flush();
                        await writer.FlushAsync();
                    }
                }

                await Windows.System.Launcher.LaunchFileAsync(storageFile);
            }
            catch
            {

            }

            
        }

        public async void SaveSearchTerm(string SearchTerm)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync("settings.xml");
            var doc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file);

            var elementHistory = doc.GetElementsByTagName("history");

            Windows.Data.Xml.Dom.XmlElement elem = doc.CreateElement("searchedterm");
            Windows.Data.Xml.Dom.XmlText text = doc.CreateTextNode(SearchTerm);

            elementHistory[0].AppendChild(elem);
            elementHistory[0].LastChild.AppendChild(text);

            await doc.SaveToFileAsync(file);

        }

        


    }
}
