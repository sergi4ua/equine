/*Copyright(C) 2018 Sergi4UA

This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see<https://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ComponentModel;
using System.Threading;

namespace EQUINE
{
    public class Downloader
    {
        private List<string> Urls = new List<string>();
        private WebClient webClient = new WebClient();
        private string destFolder;
        public double downloadProgress;
        private int index;
        private bool secondFileDownload;
        public bool IsDone { get; set; }

        public Downloader(List<string> urls, string destFolder)
        {
            this.Urls = urls;
            this.destFolder = destFolder;
        }

        public void BeginDownload()
        {
            Thread dlThread = new Thread(() =>
            {
                Uri dlUri = new Uri(Urls[index]);
                string fileName = System.IO.Path.GetFileName(dlUri.LocalPath);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                webClient.DownloadFileAsync(dlUri, destFolder + "\\" + fileName);
            });
            dlThread.Start();
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if(Urls.Count == 2 && secondFileDownload == false)
            {
                index++;
                secondFileDownload = true;
                BeginDownload();
            }
            this.downloadProgress = 0;

            if(secondFileDownload == true && Urls.Count == 2)
            {
                return;
            }
            else
            {
                IsDone = true;
            }
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            this.downloadProgress = percentage;
        }

        public void Abort()
        {
            webClient.Dispose();
        }
    }
}
