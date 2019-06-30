using PaintLibrary.Core;
using PaintLibrary.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintApp
{
    public partial class MainForm : Form
    {
        private bool down;
        private Document doc;
        private PenTool tool;

        public MainForm()
        {
            InitializeComponent();
            doc = new Document
            {
                Layer = new Bitmap(1000, 1000)
            };
            workArea.Size = new Size(doc.Layer.Width, doc.Layer.Height);
            tool = new PenTool();
            doc.SetTool(tool);
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                down = true;
                tool.OnMouseDown(doc, e.Location);
                workArea.Invalidate();
            }
        }

        private void WorkArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (down)
            {
                tool.OnMouseMove(doc, e.Location);
                workArea.Invalidate();
            }
        }

        private void WorkArea_MouseUp(object sender, MouseEventArgs e)
        {
            if (down)
            {
                down = false;
                tool.OnMouseUp(doc, e.Location);
                workArea.Invalidate();
            }
        }

        private void WorkArea_Paint(object sender, PaintEventArgs e)
        {
            doc.Render(e.Graphics);
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
