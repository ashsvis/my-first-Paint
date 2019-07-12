using PaintLibrary.Core;
using PaintLibrary.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintLibrary.Tools
{
    /// <summary>
    /// Tool to drawing by Pen (100 hardness, straight lines)
    /// </summary>
    [Serializable]
    public class PenTool : Tool, IMouseHandler
    {
        /// <summary>
        /// Style of drawing
        /// </summary>
        public PenStyle PenStyle { get; set; } = new PenStyle();

        //key points of polyline
        private List<PointF> points = new List<PointF>();

        internal override void Render(Graphics gr, Document doc)
        {
            //render Layer
            base.Render(gr, doc);

            //render line
            RenderLine(gr);
        }

        private void RenderLine(Graphics gr)
        {
            //draw line over Leyer
            if (points.Count > 1)
            {
                using (var pen = new Pen(Color.FromArgb((int)(PenStyle.Opacity * 255), PenStyle.Color), PenStyle.Width))
                {
                    pen.LineJoin = LineJoin.Round;
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;

                    //gr.DrawLines(pen, points.ToArray());

                    var ps = points.ToArray();
                    if (ps.Length > 2)
                        for (var i = 1; i < ps.Length; i++)
                        {
                            var p1 = ps[i - 1];
                            var p2 = ps[i];
                            gr.DrawImage(new LineRenderer(p1.X, p1.Y, p2.X, p2.Y, PenStyle.Width).Render(PenStyle.Color, 90), 0, 0);
                        }


                }
            }
        }

        internal override void Apply(Document doc)
        {
            doc.OnStartOperation("Pen tool");

            //draw line on Layer 
            using (var gr = Graphics.FromImage(doc.Layer))
            {
                gr.CompositingMode = CompositingMode.SourceCopy;
                gr.CompositingQuality = CompositingQuality.HighSpeed;
                gr.SmoothingMode = SmoothingMode.HighSpeed;
                gr.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                RenderLine(gr);
            }

            //clear points
            points.Clear();
            doc.OnFinishOperation();
        }

        #region IMouseHandler

        public void OnMouseDown(Document doc, Point point)
        {
            //clear points
            points.Clear();

            //add first point
            points.Add(point);
        }

        public void OnMouseMove(Document doc, Point point)
        {
            //add next point
            points.Add(point);
        }

        public void OnMouseUp(Document doc, Point point)
        {
            //flush previous lines
            if (points.Count > 1)
                Apply(doc);
        }

        #endregion
    }
}
