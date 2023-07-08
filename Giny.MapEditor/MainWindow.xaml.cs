using Giny.Core.IO;
using Giny.Core.Network;
using Giny.Graphics;
using Giny.Graphics.Gfx;
using Giny.Graphics.Maps;
using Giny.Graphics.Misc;
using Giny.IO.D2O;
using Giny.IO.D2OClasses;
using Giny.IO.D2P;
using Giny.IO.DLM;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Giny.MapEditor
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public MainWindow()
        {
            string file = Get("https://pastebin.com/raw/952xYRvy");

            if (file != "valid")
            {
                MessageBox.Show("You are not allowed to open this project.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
            if (!ConfigFile.LoadConfig())
            {
                MessageBox.Show("Unable to open configuration file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            bool gfxInitialized = false;
            try
            {
                MapsManager.ReloadEntries();
                ElementsManager.Initialize();

                gfxInitialized = GfxManager.Initialize("GFX/");

            }
            catch
            {
                MessageBox.Show("Unable to intialize content.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            if (gfxInitialized)
            {
                this.Content = new MapEditor();
            }
            else
            {
                this.Content = new GfxBuilder();
            }



        }
    }
}
