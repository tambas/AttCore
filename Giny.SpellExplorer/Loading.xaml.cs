using Giny.Core.Extensions;
using Giny.Core.Misc;
using Giny.ORM;
using Giny.ORM.IO;
using Giny.World.Managers.Effects;
using Giny.World.Managers.Fights.Effects;
using Giny.World.Modules;
using Giny.World.Records.Breeds;
using Giny.World.Records.Effects;
using Giny.World.Records.Monsters;
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

namespace Giny.SpellExplorer
{
    /// <summary>
    /// Logique d'interaction pour Loading.xaml
    /// </summary>
    public partial class Loading : UserControl
    {
        public Loading(MainWindow window)
        {
            InitializeComponent();

            DatabaseManager.Instance.Initialize(Assembly.GetAssembly(typeof(SpellRecord)), "127.0.0.1",
         "giny_world", "root", "");

            DatabaseManager.Instance.OnStartLoadTable += OnStartLoadTable;

            new Thread(new ThreadStart(() =>
            {
                AssemblyCore.OnAssembliesLoaded();

                UpdateProgressBar(10);

                DatabaseManager.Instance.LoadTable<SpellRecord>();
                UpdateProgressBar(20);
                DatabaseManager.Instance.LoadTable<MonsterRecord>();
                UpdateProgressBar(30);
                DatabaseManager.Instance.LoadTable<EffectRecord>();
                UpdateProgressBar(40);
                DatabaseManager.Instance.LoadTable<SpellStateRecord>();
                UpdateProgressBar(50);
                DatabaseManager.Instance.LoadTable<BreedRecord>();
                SpellEffectManager.Instance.Initialize();

                UpdateProgressBar(60);
                DatabaseManager.Instance.LoadTable<SpellLevelRecord>();

                UpdateProgressBar(100);

                SpellRecord.Initialize();

                window.Dispatcher.Invoke(() =>
                {
                    window.OnLoadingEnd();
                });

            })).Start();


        }

        private void UpdateProgressBar(int percentage)
        {
            this.Dispatcher.Invoke(() =>
            {
                progressBar.Value = percentage;
            });
        }

        private void OnStartLoadTable(Type arg1, string arg2)
        {
            this.Dispatcher.Invoke(() =>
            {
                loadingLbl.Content = "Loading " + arg1.Name + "...";
            });
        }


    }
}
