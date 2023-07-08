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
using System.Xml.Serialization;
using Path = System.IO.Path;

namespace Raiders.Uplauncher
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DofusPath = "app/Dofus.exe";

        private const string DofusProcessName = "Dofus";

        private const string CloseButtonPressed = "pack://application:,,,/Images/btn_close_pressed.png";
        private const string CloseButtonNormal = "pack://application:,,,/Images/btn_close_normal.png";
        private const string CloseButtonOver = "pack://application:,,,/Images/btn_close_over.png";

        private Updater Updater
        {
            get;
            set;
        }
        public MainWindow()
        {
            InitializeComponent();
            CloseDofus();
            Config.Initialize();
            this.Loaded += MainWindow_Loaded;
            closeButton.MouseEnter += CloseButton_MouseEnter;
            closeButton.MouseLeave += CloseButton_MouseLeave;
            closeButton.MouseDown += CloseButton_MouseDown;
            closeButton.MouseUp += CloseButton_MouseUp;
            this.background.MouseDown += (s, e) => DragMove();
            Updater = new Updater();
            Updater.PercentChanged += Updater_PercentChanged;
            Updater.UpToDate += Updater_UpToDate;
            Updater.DownloadStarted += Updater_DownloadStarted;
        }

        private void CloseDofus()
        {
            foreach (var process in Process.GetProcessesByName(DofusProcessName))
            {
                process.Kill();
            }
        }
        private void Updater_DownloadStarted(string obj)
        {
            stateLabel.Content = "Téléchargement de la mise a jour ..";
        }

        private void Updater_UpToDate(string version)
        {
            progress.Value = 100;
            stateLabel.Content = "Le jeu est a jour.";
            Config.Instance.CurrentVersion = version;
            Config.Save();
            StartDofus();
        }

        private void StartDofus()
        {
            if (File.Exists(DofusPath))
            {
                Process.Start(Path.Combine(Environment.CurrentDirectory, DofusPath));
            }
            else
            {
                stateLabel.Content = "Impossible de trouver Dofus.exe";
            }
        }
        private void Updater_PercentChanged(int obj)
        {
            stateLabel.Content = "Téléchargement de la mise a jour ... " + obj + "%";
            progress.Value = obj;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Updater.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'effectuer la mise à jour : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                stateLabel.Content = "Impossible d'effectuer la mise a jour.";
            }
        }

        private void CloseButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var image = new BitmapImage(new Uri(CloseButtonPressed));
            closeButton.Source = image;
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            var image = new BitmapImage(new Uri(CloseButtonNormal));
            closeButton.Source = image;
        }

        private void CloseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            var image = new BitmapImage(new Uri(CloseButtonOver));
            closeButton.Source = image;
        }
    }
}
