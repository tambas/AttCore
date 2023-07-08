using Giny.Core.Extensions;
using Giny.IO.D2I;
using Giny.IO.D2O;
using Giny.IO.D2OClasses;
using Giny.ORM;
using Giny.World.Managers.Fights.Effects;
using Giny.World.Modules;
using Giny.World.Records.Items;
using Giny.World.Records.Maps;
using Giny.World.Records.Npcs;
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

namespace Giny.Npcs
{
    /// <summary>
    /// Logique d'interaction pour Loader.xaml
    /// </summary>
    public partial class Loader : UserControl
    {
        public static D2IFile D2IFile
        {
            get;
            set;
        }
        public Loader(MainWindow window)
        {
            InitializeComponent();

            DatabaseManager.Instance.Initialize(Assembly.GetAssembly(typeof(SpellRecord)), "127.0.0.1",
         "giny_world", "root", "");

            D2OManager.Initialize(@"C:\Users\Skinz\Desktop\Giny\Giny\Raiders\app\data\common");
            D2IFile = new D2IFile(@"C:\Users\Skinz\Desktop\Giny\Giny\Raiders\app\data\i18n\i18n_fr.d2i");


            DatabaseManager.Instance.OnTablesLoadProgress += OnLoadProgress;
            DatabaseManager.Instance.OnStartLoadTable += OnStartLoadTable;

            new Thread(new ThreadStart(() =>
            {
                AssemblyCore.OnAssembliesLoaded();

                DatabaseManager.Instance.LoadTable<InteractiveSkillRecord>();

                DatabaseManager.Instance.LoadTable<NpcRecord>();

                DatabaseManager.Instance.LoadTable<NpcSpawnRecord>();

                DatabaseManager.Instance.LoadTable<NpcReplyRecord>();

                DatabaseManager.Instance.LoadTable<NpcActionRecord>();

                NpcSpawnRecord.Initialize();

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
            this.Dispatcher.Invoke(() =>
            {
                double percentage = arg2 * 100d;
                progressBar.Value = percentage;
            });
        }
    }
}
