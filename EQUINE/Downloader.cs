using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace EQUINE
{
    public class Downloader
    {
        private List<string> Urls = new List<string>();

        public Downloader(List<string> urls)
        {
            this.Urls = urls;
        }
    }
}
