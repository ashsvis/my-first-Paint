using System;
using System.Drawing;

namespace PaintLibrary.Core
{
    /// <summary>
    /// Tool to change layer
    /// </summary>
    [Serializable]
    public abstract class Tool
    {
        /// <summary>
        /// Apply tool to layer
        /// </summary>
        internal virtual void Apply(Document doc)
        {
        }

        /// <summary>
        /// Render preview
        /// </summary>
        internal virtual void Render(Graphics gr, Document doc)
        {
            gr.DrawImage(doc.Layer, Point.Empty);
        }
    }
}
