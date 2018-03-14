using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DevAzt.FormsX.Storage.SQLite.LiteConnection
{
    public class Table<T> : TableQuery<T>
    {
        private DataBase _connection;

        /// <summary>
        /// Instanciamos la tabla
        /// </summary>
        /// <param name="conn"></param>
        public Table(DataBase conn) : base(conn)
        {
            _connection = conn;
        }

        /// <summary>
        /// Este método agrega elementos a la tabla
        /// </summary>
        /// <param name="element"></param>
        public void Add(T element)
        {
            _connection.Add(element);
        }

        /// <summary>
        /// Con este método es posible verificar si existe algun elemento en la tabla
        /// </summary>
        /// <param name="lambda">Funcion para comparar</param>
        /// <returns></returns>
        public bool Exists(Func<T, bool> lambda)
        {
            bool exists = false;
            foreach (var item in this)
            {
                if (lambda.Invoke(item))
                {
                    exists = true;
                }
            }
            return exists;
        }
    }

    public class DataBase : SQLiteConnection
    {

        List<object> elements;

        public DataBase(string databasePath, bool storeDateTimeAsTicks = true) : base(databasePath, storeDateTimeAsTicks)
        {
            elements = new List<object>();
        }

        public Table<T> DBSet<T>()
        {
            try
            {
                CreateTable<T>();
                Table<T> query = new Table<T>(this);
                return query;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DevAzt: {ex.StackTrace}");
            }
            return null;
        }

        public void Add<T>(T element)
        {
            elements.Add(element);
        }

        public bool SaveChanges()
        {
            bool status = false;
            try
            {
                BeginTransaction();
                if (elements != null && elements.Count > 0)
                {
                    InsertAll(elements);
                }
                if (_currententry != null)
                {
                    if (_currententry.State == EntryState.Modify)
                    {
                        Update(_currententry.EntryObject());
                    }
                    else if (_currententry.State == EntryState.Delete)
                    {
                        Delete(_currententry.EntryObject());
                    }
                }
                Commit();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
#if DEBUG
                throw new Exception("No fue posible guardar los cambios...", ex);
#endif
            }
            finally
            {
                elements = new List<object>();
            }
            return status;
        }

        private EntryRow _currententry { get; set; }

        public EntryRow Entry<T>(T element)
        {
            _currententry = new EntryRow(element);
            return _currententry;
        }

        public void Delete<T>(T element)
        {
            Delete(element);
        }

        public enum EntryState
        {
            Modify, Delete
        }

        public class EntryRow
        {
            private object element;

            public EntryState State { get; set; }

            public EntryRow(object _element)
            {
                element = _element;
            }

            public object EntryObject()
            {
                return element;
            }
        }
    }
}
