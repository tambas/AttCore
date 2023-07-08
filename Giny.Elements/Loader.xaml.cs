using Giny.Core.Extensions;
using Giny.IO.D2I;
using Giny.IO.D2O;
using Giny.ORM;
using Giny.Rendering.GFX;
using Giny.Rendering.Maps;
using Giny.Rendering.Textures;
using Giny.World.Records.Maps;
using Giny.World.Records.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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

namespace Giny.Elements
{
    /// <summary>
    /// Logique d'interaction pour Loader.xaml
    /// </summary>
    public partial class Loader : UserControl
    {
        public static string ClientPath = @"C:\Users\Skinz\Desktop\Giny\Giny\Raiders\app\";

        public Loader(MainWindow window)
        {
            InitializeComponent();

            DatabaseManager.Instance.Initialize(Assembly.GetAssembly(typeof(SpellRecord)), "localhost",
         "giny_world", "root", "");


            DatabaseManager.Instance.OnTablesLoadProgress += OnLoadProgress;
            DatabaseManager.Instance.OnStartLoadTable += OnStartLoadTable;

            new Thread(new ThreadStart(() =>
            {


                this.Dispatcher.Invoke(() =>
                {
                    loadingLbl.Content = "Loading Textures...";
                    progressBar.IsIndeterminate = true;
                });

                TextureManager.Instance.Initialize(ClientPath);

                this.Dispatcher.Invoke(() =>
                {
                    loadingLbl.Content = "Loading DLM Maps...";
                    progressBar.IsIndeterminate = true;
                });

                MapsManager.Instance.Initialize(ClientPath);

                this.Dispatcher.Invoke(() =>
                {
                    progressBar.IsIndeterminate = false;
                });

                DatabaseManager.Instance.LoadTable<MapPositionRecord>();
                DatabaseManager.Instance.LoadTable<MapRecord>();
                DatabaseManager.Instance.LoadTable<InteractiveSkillRecord>();
                DatabaseManager.Instance.LoadTable<MapScrollActionRecord>();

                window.Dispatcher.Invoke(() =>
                {
                    window.OnLoadingEnd();
                });

            })).Start();
        }


        private void OnStartLoadTable(Type arg1, string arg2)
        {
            this.Dispatcher.Invoke(() =>
            {
                loadingLbl.Content = "Loading " + arg2.FirstCharToUpper() + "...";
            });
        }

        private void OnLoadProgress(string arg1, double arg2)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    double percentage = arg2 * 100d;
                    progressBar.Value = percentage;
                });
            }
            catch
            {
                Environment.Exit(0);
            }
        }
    }
}
