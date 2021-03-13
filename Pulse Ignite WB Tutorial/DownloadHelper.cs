using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Popups;

namespace Pulse_Ignite_WB_Tutorial
{
    public class DownloadHelper
    {
        public async Task<BackgroundDownloader> _BackgroundDownloader(string FileName, Uri uri, StorageFile OutputLocation)
        {
            BackgroundDownloader downloader = new BackgroundDownloader();
            await Task.Run(async () =>
            {
                try
                {
                    DownloadOperation dO = downloader.CreateDownload(uri, OutputLocation);
                    await dO.StartAsync();
                }
                catch (Exception ex)
                {
                    MessageDialog msg = new MessageDialog(ex.Message);
                    await msg.ShowAsync();
                }
            });

            return downloader;
            
        }
    }
}
