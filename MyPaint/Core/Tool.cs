using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.Core
{
    /// <summary>
    /// Tool to change layer
    /// </summary>
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
        }
    }
}
