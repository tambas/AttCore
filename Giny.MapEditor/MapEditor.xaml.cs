using Giny.Core.IO;
using Giny.Graphics;
using Giny.Graphics.Gfx;
using Giny.Graphics.Maps;
using Giny.Graphics.Misc;
using Giny.IO.DLM;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;

namespace Giny.MapEditor
{
    /// <summary>
    /// Logique d'interaction pour MapEditor.xaml
    /// </summary>
    public partial class MapEditor : UserControl
    {
        public static Color NOT_WALKABLE_COLOR = Color.FromArgb(120, 255, 0, 0);

        public Map Map
        {
            get;
            set;
        }
        public Rectangle TilePreview
        {
            get;
            set;
        }
        private TileSelection TileSelection
        {
            get;
            set;
        }
        private LayerEnum DrawingLayer
        {
            get
            {
                return (LayerEnum)drawingLayer.SelectedValue;
            }
        }
        public bool EditCollision
        {
            get
            {
                return editCollisions.IsChecked.Value;
            }
        }
        public bool TileIsSelected
        {
            get
            {
                return TileSelection != null && TileSelection.SelectedGfxId != -1;
            }
        }
        private new ContextMenu ContextMenu
        {
            get;
            set;
        }
        private Point PixelOffset;

        public short SubareaId
        {
            get;
            set;
        }

        public MapEditor()
        {
            InitializeComponent();
            this.KeyDown += MapEditor_KeyDown;
            ContextMenu = CreateContextMenu();
            foreach (LayerEnum value in Enum.GetValues(typeof(LayerEnum)))
            {
                drawingLayer.Items.Add(value);
            }
            drawingLayer.SelectedValue = LayerEnum.LAYER_GROUND;

            NewMap();
        }

        private void OpenMapClick(object sender, RoutedEventArgs e)
        {
            int id = 0;

            if (int.TryParse(mapIdtb.Text, out id))
            {
                LoadMap(id);
            }
            else
            {
                MessageBox.Show("Unable to load map.. MapId must be a number.", "Informations", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }
        private void LoadMap(int id)
        {
            var map = MapsManager.LoadDlmMap(id);

            if (map == null)
            {
                MessageBox.Show("Unable to load map.. This map do not exists in D2P.", "Informations", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }
            Map = Map.FromDLM(canvas, map, ElementsManager.GetElements());
            mapIdtb.Text = id.ToString();
            OnMapLoaded();
        }
        private LayerEnum[] ComputeDisplayedLayer()
        {
            List<LayerEnum> result = new List<LayerEnum>();

            if (ground.IsChecked.Value)
                result.Add(LayerEnum.LAYER_GROUND);
            if (additionalGround.IsChecked.Value)
                result.Add(LayerEnum.LAYER_ADDITIONAL_GROUND);
            if (decor.IsChecked.Value)
                result.Add(LayerEnum.LAYER_DECOR);
            if (additionalDecor.IsChecked.Value)
                result.Add(LayerEnum.LAYER_ADDITIONAL_DECOR);
            return result.ToArray();
        }
        private void OnMapLoaded()
        {
            if (Map != null)
            {
                Map.MouseLeave -= Map_MouseLeave;
                Map.MouseEnter -= Map_MouseEnter;
                Map.MouseClick -= Map_MouseClick;
            }
            canvas.Children.Clear();
            TilePreview = new Rectangle();

            canvas.Children.Add(TilePreview);


            Map.Draw(ComputeDisplayedLayer());


            this.DataContext = Map;
            if (displayGrid.IsChecked.Value == false)
            {
                Map.ToogleGrid(false);
            }
            if (editCollisions.IsChecked.Value == true)
            {
                OnEditCollisionValueChanged();
            }

            backgroundColor.SelectedColor = Color.FromArgb(byte.MaxValue, Map.BackgroundColor.R, Map.BackgroundColor.G, Map.BackgroundColor.B);
            canvas.Background = new SolidColorBrush(backgroundColor.SelectedColor.Value);


            scrollViewer.ScrollToVerticalOffset(1113);
            scrollViewer.ScrollToHorizontalOffset(625);

            Map.MouseLeave += Map_MouseLeave;
            Map.MouseEnter += Map_MouseEnter;
            Map.MouseClick += Map_MouseClick;
        }
        private void PreviewTile(Cell cell)
        {
            if (TileSelection == null)
            {
                return;
            }
            var imageSource = GfxManager.GetImageSource(TileSelection.SelectedGfxId);
            TilePreview.Width = imageSource.Width;
            TilePreview.Height = imageSource.Height;
            TilePreview.Fill = new ImageBrush(imageSource);
            TilePreview.DataContext = cell;
            TilePreview.Opacity = 0.7d;
            var pxOffset = ElementsManager.ComputePixelOffset(TileSelection.SelectedGfxId, new Point());
            pxOffset.X += PixelOffset.X;
            pxOffset.Y += PixelOffset.Y;
            var position = ElementGraphical.ComputePosition(cell.Center, new Point(), pxOffset, 0);
            Canvas.SetZIndex(TilePreview, int.MaxValue);
            Canvas.SetLeft(TilePreview, position.X);
            Canvas.SetTop(TilePreview, position.Y);
        }
        private void Map_MouseEnter(Cell cell)
        {

            if (TileIsSelected && !EditCollision)
            {
                PreviewTile(cell);
            }
            else
            {
                TilePreview.Fill = Brushes.Transparent;
            }

            cell.SetThickness(5f);
            cell.Stroke(Colors.CornflowerBlue);

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {

                Click(cell, MouseButtonEnum.Left);
            }
            else if (Mouse.RightButton == MouseButtonState.Pressed)
            {

                Click(cell, MouseButtonEnum.Right);

            }

            if (EditCollision)
            {
                return;
            }

            cell.Fill(Color.FromArgb(120, 255, 255, 255));
        }

        private void Map_MouseLeave(Cell obj)
        {
            obj.SetThickness(1f);
            obj.Unstroke();
            if (EditCollision)
            {
                return;
            }
            obj.Fill(Colors.Transparent);
        }
        private void Click(Cell cell, MouseButtonEnum button)
        {
            if (button == MouseButtonEnum.Left)
            {
                if (EditCollision)
                {
                    cell.Fill(NOT_WALKABLE_COLOR);
                    cell.Data.Mov = false;
                    return;
                }

                if (TileSelection == null || TileSelection.SelectedGfxId == -1)
                {
                    return;
                }

                var element = ElementsManager.GetElementId(TileSelection.SelectedGfxId);

                if (element != null)
                {
                    Point pixelOffset = ElementsManager.ComputePixelOffset(element.Gfx, new Point(element.OriginX, element.OriginY));
                    pixelOffset.X += PixelOffset.X;
                    pixelOffset.Y += PixelOffset.Y;
                    cell.AddElement(element.Id, DrawingLayer, element.Gfx, new Point(element.OriginX, element.OriginY), pixelOffset, element.HorizontalSymmetry);
                    cell.Refresh(ComputeDisplayedLayer());
                }
                else
                {
                    MessageBox.Show("Unable to find element without horizontal symmetry.");
                }
            }
            else
            {
                if (EditCollision)
                {
                    cell.Unfill();
                    cell.Data.Mov = true;
                    return;
                }


                cell.RemoveElement(DrawingLayer);
                cell.Refresh(ComputeDisplayedLayer());
            }
        }

        private void Map_MouseClick(Cell arg1, Graphics.Misc.MouseButtonEnum arg2)
        {
            if (arg2 == MouseButtonEnum.Right && Keyboard.IsKeyDown(Key.LeftShift))
            {
                ContextMenu.PlacementTarget = arg1.GetPolygon();
                ContextMenu.DataContext = arg1;
                ContextMenu.IsOpen = true;
            }
            else
                Click(arg1, arg2);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DisplayTileSelection();
        }
        private void DisplayTileSelection()
        {
            TileSelection?.Close();
            this.TileSelection = new TileSelection();
            TileSelection.Show();
            TileSelection.Closed += TileSelection_Closed;
        }

        private void TileSelection_Closed(object sender, EventArgs e)
        {
            TileSelection = null;
        }

        private void NewMapClick(object sender, RoutedEventArgs e)
        {
            NewMap();
        }

        private void NewMap()
        {
            canvas.Children.Clear();
            Map = new Map(canvas, ConfigFile.Instance.DefaultDlmVersion);
            OnMapLoaded();
        }
        private void FillClick(object sender, RoutedEventArgs e)
        {
            if (DrawingLayer == LayerEnum.LAYER_GROUND && TileSelection != null && Map != null)
            {
                foreach (var cell in Map.Cells)
                {
                    var element = ElementsManager.GetElementId(TileSelection.SelectedGfxId);
                    cell.AddElement(element.Id, LayerEnum.LAYER_GROUND, element.Gfx, new Point(element.OriginX, element.OriginY), new Point(), element.HorizontalSymmetry);
                    cell.Refresh(ComputeDisplayedLayer());
                }
            }
        }

        private void OnToogleGridClicked(object sender, RoutedEventArgs e)
        {
            if (Map == null)
                return;

            if (displayGrid.IsChecked.Value)
            {
                Map.ToogleGrid(true);
            }
            else
            {
                Map.ToogleGrid(false);
            }
        }

        private void ExportClick(object sender, RoutedEventArgs e)
        {
            if (Map == null)
                return;


            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "DLM files (*.dlm) | *.dlm";
            var result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                BigEndianWriter writer = new BigEndianWriter();

                var map = Map.ToDLM();

                map.Serialize(writer);

                File.WriteAllBytes(saveFileDialog.FileName, writer.Data);

                writer.Dispose();


            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DLM files (*.dlm) | *.dlm";
            var result = openFileDialog.ShowDialog();

            if (result == true)
            {
                byte[] fileContent = File.ReadAllBytes(openFileDialog.FileName);

                BigEndianReader reader = new BigEndianReader(fileContent);
                Map = Map.FromDLM(canvas, new DlmMap(reader), ElementsManager.GetElements());
                OnMapLoaded();
            }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            Map.Id = int.Parse(mapIdtb.Text);
            Map.SubareaId = short.Parse(subid.Text);

            bool newMap = !MapsManager.MapExistsInD2P(Map.Id);

            try
            {
                if (!newMap)
                {
                    D2PManager.ModifyMap(Map.ToDLM());
                }
                else
                {
                    D2PManager.AddMap(Map.ToDLM());

                }

                MapsManager.ReloadEntries();
                MessageBox.Show("Map was exported to D2P sucessfully", "Information", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch
            {
                MessageBox.Show("Unable to export map. Client process is probably locking D2P files.", "Error", MessageBoxButton.OK, MessageBoxImage.Error); ;
            }

        }

        private void OnEditCollisionValueChanged()
        {
            if (Map == null)
            {
                return;
            }
            if (EditCollision)
            {
                TilePreview.Fill = Brushes.Transparent;

                foreach (var cell in Map.Cells)
                {
                    if (!cell.Data.Mov)
                    {
                        cell.Fill(NOT_WALKABLE_COLOR);
                    }
                }
            }
            else
            {
                foreach (var cell in Map.Cells)
                {
                    cell.Unfill();
                }
            }
        }
        private void EditCollisions_Click(object sender, RoutedEventArgs e)
        {
            OnEditCollisionValueChanged();
        }
        private ContextMenu CreateContextMenu()
        {
            ContextMenu menu = new ContextMenu();

            MenuItem copyItem = new MenuItem() { Header = "_View Elements" };
            copyItem.Click += Test;
            menu.Items.Add(copyItem);

            MenuItem flipXItem = new MenuItem() { Header = "_Flip" };
            //  flipXItem.Click += MenuRotateXSprite;
            menu.Items.Add(flipXItem);

            MenuItem flipYItem = new MenuItem() { Header = "_Remove All Elements" };
            // flipYItem.Click += MenuRotateYSprite;
            menu.Items.Add(flipYItem);

            return menu;
        }
        private void Test(object sender, RoutedEventArgs args)
        {
            ElementsExplorer ex = new ElementsExplorer();
            ex.Show();
        }
        private void AdditionalGround_Click(object sender, RoutedEventArgs e)
        {
            OnMapLoaded();
        }

        private void Ground_Click(object sender, RoutedEventArgs e)
        {
            OnMapLoaded();
        }

        private void Decor_Click(object sender, RoutedEventArgs e)
        {
            OnMapLoaded();
        }

        private void AdditionalDecor_Click(object sender, RoutedEventArgs e)
        {
            OnMapLoaded();
        }

        private void VerifyD2PValidityButton(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("This operation may take a while. Do you want to continue?", "Informations", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    MapsManager.GetMaps();
                    MessageBox.Show("D2P File is valid!", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                catch
                {
                    MessageBox.Show("One of the D2P File is invalid.", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }

            }


        }

        private void CaptureImageClick(object sender, RoutedEventArgs e)
        {
            var image = WpfUtils.CapturePngFromCanvas(canvas);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG files (*.png) | *.png";
            var result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, image);
            }
        }

        private void Cp_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (Map != null && backgroundColor.SelectedColor.HasValue)
            {
                Map.BackgroundColor = backgroundColor.SelectedColor.Value;
                canvas.Background = new SolidColorBrush(backgroundColor.SelectedColor.Value);
            }
        }

        private void ResetPixelOffsetClick(object sender, RoutedEventArgs e)
        {
            PixelOffset = new Point(0, 0);
        }
        private void MapEditor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Z:
                    PixelOffset.Y -= 6;
                    break;
                case Key.Q:
                    PixelOffset.X -= 6;
                    break;
                case Key.S:
                    PixelOffset.Y += 6;
                    break;
                case Key.D:
                    PixelOffset.X += 6;
                    break;
            }
            OnPixelOffsetValueChanged();


        }

        private void OnPixelOffsetValueChanged()
        {

            pixelOffsetX.Text = PixelOffset.X.ToString();
            pixelOffsetY.Text = PixelOffset.Y.ToString();

            if (TilePreview != null && TilePreview.DataContext is Cell)
            {
                PreviewTile((Cell)TilePreview.DataContext);
            }
        }

        private void PixelOffsetX_TextChanged(object sender, TextChangedEventArgs e)
        {
            int offsetX = 0;

            if (int.TryParse(pixelOffsetX.Text, out offsetX))
            {
                PixelOffset.X = offsetX;
            }
        }

        private void PixelOffsetY_TextChanged(object sender, TextChangedEventArgs e)
        {
            int offsetY = 0;

            if (int.TryParse(pixelOffsetY.Text, out offsetY))
            {
                PixelOffset.Y = offsetY;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start(ConfigFile.Instance.ClientPath);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadMap(Map.Top);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadMap(Map.Left);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            LoadMap(Map.Right);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            LoadMap(Map.Bottom);
        }
    }
}
