using Giny.Maps.Maps;
using Giny.Maps.SFML;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.Maps.Renderers
{
    public class EditorRenderer : Renderer
    {
        private const float CameraSpeed = 5f;

        private const int MapGridSize = 3;

        private List<Map> Maps
        {
            get;
            set;
        }
        private View View
        {
            get;
            set;
        }

        public EditorRenderer(IntPtr handle) : base(handle)
        {
            this.View = new View(Window.GetView());

            this.Window.MouseWheelScrolled += MouseWheelScrolled;
            this.Maps = new List<Map>();

            for (int x = 0; x < MapGridSize; x++)
            {
                for (int y = 0; y < MapGridSize; y++)
                {
                    Vector2f position = new Vector2f(Map.PixelSize.X * x, Map.PixelSize.Y * y);
                    Map map = new Map(position);
                    Maps.Add(map);
                }
            }
        }
        private void MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            this.View.Zoom(1 - (e.Delta / 10));
        }
        public override void Draw()
        {
            HandleCameraMovement();

            Window.SetView(View);

            foreach (var map in Maps)
            {
                map.Draw(this.Window);
            }

        }

        private void HandleCameraMovement()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.LControl))
            {
                return;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
            {
                View.Move(new Vector2f(-CameraSpeed, 0));
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                View.Move(new Vector2f(CameraSpeed, 0));
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Z))
            {
                View.Move(new Vector2f(0, -CameraSpeed));
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                View.Move(new Vector2f(0, CameraSpeed));
            }

        }
    }
}
