using Giny.Core.Time;
using Giny.Rendering.Maps;
using Giny.Rendering.Winforms;
using Giny.World.Records.Maps;
using System;
using System.Collections.Generic;
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
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using Giny.Protocol.Custom.Enums;
using Giny.Rendering.Maps.Elements;

namespace Giny.Elements
{
    /// <summary>
    /// Logique d'interaction pour Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        private const double FramePerSecond = 60d;



        private MapRenderer Renderer
        {
            get;
            set;
        }
        public Editor()
        {
            InitializeComponent();
            this.Loaded += Editor_Loaded;
            host.Child = new DrawingSurface();
            this.host.KeyDown += Editor_KeyDown;
            host.Focus();

        }


        private void LoadMap(int mapId)
        {
            Renderer.LoadMap(mapId);
            Renderer.ToggleGrid(displayGrid.IsChecked.Value);
            targetMapId.Text = Renderer.Map.Id.ToString();
            interactives.Items.Clear();

            foreach (var element in Renderer.MapRecord.Elements)
            {
                if (element.Skill != null)
                {
                    interactives.Items.Add(element.Skill);
                }
            }


        }
        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.R:
                    {
                        var maps = MapRecord.GetMaps(Renderer.MapRecord.Position.Point).ToList();

                        int index = maps.IndexOf(Renderer.MapRecord);

                        if (maps.Count > index + 1)
                            LoadMap((int)maps[index + 1].Id);
                        else
                            LoadMap((int)maps[0].Id);

                        break;
                    }
                case Key.M:
                    LoadMap(World.Managers.Maps.MapsManager.Instance.GetNeighbourMapId(Renderer.MapRecord, MapScrollEnum.RIGHT));
                    break;
                case Key.K:
                    LoadMap(World.Managers.Maps.MapsManager.Instance.GetNeighbourMapId(Renderer.MapRecord, MapScrollEnum.LEFT));
                    break;
                case Key.O:
                    LoadMap(World.Managers.Maps.MapsManager.Instance.GetNeighbourMapId(Renderer.MapRecord, MapScrollEnum.TOP));
                    break;
                case Key.L:
                    LoadMap(World.Managers.Maps.MapsManager.Instance.GetNeighbourMapId(Renderer.MapRecord, MapScrollEnum.BOTTOM));
                    break;
            }

        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            Renderer = new MapRenderer(host.Child.Handle);

            LoadMap(191105026);

            var timer = new HighPrecisionTimer((int)(1000 / FramePerSecond));
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, HighPrecisionTimer.TickEventArgs e)
        {
            if (this.IsVisible)
            {
                Dispatcher.Invoke(() =>
                {
                    Renderer.Loop();
                });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadMap(int.Parse(targetMapId.Text));

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Renderer.ClientPerspective();
        }

        private void displayGrid_Click(object sender, RoutedEventArgs e)
        {
            Renderer.ToggleGrid(displayGrid.IsChecked.Value);
        }

        private void interactives_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                foreach (InteractiveSkillRecord skill in e.RemovedItems)
                {
                    ColorInteractive(skill, Color.White);
                }
            }


            var skillRecord = (InteractiveSkillRecord)interactives.SelectedItem;

            if (skillRecord != null)
                ColorInteractive(skillRecord, Color.Red);



        }

        private void ColorInteractive(InteractiveSkillRecord skill, Color color)
        {
            var element = Renderer.Map.FindElement<MapGraphicalElement>(x => x.DlmElement.Identifier == skill.Identifier);

            if (element != null)
            {
                (element).Sprite.Color = color;
            }

        }
    }
}
