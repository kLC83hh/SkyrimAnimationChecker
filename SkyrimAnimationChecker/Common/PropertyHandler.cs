using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.Common
{
    public interface IPropertyHandler
    {
        #region Keys
        [System.Text.Json.Serialization.JsonIgnore]
        public string[] Keys { get; }
        public abstract int KeysOrderComparer(string key);
        #endregion

        #region Values
        [System.Text.Json.Serialization.JsonIgnore]
        public object[] Values => PropertyHandleGetValues<object>();
        public virtual T[] PropertyHandleGetValues<T>()
        {
            T[] vals = new T[Keys.Length];
            for (int i = 0; i < Keys.Length; i++)
            {
                vals[i] = PropertyHandleGetValue<T>(Keys[i]);
            }
            return vals;
        }
        #endregion

        #region Property Handling
        public virtual T PropertyHandleGetValue<T>(string key) => (T)(GetType().GetProperty(key)?.GetValue(this) ?? new());
        public virtual void PropertyHandleSetValue<T>(string key, T data) => GetType().GetProperty(key)?.SetValue(this, data);
        #endregion

    }
    public class PropertyHandler : Notify.NotifyPropertyChanged, IPropertyHandler
    {
        public PropertyHandler(string[]? KeysIgnore = null, bool? ReplaceKeysException = null, object[]? KeysOrder = null, object[]? KeysOrder2 = null, bool? RawOrderData = null)
        {
            if (KeysIgnore != null)
            {
                if (ReplaceKeysException != null && (bool)ReplaceKeysException) this.PropertyHandleKeysIgnore = KeysIgnore;
                else this.PropertyHandleKeysIgnore = this.PropertyHandleKeysIgnore.Concat(KeysIgnore).ToArray();
            }
            if (KeysOrder != null) this.KeysOrder = KeysOrder;
            if (KeysOrder2 != null) this.KeysOrder2 = KeysOrder2;
            if (RawOrderData != null) this.RawOrderData = (bool)RawOrderData;
            PropertyHandleMakeKeys();
        }

        #region Keys
        [System.Text.Json.Serialization.JsonIgnore]
        public string[] PropertyHandleKeysIgnore = new string[] { "Keys", "Values" };
        /// <summary>
        /// string[], Regex[] or string[] (regexpattern when RegexOrder=true)
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public object[] KeysOrder = Array.Empty<object>();
        /// <summary>
        /// string[], Regex[] or string[] (regexpattern when RegexOrder=true)
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public object[] KeysOrder2 = Array.Empty<object>();
        [System.Text.Json.Serialization.JsonIgnore]
        public bool RawOrderData = false;
        [System.Text.Json.Serialization.JsonIgnore]
        public string[] Keys => _PropertyHandleKeys ?? Array.Empty<string>();

        [System.Text.Json.Serialization.JsonIgnore]
        private string[]? _PropertyHandleKeys;
        private bool CheckPropertyHandleKeyException(string propname)
        {
            foreach (string ex in PropertyHandleKeysIgnore)
            {
                if (propname == ex) return false;
            }
            return true;
        }
        private int Ordering(string key, object[] order, int mult = 1)
        {
            for (int i = 1; i < order.Length + 1; i++)
            {
                if (order[i - 1] is string s && key.Contains(s)) return i * mult;
                if (order[i - 1] is System.Text.RegularExpressions.Regex r && r.IsMatch(key)) return i * mult;
            }
            return mult > 1 ? 0 : order.Length;
        }
        private int ReverseOrdering(string key, object[] order)
        {
            int f = int.MaxValue;
            for (int i = order.Length - 1; i >= 0; i--)
            {
                if (order[i] is string && key.Contains((string)order[i])) f -= (order.Length - i);
                if (order[i] is System.Text.RegularExpressions.Regex r && r.IsMatch(key)) f -= (order.Length - i);
            }
            return f;
        }
        public virtual int KeysOrderComparer(string key)
        {
            int i = Ordering(key, KeysOrder);
            int f = Ordering(key, KeysOrder2, 10000);
            return i + f;
        }
        private void Regexize(ref object[] order)
        {
            System.Text.RegularExpressions.Regex[] r = new System.Text.RegularExpressions.Regex[order.Length];
            for (int i = 0; i < order.Length; i++)
            {
                if (order[i] is string) r[i] = new System.Text.RegularExpressions.Regex((string)order[i]);
            }
            order = r;
        }
        private void PropertyHandleMakeKeys()
        {
            var messyprops = GetType().GetProperties();
            if (messyprops.Length == 0) return;
            if (!RawOrderData)
            {
                Regexize(ref KeysOrder);
                Regexize(ref KeysOrder2);
            }
            var props = (from prop in messyprops
                         orderby KeysOrderComparer(prop.Name), prop.Name
                         select prop).ToArray();
            List<string> buffer = new();
            foreach (var prop in props)
            {
                if (CheckPropertyHandleKeyException(prop.Name))
                    buffer.Add(prop.Name);
            }
            _PropertyHandleKeys = buffer.ToArray();
        }
        #endregion

        #region Values
        [System.Text.Json.Serialization.JsonIgnore]
        public object[] Values => PropertyHandleGetValues<object>();
        public virtual T[] PropertyHandleGetValues<T>()
        {
            T[] vals = new T[Keys.Length];
            for (int i = 0; i < Keys.Length; i++)
            {
                try
                {
                    vals[i] = PropertyHandleGetValue<T>(Keys[i]);
                }
                catch { throw; }
            }
            return vals;
        }
        #endregion

        #region Property Handling
        public virtual T PropertyHandleGetValue<T>(string key) => (T)(GetType().GetProperty(key)?.GetValue(this) ?? new());
        public virtual void PropertyHandleSetValue<T>(string key, T data) => GetType().GetProperty(key)?.SetValue(this, data);
        #endregion


    }
}
