using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PaintLibrary.Common
{
    public enum UserCursor
    {
        Pencil,
        Ribber
    }

    public static class CursorFactory
    {
        static readonly Dictionary<UserCursor, Cursor> UserCursors;

        /// <summary>
        /// Статический конструктор загружает все требуемые курсоры
        /// </summary>
        static CursorFactory()
        {
            UserCursors = new Dictionary<UserCursor, Cursor>();
            AddCursor(UserCursor.Pencil, PaintApp.Properties.Resources.Pencil);
            AddCursor(UserCursor.Ribber, PaintApp.Properties.Resources.Ribber);
            // добавлять курсоры здесь
        }

        /// <summary>
        /// Добавить курсор из ресурсов программы
        /// </summary>
        /// <param name="name"></param>
        /// <param name="source"></param>
        private static void AddCursor(UserCursor name, byte[] source)
        {
            using (var stream = new MemoryStream(source))
                UserCursors.Add(name, new Cursor(stream));
        }

        /// <summary>
        /// Взять подготовленный курсор из словаря
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Cursor GetCursor(UserCursor name)
        {
            return UserCursors.ContainsKey(name) ? UserCursors[name] : Cursors.No;
        }
    }
}
