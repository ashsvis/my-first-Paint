using PaintApp.Services;
using PaintLibrary.Common;
using PaintLibrary.Core;
using PaintLibrary.Services;
using PaintLibrary.Tools;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PaintApp
{
    public partial class MainForm : Form
    {
        private bool down;
        private Document doc;
        private UndoRedoController undoRedoController;
        private Tool tool;

        public MainForm()
        {
            InitializeComponent();
            CreateNewDocument();
        }

        private void CreateNewDocument()
        {
            doc = new Document
            {
                Layer = new Bitmap(1000, 1000)
            };
       
            undoRedoController = new UndoRedoController(doc);
            workArea.Size = new Size(doc.Layer.Width, doc.Layer.Height);
            workArea.Cursor = CursorFactory.GetCursor(UserCursor.Pencil);
            UncheckedTools();
            tsbPencil.Checked = true;
            tool = new PenTool();
            doc.SetTool(tool);
            UndoRedoManager.Instance.ClearHistory();
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                down = true;
                ((IMouseHandler)tool).OnMouseDown(doc, e.Location);
                workArea.Invalidate();
            }
        }

        private void WorkArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (down)
            {
                ((IMouseHandler)tool).OnMouseMove(doc, e.Location);
                workArea.Invalidate();
            }
        }

        private void WorkArea_MouseUp(object sender, MouseEventArgs e)
        {
            if (down)
            {
                down = false;
                ((IMouseHandler)tool).OnMouseUp(doc, e.Location);
                workArea.Invalidate();
            }
        }

        private void WorkArea_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            doc.Render(graphics);
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UncheckedTools()
        {
            tsbPencil.Checked = false;
            tsbRibber.Checked = false;
            doc.CancelTool();
        }

        private void tsbPencil_Click(object sender, EventArgs e)
        {
            UncheckedTools();
            tsbPencil.Checked = true;
            workArea.Cursor = CursorFactory.GetCursor(UserCursor.Pencil);
            tool = new PenTool();
            doc.SetTool(tool);
        }

        private void tsbRibber_Click(object sender, EventArgs e)
        {
            UncheckedTools();
            tsbRibber.Checked = true;
            workArea.Cursor = CursorFactory.GetCursor(UserCursor.Ribber);
            tool = new RibberTool();
            doc.SetTool(tool);
        }

        private void tsmiNew_Click(object sender, EventArgs e)
        {
            CreateNewDocument();
            workArea.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tsbUndo.Enabled = tsmiUndo.Enabled = UndoRedoManager.Instance.CanUndo;
            tsbRedo.Enabled = tsmiRedo.Enabled = UndoRedoManager.Instance.CanRedo;
        }

        private void tsmiUndo_Click(object sender, EventArgs e)
        {
            UndoRedoManager.Instance.Undo();
            workArea.Invalidate();
        }

        private void tsmiRedo_Click(object sender, EventArgs e)
        {
            UndoRedoManager.Instance.Redo();
            workArea.Invalidate();
        }
    }
}
