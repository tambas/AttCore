using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.Maps.SFML
{
    public abstract class Renderer
    {
        private const uint FrameRateLimit = 144;

        protected RenderWindow Window
        {
            get;
            private set;
        }

        public Renderer(VideoMode mode, string title, Styles styles = Styles.Default)
        {
            this.Window = new RenderWindow(mode, title, styles);
            Initialize();
        }
        public Renderer(IntPtr handle)
        {
            this.Window = new RenderWindow(handle);
            Initialize();
        }

        private void Initialize()
        {
            Window.SetFramerateLimit(FrameRateLimit);
        }

        public void Display()
        {
            Window.SetActive();

            while (Window.IsOpen)
            {
                Loop();
            }
        }
        public void Loop()
        {
            Window.Clear(Color.White);
            Window.DispatchEvents();
            Draw();
            Window.Display();
        }

        public abstract void Draw();

        public RenderWindow GetWindow()
        {
            return this.Window;
        }

    }
}
