using System;
using System.Drawing;

namespace PaintLibrary.Services
{
    /*
     *  Example:
     *  
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(new LineRenderer(20, 60, 200, 60, 15).Render(Color.Blue, 10), 0, 0);
            e.Graphics.DrawImage(new LineRenderer(20, 120, 200, 120, 15).Render(Color.Blue, 90), 0, 0);
        }
     */

    /// <info>
    /// Этот код для проекта любезно предоставил Рядовой (www.cyberforum.ru)
    /// </info>
    public class LineRenderer
    {
        public LineRenderer(double X1, double Y1, double X2, double Y2, double tikness)
        {
            x1 = X1; x2 = X2; y1 = Y1; y2 = Y2;
            Tikness = tikness;
            k = (Y2 - Y1) / (X2 - X1);
            b = Y1 - k * X1;
        }
        double x1, x2, y1, y2;
        double k, b, Tikness;
        private bool IsPointOnLine(int X, int Y, out double distnPercent)
        {
            double dist = DistanceToSegment(x1, y1, x2, y2, X, Y);
            distnPercent = DistanseAbs(dist, Tikness);
            if (dist < Tikness)
                return true;
            else return false;
        }
        public Bitmap Render(Color color, int hardness)
        {
            Bitmap bmp = new Bitmap(1000, 1000);
            for (int i = 0; i < bmp.Width; i++)
                for (int j = 0; j < bmp.Height; j++)
                {
                    double DistnancePercent;
                    if (IsPointOnLine(i, j, out DistnancePercent))
                    {
                        int alpha = AlphaFromHardness(DistnancePercent, hardness);
                        bmp.SetPixel(i, j, Color.FromArgb(alpha, color));
                    }
                }
            return bmp;
        }
        private double DistanseAbs(double dist, double tick) => Math.Abs((dist - tick) / ((dist + tick) / 2)) * 100;
        private int AlphaFromHardness(double distPercent, int hardness)
        {
            int res = (int)(255 / 100 * distPercent) * hardness / 10;
            if (res > 255) return 255;
            else if (res < 0) return 0;
            return res;
        }
        public double DistanceToSegment(double ax, double ay, double bx, double by, double x, double y)
        {
            double ak = Math.Sqrt((x - ax) * (x - ax) + (y - ay) * (y - ay));
            double kb = Math.Sqrt((x - bx) * (x - bx) + (y - by) * (y - by));
            double ab = Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));

            double mulScalarAKAB = (x - ax) * (bx - ax) + (y - ay) * (by - ay);
            double mulScalarBKAB = (x - bx) * (-bx + ax) + (y - by) * (-by + ay);


            if (ab == 0) return ak;

            else if (mulScalarAKAB >= 0 && mulScalarBKAB >= 0)
            {

                double p = (ak + kb + ab) / 2.0;
                double s = Math.Sqrt(Math.Abs((p * (p - ak) * (p - kb) * (p - ab))));

                return (2.0 * s) / ab;
            }

            else if (mulScalarAKAB < 0 || mulScalarBKAB < 0)
            {
                return Math.Min(ak, kb);
            }

            else return 0;

        }
    }
}
