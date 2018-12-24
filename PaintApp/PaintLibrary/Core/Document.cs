using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaintLibrary.Services;

namespace PaintLibrary.Core
{
    /// <summary>
    /// Document
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Current tool
        /// </summary>
        public Tool Tool { get; protected set; }

        /// <summary>
        /// Current layer
        /// </summary>
        public Bitmap Layer { get; set; }

        /// <summary>
        /// Render layer
        /// </summary>
        public void Render(Graphics gr)
        {
            if (Tool != null)
                Tool.Render(gr, this);
            else
                gr.DrawImage(Layer, Point.Empty);
        }

        /// <summary>
        /// Cancel current tool (w/o applying)
        /// </summary>
        public void CancelTool()
        {
            Tool = null;
        }

        /// <summary>
        /// Apply and clear current tool (if persented)
        /// </summary>
        public void ApplyTool()
        {
            if (Tool != null)
                Tool.Apply(this);
            Tool = null;
        }

        /// <summary>
        /// Set new tool and Apply current tool (if presented)
        /// </summary>
        public void SetTool(Tool tool)
        {
            if (Tool != null)
                ApplyTool();
            Tool = tool;
        }

        /// <summary>
        /// Create restore point for Undo/Redo manager
        /// </summary>
        internal void CreateUndoPoint(string toolName)
        {
            //make clone of current Layer
            var ms = new MemoryStream();
            Layer.Save(ms, ImageFormat.Png);

            //create undo action
            Action undo = () =>
            {
                ms.Position = 0;
                if (Layer != null)
                    Layer.Dispose();
                Layer = new Bitmap(ms);
            };

            //create undo command (redo is not supported)
            var command = new ActionCommand(undo, null) {Name = toolName};

            //push command to undo/redo manager
            UndoRedoManager.Instance.Add(command);
        }
    }

}
