using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPaint.Core
{
    /// <summary>
    /// Определение команды, обладающей возможностью отмены
    /// </summary>
    public abstract class UndoableCommand
    {
        public string Name { get; set; }
        public abstract void Undo();
        public abstract void Redo();
    }
}
