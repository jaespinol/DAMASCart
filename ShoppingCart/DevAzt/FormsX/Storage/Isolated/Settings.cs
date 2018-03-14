using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DevAzt.FormsX.Storage.Isolated
{
    public class Settings //: IDictionary<string, object>
    {

        private IDictionary<string, object> Isolated = Application.Current.Properties;

        public static Settings Data { get; set; }

        static Settings(){
            Data = new Settings();
        }

        public async Task SaveAsync()
        {
            await Application.Current.SavePropertiesAsync();
        }

        public object this[string key]
        {
            get
            {
                if (Isolated.ContainsKey(key))
                {
                    return Isolated[key];
                }
                return null;
            }

            set
            {
                Add(key, value);
            }
        }
        
        public ICollection<string> Keys => Isolated.Keys;

        public ICollection<object> Values => Isolated.Values;

        public int Count => Isolated != null ? Isolated.Count : 0;

        public bool IsReadOnly => false;

        public bool Replace { get; internal set; }

        public event EventHandler<KeyValuePair<string, object>> AddConstraintExists;

        protected virtual void OnAddConstraintExists(KeyValuePair<string, object> info)
        {
            if (AddConstraintExists != null)
            {
                AddConstraintExists.Invoke(this, info);
            }
        }

        public void Add(string key, object value)
        {
            if (Isolated.ContainsKey(key))
            {
                if (Replace)
                {
                    Isolated[key] = value;
                }
                else
                {
                    OnAddConstraintExists(new KeyValuePair<string, object>(key, Isolated[key]));
                }
            }
            else
            {
                Isolated.Add(key, value);
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            if (Isolated.ContainsKey(item.Key))
            {
                if (Replace)
                {
                    Isolated[item.Key] = item.Value;
                }
                else
                {
                    OnAddConstraintExists(item);
                }
            }
            else
            {
                Isolated.Add(item);
            }
        }

        public event EventHandler<string> GetConstraintNotExists;

        protected virtual void OnGetConstraintNotExists(string key)
        {
            if (GetConstraintNotExists != null)
            {
                GetConstraintNotExists.Invoke(this, key);
            }
        }

        public T Get<T>(string key)
        {
            if (Isolated.ContainsKey(key))
            {
                var readData = Isolated[key];
                if (readData is T)
                {
                    return (T)readData;
                }
                else
                {
                    try
                    {
                        return (T)Convert.ChangeType(readData, typeof(T));
                    }
                    catch (InvalidCastException)
                    {
                        return default(T);
                    }
                }
            }
            OnGetConstraintNotExists(key);
            return default(T);
        }

        public void Clear()
        {
            Isolated.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return Isolated.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return Isolated.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Isolated.GetEnumerator();
        }

        public event EventHandler<string> RemoveConstraintNotExists;

        public virtual void OnRemoveConstraintNotExists(string key)
        {
            if (RemoveConstraintNotExists != null)
            {
                RemoveConstraintNotExists.Invoke(this, key);
            }
        }

        public bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;
            if (Isolated.ContainsKey(key))
            {
                return Isolated.Remove(key);
            }
            OnRemoveConstraintNotExists(key);
            return false;
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            if (this.Remove(item.Key))
            {
                return Isolated[item.Key].Equals(item.Value);
            }
            OnRemoveConstraintNotExists(item.Key);
            return false;
        }

        public bool TryGetValue(string key, out object value)
        {
            if (Isolated.ContainsKey(key))
            {
                value = Isolated[key];
                return true;
            }
            value = null;
            return false;
        }

        /*
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Isolated.GetEnumerator();
        }
        */
    }
}
