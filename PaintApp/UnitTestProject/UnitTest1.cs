using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaintLibrary.Core;
using PaintLibrary.Services;
using PaintLibrary.Tools;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        #region Utils

        private Document doc;

        public UnitTest1()
        {
            doc = new Document();
            doc.Layer = new Bitmap(1000, 1000);
        }

        void SaveLayer(string name = "temp")
        {
            doc.Layer.Save("c:\\" + name + ".png");
        }

        #endregion

        [TestMethod]
        public void TestPenTool()
        {
            //create tool
            var tool = new PenTool();
            //set tool to doc
            doc.SetTool(tool);
            //draw some lines
            tool.OnMouseDown(doc, new Point(10, 10));
            tool.OnMouseMove(doc, new Point(10, 100));
            tool.OnMouseMove(doc, new Point(100, 100));
            tool.OnMouseUp(doc, new Point(100, 100));
            //apply tool
            doc.ApplyTool();
            //save image
            SaveLayer();
        }

        [TestMethod]
        public void TestUndo()
        {
            if (UndoRedoManager.Instance.CanUndo)
                UndoRedoManager.Instance.Undo();
            SaveLayer("afterUndo");
        }
    }
}
