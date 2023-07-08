using Giny.Graphics;
using Giny.Graphics.Gfx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;
using Size = System.Drawing.Size;

namespace Giny.MapEditor
{
    /// <summary>
    /// Logique d'interaction pour TileSelection.xaml
    /// </summary>
    public partial class TileSelection : Window
    {
        public const int TILE_SIZE = 100;
        public const int TILE_PER_LINE = 10;

        public const string TILE_EXTENSION = ".png";

        public bool IsTileSelected
        {
            get
            {
                return selectedTile.Fill is ImageBrush;
            }
        }
        public int SelectedGfxId
        {
            get
            {
                return selectedTile != null && selectedTile.Uid != string.Empty ? int.Parse(System.IO.Path.GetFileNameWithoutExtension(selectedTile.Uid)) : -1;
            }
        }
        public TileSelection()
        {
            InitializeComponent();
            this.Loaded += TileSelection_Loaded;
        }

        private void TileSelection_Loaded(object sender, RoutedEventArgs e)
        {
            Render(GfxManager.GetPngDirectory());
            LoadTilesRepertories(string.Empty);
        }

        private void Render(string path)
        {
            canvas.Children.Clear();
            var files = Directory.GetFiles(path);

            double lineCount = Math.Ceiling(files.Length / (double)TILE_PER_LINE);

            int i = 0;

            canvas.Width = TILE_SIZE * TILE_PER_LINE;
            canvas.Height = TILE_SIZE * lineCount;


            for (int y = 0; y < lineCount; y++)
            {
                for (int x = 0; x < TILE_PER_LINE; x++)
                {
                    if (i >= files.Length)
                        break;

                    double percentage = (double)i / files.Length * 100;

                    Rectangle rect = new Rectangle();
                    rect.Uid = Path.GetFullPath(files[i]);
                    rect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
                    rect.MouseEnter += Rect_MouseEnter;
                    rect.MouseLeave += Rect_MouseLeave;
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    ImageBrush imageBrush = new ImageBrush(GfxManager.GetIcon(int.Parse(System.IO.Path.GetFileNameWithoutExtension(files[i]))));
                    rect.Fill = imageBrush;
                    rect.StrokeThickness = 1;
                    rect.Width = TILE_SIZE;
                    rect.Height = TILE_SIZE;
                    Canvas.SetLeft(rect, x * TILE_SIZE);
                    Canvas.SetTop(rect, y * TILE_SIZE);
                    canvas.Children.Add(rect);
                    i++;
                }
            }

        }

        private void Rect_MouseLeave(object sender, MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            rect.Fill.Opacity = 1f;

        }
        public Brush GetSelectedTileBrush()
        {
            return selectedTile.Fill;
        }
        public string GetSelectedTilePath()
        {
            return selectedTile.Uid;
        }
        private void Rect_MouseEnter(object sender, MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            rect.Fill.Opacity = 0.5f;
        }

        private void Rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            var imageBrush = new ImageBrush(WpfUtils.GetImageSource(rect.Uid));
            selectedTile.Fill = imageBrush;
            selectedTile.Uid = rect.Uid;
            tilename.Content = "Tilename: " + System.IO.Path.GetFileNameWithoutExtension(rect.Uid);
        }

        private void FolderSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (folderSelection.SelectedItem != null)
                Render(System.IO.Path.Combine(GfxManager.GetPngDirectory(), folderSelection.SelectedItem.ToString()));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadTilesRepertories(search.Text);
        }
        private void LoadTilesRepertories(string searchText)
        {
            folderSelection.Items.Clear();

            foreach (var directory in Directory.GetDirectories(GfxManager.GetPngDirectory()))
            {
                string dirName = directory.Split('/').Last();  // bof

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    folderSelection.Items.Add(dirName);
                }
                else
                if (dirName.ToLower().Contains(searchText.ToLower()))
                    folderSelection.Items.Add(dirName);
            }
        }
    }
}
