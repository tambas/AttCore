using Giny.Maps.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Path = System.IO.Path;
using System.Windows.Shapes;
using Giny.Maps.Renderers;
using Giny.Maps.SFML;
using System.IO;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using Giny.Core.Time;
using UserControl = System.Windows.Controls.UserControl;

namespace Giny.Maps.WPF
{
    /// <summary>
    /// Logique d'interaction pour Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        public const string FileExtension = ".dlm";

        public const int TileSize = 100;
        public const int TilesPerLine = 10;

        private const double FramePerSecond = 60d;

        private EditorRenderer EditorRenderer
        {
            get;
            set;
        }

        private string CurrentPath
        {
            get;
            set;
        }
        public Editor()
        {
            InitializeComponent();
            host.Child = new DrawingSurface();
            this.Loaded += OnLoad;
            this.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.Z:
                      
                        break;

                    case Key.W:
                      
                        break;
                    case Key.X:
                        
                        break;
                }
            }

        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            EditorRenderer = new EditorRenderer(host.Child.Handle);

            var timer = new HighPrecisionTimer((int)(1000 / FramePerSecond));
            timer.Tick += Timer_Tick;

            //EditorRenderer.SetDrawingLayer(LayerEnum.Ground);

            LoadTilesRepertories(string.Empty);
        }

        private void Timer_Tick(object sender, HighPrecisionTimer.TickEventArgs e)
        {
            if (this.IsVisible)
            {
                Dispatcher.Invoke(() =>
                {
                    EditorRenderer.Loop();
                });
            }
        }

        private void OpenMapClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = string.Format("{0} files(*.{1}) | *.{1}", FileExtension.ToUpper(), FileExtension);

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                  //  EditorRenderer.LoadMap(ofd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load map. Corrupted file. (" + ex.Message + ")", "Informations", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.CurrentPath = ofd.FileName;
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Save();
        }

        public void Save()
        {
            if (CurrentPath == null)
            {
                SaveAs();
            }
            else
            {
                Save(CurrentPath);
            }
        }
        private void Save(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);
           // MapRecord record = EditorRenderer.Map.GetRecord();
            //record.Serialize(writer);
            stream.Close();
            writer.Close();
            CurrentPath = path;
            MessageBox.Show("Map was saved sucessfully.", "Informations", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        private void SaveAs()
        {
            SaveFileDialog sf = new SaveFileDialog();

            sf.Filter = string.Format("{0} files(*.{1}) | *.{1}", FileExtension, FileExtension);

            if (sf.ShowDialog() == DialogResult.OK)
            {
                Save(sf.FileName);
            }
        }
        private void LoadTilesRepertories(string searchText)
        {
            folderSelection.Items.Clear();

            foreach (var directory in Directory.GetDirectories(GfxManager.GetPath()))
            {
                string folderName = Path.GetFileName(directory);

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    folderSelection.Items.Add(folderName);
                }
                else
                if (folderName.ToLower().Contains(searchText.ToLower()))
                    folderSelection.Items.Add(folderName);
            }
        }
        private void OnSelectTileDirectory(object sender, SelectionChangedEventArgs e)
        {
            if (folderSelection.SelectedItem != null)
            {
                string path = Path.Combine(GfxManager.GetPath(), folderSelection.SelectedItem.ToString());
                RenderTiles(path);
            }
        }

        public void RenderTiles(string path)
        {
            tileSelection.Children.Clear();
            var files = Directory.GetFiles(path);

            double lineCount = Math.Ceiling(files.Length / (double)TilesPerLine);

            int i = 0;

            tileSelection.Width = TileSize * TilesPerLine;
            tileSelection.Height = TileSize * lineCount;


            for (int y = 0; y < lineCount; y++)
            {
                for (int x = 0; x < TilesPerLine; x++)
                {
                    if (i >= files.Length)
                        break;

                    double percentage = (double)i / files.Length * 100;

                    Rectangle rect = new Rectangle();
                    rect.Uid = Path.GetFullPath(files[i]);
                    rect.MouseLeftButtonDown += OnTileClick;
                    rect.MouseEnter += OnMouseEnterTile;
                    rect.MouseLeave += OnMouseLeaveTile;
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    ImageBrush imageBrush = new ImageBrush(GfxManager.GetIcon(System.IO.Path.GetFileNameWithoutExtension(files[i])));
                    rect.Fill = imageBrush;
                    rect.StrokeThickness = 1;
                    rect.Width = TileSize;
                    rect.Height = TileSize;
                    Canvas.SetLeft(rect, x * TileSize);
                    Canvas.SetTop(rect, y * TileSize);
                    tileSelection.Children.Add(rect);
                    i++;
                }
            }
        }

        private void OnTileClick(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            var imageBrush = new ImageBrush(WpfUtils.GetImageSource(rect.Uid));
            string name = Path.GetFileNameWithoutExtension(rect.Uid);
            this.SelectTile(name);
            EditorRenderer.GetWindow().RequestFocus();
            host.Focus();
        }

        private void OnMouseLeaveTile(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            rect.Fill.Opacity = 1f;
        }

        private void OnMouseEnterTile(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            rect.Fill.Opacity = 0.5f;
        }


        private void OnTileSelectionChanged(object sender, MouseButtonEventArgs e)
        {

        }

        private void SaveAsClick(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void CapturePngClick(object sender, RoutedEventArgs e)
        {
           
        }

        private void DisplayGridClicked(object sender, RoutedEventArgs e)
        {
          //  EditorRenderer?.Map.ToogleBorders(displayGrid.IsChecked.Value);
        }


   
        public void SelectTile(string name)
        {
         //   this.SelectedTile?.Dispose();
            //  this.SelectedTile = new SelectedTile(name);
        }
        //public SelectedTile GetSelectedTile()
        // {
        //    return SelectedTile;
        //}
        private void DisplayInfoClick(object sender, RoutedEventArgs e)
        {
          //  EditorRenderer.DisplayUI = displayInfos.IsChecked.Value;
        }
        private void NewMap(int width, int heigth)
        {
            CurrentPath = null;
            // EditorRenderer.NewMap(width, heigth);
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
      

       
      
    }
}
