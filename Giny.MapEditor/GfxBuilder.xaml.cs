using Giny.Graphics.Gfx;
using Giny.IO.D2P;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace Giny.MapEditor
{
    public partial class GfxBuilder : UserControl
    {
        public const string GFX_PATH = "content/gfx/world";

        public const string GFX0_PATH = "gfx0.d2p";

        public GfxBuilder()
        {
            InitializeComponent();
            this.Loaded += GfxBuilder_Loaded;


        }

        private void GfxBuilder_Loaded(object sender, RoutedEventArgs e)
        {
            ExtractGFX(GfxManager.GfxPath);
        }

        private void ExtractGFX(string path)
        {
            Directory.CreateDirectory(path);


            var file = Path.Combine(ConfigFile.Instance.ClientPath, Path.Combine(GFX_PATH, GFX0_PATH));

            D2PFile d2pFile = new D2PFile(file);
            d2pFile.ExtractPercentProgress += D2pFile_ExtractPercentProgress;
            d2pFile.ExtractAllFiles(path, true, true);
            d2pFile.Dispose();
            d2pFile = null;



            subHeaderText.Text = "Now ordering sprites... step (2/2)";
            progressBar.Value = 0;
            MessageBox.Show("GFX Importations in finished. Now ordering sprites. Press OK to continue.");

            GfxOrderManager.Progress += GfxOrderManager_Progress;


            GfxOrderManager.Order();

            this.Content = new MapEditor();

        }

        private void GfxOrderManager_Progress(int obj)
        {
            progressBar.Dispatcher.Invoke(() => progressBar.Value = obj, DispatcherPriority.Background);
        }

        private void D2pFile_ExtractPercentProgress(D2PFile arg1, int arg2)
        {
            progressBar.Dispatcher.Invoke(() => progressBar.Value = arg2, DispatcherPriority.Background);
        }
    }
}
