using PaintApp.Services;
using PaintLibrary.Common;
using PaintLibrary.Core;
using PaintLibrary.Services;
using PaintLibrary.Tools;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Imaging;
using System.IO;

namespace PaintApp
{
    public partial class MainForm : Form
    {
        private bool down;
        private Document doc;
        private UndoRedoController undoRedoController;
        private Tool tool;
        private string fileName = string.Empty;
        private readonly string caption;
        private readonly VersionInfo versionInfo;

        public MainForm()
        {
            InitializeComponent();
            versionInfo = new VersionInfo();
            caption = string.Format("Plain Paint (ver {0:0.0})",
                (decimal)versionInfo.Version / 10);
            CreateNewDocument();
        }

        #region для карандаша

        private PenTool penTool;
        private Image penToolColorImage;

        /// <summary>
        /// Рабочий карандаш
        /// </summary>
        private PenTool PenTool
        {
            get
            {
                if (penTool == null)
                    penTool = new PenTool();
                return penTool;
            }
        }

        private Image PenToolColorImage
        {
            get
            {
                if (penToolColorImage == null)
                {
                    penToolColorImage = new Bitmap(16, 16);
                    UpdatePenToolColorImage();
                }
                return penToolColorImage;
            }
        }

        private void UpdatePenToolColorImage()
        {
            using (var gr = Graphics.FromImage(PenToolColorImage))
            using (var brush = new SolidBrush(PenTool.PenStyle.Color))
            {
                var rect = new Rectangle(0, 0, 15, 15);
                rect.Inflate(-1, -1);
                gr.FillRectangle(brush, rect);
                gr.DrawRectangle(Pens.Black, rect);
            }
            tsbPencilColor.Invalidate();
        }

        #endregion

        #region для ластика

        private RibberTool ribberTool;
        private Image ribberToolColorImage;

        private Image RibberToolColorImage
        {
            get
            {
                if (ribberToolColorImage == null)
                {
                    ribberToolColorImage = new Bitmap(16, 16);
                    UpdateRibberToolColorImage();
                }
                return ribberToolColorImage;
            }
        }

        private void UpdateRibberToolColorImage()
        {
            using (var gr = Graphics.FromImage(RibberToolColorImage))
            using (var brush = new SolidBrush(RibberTool.RibberStyle.Color))
            {
                var rect = new Rectangle(0, 0, 15, 15);
                rect.Inflate(-1, -1);
                gr.FillRectangle(brush, rect);
                gr.DrawRectangle(Pens.Black, rect);
            }
            tsbRibberColor.Invalidate();
        }

        /// <summary>
        /// Рабочий ластик
        /// </summary>
        private RibberTool RibberTool
        {
            get
            {
                if (ribberTool == null)
                    ribberTool = new RibberTool();
                return ribberTool;
            }
        }

        #endregion

        /// <summary>
        /// Создаём пустой документ
        /// </summary>
        private void CreateNewDocument(int width = 1000, int height = 1000)
        {
            doc = new Document
            {
                Layer = new Bitmap(width, height)
            };
            // экземпляр контроллера для механизма отменить/вернуть
            undoRedoController = new UndoRedoController(doc);
            // задаём размер холста
            workArea.Size = new Size(doc.Layer.Width, doc.Layer.Height);
            workArea.Cursor = CursorFactory.GetCursor(UserCursor.Pencil);
            // начальные настройки инструментов
            UncheckedTools();
            tsbPencil.Checked = true;
            foreach (var item in tsbPencilThickness.DropDownItems.Cast<ToolStripMenuItem>())
                item.Checked = false;
            tsmiPencilThicknessOne.Checked = true;
            penTool = null;
            tool = PenTool;
            doc.SetTool(tool);
            penToolColorImage = null;
            tsbPencilColor.Image = PenToolColorImage;
            tsbPencilColor.Invalidate();
            foreach (var item in tsbRibberThickness.DropDownItems.Cast<ToolStripMenuItem>())
                item.Checked = false;
            tsmiRibberThicknessFour.Checked = true;
            ribberTool = null;
            ribberToolColorImage = null;
            tsbRibberColor.Image = RibberToolColorImage;
            tsbRibberColor.Invalidate();
            // очистка истории отмен
            UndoRedoManager.Instance.ClearHistory();
        }

        /// <summary>
        /// Действия при нажатии левой кнопки мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                down = true;
                ((IMouseHandler)tool).OnMouseDown(doc, e.Location);
                workArea.Invalidate();
            }
        }

        /// <summary>
        /// Действия при перемещении курсора мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkArea_MouseMove(object sender, MouseEventArgs e)
        {
            tsslLocation.Text = $"{e.Location.X}, {e.Location.Y}пкс";
            if (down)
            {
                ((IMouseHandler)tool).OnMouseMove(doc, e.Location);
                workArea.Invalidate();
            }
        }

        /// <summary>
        /// Действия при отпускании левой кнопки мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkArea_MouseUp(object sender, MouseEventArgs e)
        {
            if (down)
            {
                down = false;
                ((IMouseHandler)tool).OnMouseUp(doc, e.Location);
                workArea.Invalidate();
            }
        }

        /// <summary>
        /// Отрисовка текущего состояния документа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkArea_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;

            // https://rsdn.org/article/gdi/gdiplus2mag.xml
            // Устанавливает или отключает учет прозрачности при наложении растровых изображений.
            graphics.CompositingMode = CompositingMode.SourceOver; // должен быть CompositingMode.SourceOver для нашего случая

            // Управляет качеством расчета цветовых компонентов при наложении растров.
            graphics.CompositingQuality = CompositingQuality.HighSpeed;

            // Позволяет указать метод устранения ступенчатости (antialiasing) при выводе примитивов – линий и геометрических фигур.
            graphics.SmoothingMode = SmoothingMode.HighSpeed;

            // Задает метод учета смещения пикселов при интерполяции. Грубо говоря, определяет, являются ли координаты пикселов (или их центров) целыми числами при расчетах.
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

            doc.Render(graphics);
        }

        /// <summary>
        /// Пользователь закрывает приложение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Сброс текущего инструмента
        /// </summary>
        private void UncheckedTools()
        {
            tsbPencil.Checked = false;
            tsbRibber.Checked = false;
            doc.CancelTool();
        }

        /// <summary>
        /// Выбираем текущий инструмент - карандаш
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbPencil_Click(object sender, EventArgs e)
        {
            UncheckedTools();
            tsbPencil.Checked = true;
            workArea.Cursor = CursorFactory.GetCursor(UserCursor.Pencil);
            tool = PenTool; // указываем на рабочий караннаш
            doc.SetTool(tool);
        }

        /// <summary>
        /// Выбираем текущий инструмент - ластик
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbRibber_Click(object sender, EventArgs e)
        {
            UncheckedTools();
            tsbRibber.Checked = true;
            workArea.Cursor = CursorFactory.GetCursor(UserCursor.Ribber);
            tool = RibberTool; // указываем на рабочий ластик
            doc.SetTool(tool);
        }

        /// <summary>
        /// Создаём новый документ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiNew_Click(object sender, EventArgs e)
        {
            CreateNewDocument();
            workArea.Invalidate();
        }

        /// <summary>
        /// Обновляем элементы интерфейса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            tsbUndo.Enabled = tsmiUndo.Enabled = UndoRedoManager.Instance.CanUndo;
            tsbRedo.Enabled = tsmiRedo.Enabled = UndoRedoManager.Instance.CanRedo;
            tsslSize.Text = $"{doc.Layer.Width}, {doc.Layer.Height}пкс";
            tsbSave.Enabled = tsmiSave.Enabled = doc.Changed;
            tsbPencilColor.Visible = tsbPencilThickness.Visible = tsbPencil.Checked;
            tsbRibberColor.Visible = tsbRibberThickness.Visible = tsbRibber.Checked;
        }

        /// <summary>
        /// Отменить изменения на один шаг
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiUndo_Click(object sender, EventArgs e)
        {
            UndoRedoManager.Instance.Undo();
            workArea.Invalidate();
        }

        /// <summary>
        /// Вернуться после отмены изменений на один шаг
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiRedo_Click(object sender, EventArgs e)
        {
            UndoRedoManager.Instance.Redo();
            workArea.Invalidate();
        }

        /// <summary>
        /// Выбор толщины карандаша
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiThicknessOne_Click(object sender, EventArgs e)
        {
            if (tool is PenTool)
            {
                foreach (var item in tsbPencilThickness.DropDownItems.Cast<ToolStripMenuItem>())
                    item.Checked = false;
                var mi = sender as ToolStripMenuItem;
                if (mi.Tag == null) return;
                int value;
                if (!int.TryParse(mi.Tag.ToString(), out value)) return;
                mi.Checked = true;
                PenTool.PenStyle.Width = value;
            }
        }

        /// <summary>
        /// Показываем список толщин для карандаша
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbPencilThickness_ButtonClick(object sender, EventArgs e)
        {
            tsbPencilThickness.ShowDropDown();
        }

        private void tsbPencilColor_ButtonClick(object sender, EventArgs e)
        {
            penсilColorDialog.Color = PenTool.PenStyle.Color;
            if (penсilColorDialog.ShowDialog() == DialogResult.OK)
            {
                Color selColor = penсilColorDialog.Color;
                foreach (var item in penсilColorDialog.CustomColors)
                    if (item != 16777215 && !DrawUtils.FindColor(Color.FromArgb(item)))
                        DrawUtils.AddCustomColor(selColor);
                PenTool.PenStyle.Color = selColor;
                UpdatePenToolColorImage();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!doc.Changed) return;
            var result = MessageBox.Show(this, @"В документе есть несохранённые данные." + Environment.NewLine +
                @"Желаете их сохранить?", @"Завершение работы приложения",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);
            if (result == DialogResult.Yes)
                tsmiSave.PerformClick();
            else if (result == DialogResult.Cancel)
                e.Cancel = true;

        }

        private static ImageFormat GetFileFormat(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            switch (extension.ToLower())
            {
                case ".png": return ImageFormat.Png;
                case ".jpg": return ImageFormat.Jpeg;
                case ".emf": return ImageFormat.Emf;
                case ".gif": return ImageFormat.Gif;
                case ".ico": return ImageFormat.Icon;
                case ".tif": return ImageFormat.Tiff;
                default: return ImageFormat.Bmp;
            }
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                var dlg = new SaveFileDialog
                {
                    Filter = @"Файлы (.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp"
                };
                if (dlg.ShowDialog() != DialogResult.OK) return;
                fileName = dlg.FileName;
            }
            try
            {
                doc.Layer.Save(fileName, GetFileFormat(fileName));
                doc.Changed = false;
                UndoRedoManager.Instance.ClearHistory();
                Text = caption + @" - " + fileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка при сохранении файла", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsmiSaveAs_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Filter = @"Файлы (.png)|*.png|Файлы (*.jpg)|*.jpg|Файлы (*.bmp)|*.bmp"
            };
            dlg.FileName = fileName;
            if (dlg.ShowDialog() != DialogResult.OK) return;
            fileName = dlg.FileName;
            try
            {
                doc.Layer.Save(fileName, GetFileFormat(fileName));
                doc.Changed = false;
                UndoRedoManager.Instance.ClearHistory();
                Text = caption + @" - " + fileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка при сохранении файла", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Открыть файл для редактирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = @"Файлы (.png;*.jpg;*.bmp;*.gif)|*.png;*.jpg;*.bmp;*.gif"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            fileName = dlg.FileName;
            Text = caption + @" - " + fileName;
            var bitmap = (Bitmap)Image.FromFile(fileName);
            CreateNewDocument(bitmap.Width, bitmap.Height);
            using (var gr = Graphics.FromImage(doc.Layer))
                gr.DrawImage(bitmap, Point.Empty);
            workArea.Invalidate();
        }

        private void tsmiRibberThicknessFour_Click(object sender, EventArgs e)
        {
            if (tool is RibberTool)
            {
                foreach (var item in tsbRibberThickness.DropDownItems.Cast<ToolStripMenuItem>())
                    item.Checked = false;
                var mi = sender as ToolStripMenuItem;
                if (mi.Tag == null) return;
                int value;
                if (!int.TryParse(mi.Tag.ToString(), out value)) return;
                mi.Checked = true;
                RibberTool.RibberStyle.Width = value;
            }
        }

        private void tsbRibberThickness_ButtonClick(object sender, EventArgs e)
        {
            tsbRibberThickness.ShowDropDown();
        }

        private void tsbRibberColor_Click(object sender, EventArgs e)
        {
            ribberColorDialog.Color = RibberTool.RibberStyle.Color;
            if (ribberColorDialog.ShowDialog() == DialogResult.OK)
            {
                Color selColor = ribberColorDialog.Color;
                foreach (var item in ribberColorDialog.CustomColors)
                    if (item != 16777215 && !DrawUtils.FindColor(Color.FromArgb(item)))
                        DrawUtils.AddCustomColor(selColor);
                RibberTool.RibberStyle.Color = selColor;
                UpdateRibberToolColorImage();
            }

        }

        private void workArea_MouseLeave(object sender, EventArgs e)
        {
            tsslLocation.Text = "";
        }
    }
}
