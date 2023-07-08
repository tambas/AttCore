using Giny.IO.D2O;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DialogResult = System.Windows.Forms.DialogResult;

namespace Giny.D2O
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            d2olist.Items.Clear();

            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && Directory.Exists(dialog.SelectedPath))
            {
                D2OManager.Initialize(dialog.SelectedPath);
            }

            foreach (var filename in D2OManager.GetFilenames())
            {
                d2olist.Items.Add(filename);
            }
        }

        private void D2OListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<object> values = D2OManager.GetObjects(d2olist.SelectedItem.ToString()).ToList();
            datas.ItemsSource = values;
        }
    }
}
