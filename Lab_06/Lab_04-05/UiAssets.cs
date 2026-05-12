using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lab_04_05
{
    public static class UiAssets
    {
        // Генерация иконки окна 
        public static BitmapSource CreateWindowIcon()
        {
            
            const int width = 32;
            const int height = 32;

            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                var bgBrush = new SolidColorBrush(Color.FromRgb(33, 150, 243));
                var whitePen = new Pen(Brushes.White, 2.0)
                {
                    StartLineCap = PenLineCap.Round,
                    EndLineCap = PenLineCap.Round,
                    LineJoin = PenLineJoin.Round
                };

                dc.DrawRectangle(bgBrush, null, new System.Windows.Rect(0, 0, width, height));
                // Руль: внешний круг, центральная ступица и три спицы.
                const double cx = 16;
                const double cy = 16;
                dc.DrawEllipse(null, whitePen, new System.Windows.Point(cx, cy), 9, 9);
                dc.DrawEllipse(null, whitePen, new System.Windows.Point(cx, cy), 2.5, 2.5);
                dc.DrawLine(whitePen, new System.Windows.Point(cx, cy), new System.Windows.Point(cx, 8.5));
                dc.DrawLine(whitePen, new System.Windows.Point(cx, cy), new System.Windows.Point(10.3, 19.8));
                dc.DrawLine(whitePen, new System.Windows.Point(cx, cy), new System.Windows.Point(21.7, 19.8));

                var arc = new StreamGeometry();
                using (var g = arc.Open())
                {
                    g.BeginFigure(new System.Windows.Point(10.3, 19.8), false, false);
                    g.ArcTo(new System.Windows.Point(21.7, 19.8), new System.Windows.Size(5.8, 3.2), 0, false, SweepDirection.Clockwise, true, false);
                }
                arc.Freeze();
                dc.DrawGeometry(null, whitePen, arc);
            }

            var bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(visual);
            bmp.Freeze();
            return bmp;
        }

        // Пользовательский курсор для всего приложения.
        public static Cursor CreateCustomCursor() => Cursors.Pen;
    }
}
