using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Raiders.Uplauncher
{
    public class Updater
    {
        private const string VersionUrl = "https://raw.githubusercontent.com/SlaveMaster123/i18n/master/version.txt";

        private const string PackageUrl = "https://raw.githubusercontent.com/SlaveMaster123/i18n/master/i18n_fr.d2i";

        private const string TargetPath = "app/data/i18n/i18n_fr.d2i";

        public event Action<int> PercentChanged;

        public event Action<string> UpToDate;

        public event Action<string> DownloadStarted;

        private string ServerVersion;

        public Updater()
        {

        }

        public void Update()
        {
            ServerVersion = HttpGet(VersionUrl);

            string currentVersion = Config.Instance.CurrentVersion;

            if (ServerVersion != currentVersion)
            {
                DownloadServerVersion();
            }
            else
            {
                UpToDate?.Invoke(ServerVersion);
                return;
            }



        }

        private void DownloadServerVersion()
        {
            DownloadStarted?.Invoke(ServerVersion);
            WebClient client = new WebClient();
            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            client.DownloadFileCompleted += Client_DownloadFileCompleted;
            client.DownloadFileAsync(new Uri(PackageUrl), TargetPath);
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            PercentChanged?.Invoke(e.ProgressPercentage);
        }
        
        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            UpToDate?.Invoke(ServerVersion);
        }

        private static string HttpGet(string url)
        {
            string result = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}
