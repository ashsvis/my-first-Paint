using PaintLibrary.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PaintLibrary.Tools
{
    /// <summary>
    /// Tool to erasing by Ribber (straight lines)
    /// </summary>
    [Serializable]
    public class RibberTool : Tool, IMouseHandler
    {
        /// <summary>
        /// Style of erasing
        /// </summary>
        public RibberStyle RibberStyle { get; set; } = new RibberStyle();

        //key points of polyline
        private List<PointF> points = new List<PointF>();

        internal override void Render(Graphics gr, Document doc)
        {
            //render Layer
            base.Render(gr, doc);

            //render line
            RenderLine(gr, RibberStyle.Color);
        }

        private void RenderLine(Graphics gr, Color transparent)
        {
            //erase line over Leyer
            if (points.Count > 1)
                using (var pen = new Pen(transparent, RibberStyle.Width))
                {
                    pen.LineJoin = LineJoin.Round;
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    gr.DrawLines(pen, points.ToArray());
                }
        }

        internal override void Apply(Document doc)
        {
            doc.OnStartOperation("Ribber tool");

            //erase line on Layer 
            using (var gr = Graphics.FromImage(doc.Layer))
            {
                gr.CompositingMode = CompositingMode.SourceCopy;
                gr.CompositingQuality = CompositingQuality.HighSpeed;
                gr.SmoothingMode = SmoothingMode.HighSpeed;
                gr.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                RenderLine(gr, RibberStyle.Color);
            }
            doc.Layer.MakeTransparent(RibberStyle.Color);

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
