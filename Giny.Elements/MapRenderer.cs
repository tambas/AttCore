using Giny.IO.D2P;
using Giny.IO.DLM;
using Giny.IO.ELE;
using Giny.Rendering.Maps;
using Giny.Rendering.SFML;
using Giny.World.Records.Maps;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.Elements
{
    public class MapRenderer : Renderer
    {
        private const float CameraSpeed = 10f;

        public Map Map
        {
            get;
            private set;
        }
        public MapRecord MapRecord
        {
            get;
            private set;
        }
        private View View
        {
            get;
            set;
        }

        public override Color ClearColor => Map.BackgroundColor;

        public MapRenderer(IntPtr handle) : base(handle)
        {
            this.View = this.Window.DefaultView;
            this.Window.MouseWheelScrolled += MouseWheelScrolled;
        }

        public override void Draw()
        {
            Map.Draw(this.Window);
            Window.SetView(View);
            HandleCameraMovement();

        }
        private void MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            this.View.Zoom(1 - (e.Delta / 10));
        }

        public bool LoadMap(int id)
        {
            var dlmMap = MapsManager.Instance.ReadMap(id);

            if (dlmMap != null)
            {
                this.Map = Map.FromDLM(dlmMap, MapsManager.Instance.Elements);
                this.MapRecord = MapRecord.GetMap(Map.Id);
                MapRecord.ReloadMembers();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void HandleCameraMovement()
        {
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

        public void ClientPerspective()
        {
            this.View.Size = new Vector2f(1659.91f, 879.2335f);
            this.View.Center = Map.GetCell(300).Center;
            this.View.Zoom(Map.ZoomScale);
        }

        public void ToggleGrid(bool value)
        {
            this.Map.DisplayBorders = value;
        }
    }
}
