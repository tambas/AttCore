using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Giny.Maps.WPF
{
    public class WpfUtils
    {
        public static BitmapImage GetImageSource(string imagePath)
        {
            BitmapImage source = new BitmapImage();
            source.BeginInit();
            source.UriSource = new Uri(imagePath, UriKind.Absolute);
            source.CreateOptions = BitmapCreateOptions.None;
            source.EndInit();
            return source;
        }
      
        public static byte[] CapturePngFromCanvas(Canvas target, double dpi = 96d)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);

            RenderTargetBitmap rtb = new RenderTargetBitmap((Int32)bounds.Width, (Int32)bounds.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual dv = new DrawingVisual();

            target.Measure(new System.Windows.Size((int)target.Width, (int)target.Height));

            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(target);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            rtb.Render(dv);

            PngBitmapEncoder png = new PngBitmapEncoder();

            png.Frames.Add(BitmapFrame.Create(rtb));

            using (MemoryStream stm = new MemoryStream())
            {
                png.Save(stm);
                return stm.ToArray();
            }
        }

    }
}
