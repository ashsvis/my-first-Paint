using PaintLibrary.Core;
using PaintLibrary.Services;
using System;

namespace PaintApp.Services
{
    /// <summary>
    /// Управление операциями отмены/возврата действий
    /// </summary>
    [Serializable]
    public class UndoRedoController
    {
        [NonSerialized]
        private Document _snapshot;
        [NonSerialized]
        private readonly Document _document;

        private string _operationName;

        /// <summary>
        /// Конструктор запоминает рабочий документ, с которым будет работать undo/redo
        /// </summary>
        /// <param name="document"></param>
        public UndoRedoController(Document document)
        {
            _document = document;
            _document.LayerStartChanging += (operation) => { OnStartOperation(operation); };
            _document.LayerChanged += () => { OnFinishOperation(); };
        }

        /// <summary>
        /// Выполняем перед началом операций изменения контента
        /// </summary>
        /// <param name="operationName"></param>
        public void OnStartOperation(string operationName)
        {
            // сначала запоминаем копию по значению рабочего слоя в локальной переменной
            _snapshot = _document.DeepClone();
            // запоминаем также наименование операции
            _operationName = operationName;
        }

        /// <summary>
        /// Выполняем после окончания операций изменения контента
        /// </summary>
        public void OnFinishOperation()
        {
            var afterOperationSnapshot = _document.DeepClone();
            var beforeOperationSnapshot = _snapshot; // захват переменной при выполнении тела акций

            Action undo = () =>
            {
                _document.Layer = beforeOperationSnapshot.DeepClone().Layer;
            };

            Action redo = () =>
            {
                _document.Layer = afterOperationSnapshot.DeepClone().Layer;
            };

            UndoRedoManager.Instance.Add(new ActionCommand(undo, redo) { Name = _operationName });
        }
    }
}
