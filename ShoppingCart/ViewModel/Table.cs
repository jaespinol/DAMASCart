using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingCart.ViewModel
{
    public class Table
    {
        /// <summary>
        /// elementos para la creacion de la tabla
        /// </summary>
        private Dictionary<Head, List<Column>> Data { get; set; }

        public Table()
        {
            /// Instanciamos los elementos para la creación de la tabla
            Data = new Dictionary<Head, List<Column>>();
        }

        /// <summary>
        /// Este método elimina una columna de una tabla en especifico
        /// </summary>
        /// <param name="old"></param>
        /// <param name="columnremove"></param>
        /// <returns></returns>
        public static Table operator -(Table old, Head columnremove)
        {
            if (old == null)
            {
                throw new Exception("La columna a eliminar no fue encontrada debido a que no existen columnas en la tabla actual");
            }
            Table t = new Table();
            foreach (var head in old.Header)
            {
                if (!head.HeadName.Equals(columnremove.HeadName))
                {
                    var columnlist = old.ListOf(head);
                    foreach (var column in columnlist)
                    {
                        t.Add(column);
                    }
                }
            }
            return t;
        }

        public void RemoveColumn(Head columnremove)
        {
            foreach (var head in Header)
            {
                if (head.HeadName.Equals(columnremove.HeadName))
                {
                    Data.Remove(head);
                }
            }
        }

        /// <summary>
        /// Este método u operacion, agrega todo lo que existe en una tabla a otra tabla
        /// En teoria de conjuntos, es una simple [UNION]
        /// </summary>
        /// <param name="old">tabla destino</param>
        /// <param name="add">tabla fuente</param>
        /// <returns>tabla</returns>
        public static Table operator +(Table old, Table add)
        {
            if (old == null) return add;
            if (old.Header.Count == add.Header.Count || old.Header.Count == 0)
            {
                foreach (var head in add.Header)
                {
                    var columnlist = add.ListOf(head);
                    foreach (var column in columnlist)
                    {
                        old.Add(column);
                    }
                }
            }
            return old;
        }

        /// <summary>
        /// Esta propiedad devuelve toda una lista de elementos tipo Head [Cabecera]
        /// </summary>
        public List<Head> Header
        {
            get
            {
                List<Head> values = new List<Head>();
                if (Data != null)
                {
                    if (Data.Count > 0)
                    {
                        foreach (var keyvalue in Data)
                        {

                            values.Add(keyvalue.Key);
                        }
                    }
                }
                return values;
            }
        }

        /// <summary>
        /// Este método agrega una cabecera a la tabla actual
        /// </summary>
        /// <param name="head"></param>
        public void AddHead(Head head)
        {
            if (head == null)
            {
                return;
            }

            var headfound = Data.Any(e => e.Key.HeadName == head.HeadName);
            if (!headfound)
            {
                Row<Column> values = new Row<Column>();
                Data.Add(head, values);
            }
        }

        /// <summary>
        /// Este método añade una columna a la tabla actual
        /// </summary>
        /// <param name="column"></param>
        public void Add(Column column)
        {
            var head = Data.Keys.FirstOrDefault(e => e.HeadName == column.ColumnName);
            if (head == null)
            {
                Row<Column> row = new Row<Column>();
                row.Add(column);
                head = new Head
                {
                    CharSize = column.CharSize,
                    HeadName = column.ColumnName
                };
                Data.Add(head, row);
            }
            else
            {
                Data[head].Add(column);
            }
        }

        /// <summary>
        /// Este método devuelve toda una columna de datos
        /// </summary>
        /// <param name="head">Name of column for the list [Collection]</param>
        /// <returns></returns>
        public List<Column> ListOf(Head head)
        {
            var listofhead = Data.Keys.FirstOrDefault(e => e.HeadName == head.HeadName);
            if (listofhead != null)
            {
                return Data[listofhead];
            }
            throw new KeyNotFoundException("Nombre de columna no encontrada");
        }

        /// <summary>
        /// Este método devuelve una fila con base a un numero entero, el cual haria referencia al numero de fila que se desea obtener
        /// </summary>
        /// <param name="row">Row of Column [collection] </param>
        /// <returns></returns>
        public Row<Column> Row(int row)
        {
            Row<Column> rowlist = new Row<Column>();
            foreach (var column in Data)
            {
                if (column.Value != null)
                {
                    if (column.Value.Count > row)
                    {
                        rowlist.Add(column.Value[row]);
                        continue;
                    }
                }
                rowlist.Add(new Column
                {
                    ColumnName = "",
                    ValueOfColumn = ""
                });
            }
            return rowlist;
        }

        /// <summary>
        /// Esta propiedad devuelve un numero que indica la cantidad de objetos column que existen dentro de la tabla
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return Data != null ? Data.Count : 0;
            }
        }

        /// <summary>
        /// Este método devuelve un numero que indica la cantidad de objetos Row que existen dentro de la tabla
        /// </summary>
        public int RowCount
        {
            get
            {
                if (Data != null)
                {
                    if (Data.Count > 0)
                    {
                        var list = Data.ElementAt(0).Value;
                        return list != null ? list.Count : 0;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// Esta propiedad devuelve todas las filas que tiene la tabla
        /// </summary>
        public List<Row<Column>> Rows
        {
            get
            {
                List<Row<Column>> rows = new List<Row<Column>>();
                for (int i = 0; i < this.RowCount; i++)
                {
                    rows.Add(Row(i));
                }
                return rows;
            }
        }

        /// <summary>
        /// Este método agrega una fila
        /// </summary>
        /// <param name="row"></param>
        public void AddRow(Row<Column> row = null)
        {
            if (row == null)
            {
                row = new Row<Column>();
            }

            if (Header.Count == 0)
            {
                foreach (var column in row)
                {
                    Column columna = new Column();
                    if (column != null)
                    {
                        columna.ColumnName = column.ColumnName;
                        column.ValueOfColumn = column.ValueOfColumn;
                    }
                    Add(columna);
                }
            }
            else
            {
                foreach (var head in Header)
                {
                    var column = row.FirstOrDefault(e => e.ColumnName == head.HeadName);
                    if (column == null)
                    {
                        column = new Column { ValueOfColumn = "", ColumnName = head.HeadName };
                    }
                    Add(column);
                }
            }
        }

        /// <summary>
        /// Realiza una funcion de remplazo, iterando una columna y ejecutando el código
        /// permitiendo la edicion del código con bajo acoplamiento de código.
        /// </summary>
        /// <param name="column">Nombre de la columna a editar</param>
        /// <param name="replacebyfunction">Funcion que realizara el remplazo o cambio de la variable columna</param>
        /// <returns></returns>
        public Table Replace(Head column, Func<Column, Column> replacebyfunction)
        {
            var old = this;
            var list = old.ListOf(column);
            old -= column;
            foreach (var item in list)
            {
                Column replace = replacebyfunction(item);
                old.Add(replace);
            }
            return old;
        }

        public bool Exists(Column column)
        {
            var found = this.FirstOrDefault(e => e.ValueOfColumn == column.ValueOfColumn);
            return found != null;
        }

        public Column FirstOrDefault(Func<Column, bool> selector)
        {
            if (this.RowCount > 0)
            {
                foreach (var row in Rows)
                {
                    row.GroupBy(e => e.ColumnName, (c, column) =>
                    {
                        return column;
                    });
                    return row.FirstOrDefault(selector);
                }
            }
            return default(Column);
        }

        public Dictionary<Column, Table> GroupBy(Head head)
        {
            Dictionary<Column, Table> columntable = new Dictionary<Column, Table>();
            foreach (var row in this.Rows)
            {
                foreach (var column in row)
                {
                    if (column.ColumnName.Equals(head.HeadName))
                    {
                        if (columntable.ContainsKey(column))
                        {
                            columntable[column].AddRow(row);
                        }
                        else
                        {
                            columntable.Add(column, new Table());
                            columntable[column].AddRow(row);
                        }
                    }
                }
            }
            return columntable;
        }

        public void RemoveAt(Func<Column, bool> remove)
        {
            if (Header == null) return;
            List<Head> headstoremove = new List<Head>();
            foreach (var head in Header)
            {
                var list = ListOf(head);
                int count = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    var column = list[i];
                    if (remove(column))
                    {
                        count++;
                    }
                }
                if (count == list.Count)
                {
                    headstoremove.Add(head);
                }
            }

            foreach (var head in headstoremove)
            {
                RemoveColumn(head);
            }
        }

        public int TotalCharsPerRow { get; set; }
    }

    public class Head
    {
        public string HeadName { get; set; }
        public static Head Name(string headname, int charsize = 0)
        {
            return new Head
            {
                HeadName = headname,
                CharSize = charsize
            };
        }
        public int CharCount
        {
            get
            {
                if (!string.IsNullOrEmpty(HeadName))
                {
                    return HeadName.Length;
                }
                return 0;
            }
        }
        public int CharSize { get; set; }

        public override string ToString()
        {
            if (CharCount > CharSize || CharCount == CharSize)
            {
                var temp = HeadName;
                var newvalue = "";
                for (int i = 0; i < CharSize; i++)
                {
                    if (i == CharSize - 1)
                    {
                        newvalue += " ";
                    }
                    else
                    {
                        newvalue += temp[i].ToString();
                    }
                }
                return newvalue;
            }
            else if (CharCount < CharSize)
            {
                var size = HeadName.Length;
                for (int i = size; i < CharSize; i++)
                {
                    HeadName += " ";
                }
            }
            return HeadName;
        }
    }

    public class Row<Column> : List<Column>
    {
        public Column FirstOrDefault(Func<Column, bool> selector)
        {
            if (Count > 0)
            {
                foreach (var column in this)
                {
                    if (selector(column))
                    {
                        return column;
                    }
                }
            }
            return default(Column);
        }

        public override string ToString()
        {
            string row = "";
            foreach (var column in this)
            {
                row += column.ToString() + "";
            }
            return row;
        }
    }

    public class Column
    {
        public string ColumnName { get; set; }
        public string ValueOfColumn { get; set; }
        public int CharCount
        {
            get
            {
                if(!string.IsNullOrEmpty(ValueOfColumn))
                {
                    return ValueOfColumn.Length;
                }
                return 0;
            }
        }
        public int CharSize { get; set; }

        public override string ToString()
        {
            if(CharCount > CharSize || CharCount == CharSize)
            {
                var temp = ValueOfColumn;
                var newvalue = "";
                if (CharSize > 0)
                {
                    for (int i = 0; i < CharSize; i++)
                    {
                        if (i == CharSize - 1)
                        {
                            newvalue += " ";
                        }
                        else
                        {
                            newvalue += temp[i].ToString();
                        }
                    }
                }
                else
                {
                    newvalue = temp;
                }
                return newvalue;
            }
            else if(CharCount < CharSize)
            {
                var size = ValueOfColumn.Length;
                for (int i = size; i < CharSize; i++)
                {
                    ValueOfColumn += " ";
                }
            }
            return ValueOfColumn;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Column column = obj as Column;
            if (column.ValueOfColumn.Equals(ValueOfColumn) && column.ColumnName.Equals(ColumnName))
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ColumnName.GetHashCode() ^ ValueOfColumn.GetHashCode();
        }

        public Head ToHead()
        {
            return new Head
            {
                CharSize = CharSize,
                HeadName = ColumnName
            };
        }
    }
}