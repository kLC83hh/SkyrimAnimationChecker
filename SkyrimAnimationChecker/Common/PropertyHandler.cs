using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SkyrimAnimationChecker.Common
{
    public interface IPropertyHandler
    {
        #region Keys
        [JsonIgnore]
        public string[] Keys { get; }
        public abstract int KeysOrderComparer(string key);
        #endregion

        #region Values
        [JsonIgnore]
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
        public PropertyHandler(string[]? KeysIgnore = null, bool? ReplaceKeysIgnore = null, bool? RawOrderData = null, PropertyOrder[]? KeysOrder = null)
        {
            if (KeysIgnore != null)
            {
                if (ReplaceKeysIgnore != null && (bool)ReplaceKeysIgnore) this.PropertyHandleKeysIgnore = KeysIgnore;
                else this.PropertyHandleKeysIgnore = this.PropertyHandleKeysIgnore.Concat(KeysIgnore).ToArray();
            }
            if (RawOrderData != null) this.RawOrderData = (bool)RawOrderData;
            if (KeysOrder != null) this.KeysOrder = KeysOrder;
            PropertyHandleMakeKeys();
        }

        #region Keys
        [JsonIgnore]
        public string[] Keys => _PropertyHandleKeys ?? Array.Empty<string>();

        [JsonIgnore]
        public string[] PropertyHandleKeysIgnore = new string[] { "Keys", "Values" };
        /// <summary>
        /// string[], Regex[] or string[] (regexpattern when RegexOrder=true)
        /// </summary>
        [JsonIgnore]
        public PropertyOrder[] KeysOrder = Array.Empty<PropertyOrder>();
        [JsonIgnore]
        public bool RawOrderData = false;

        [JsonIgnore]
        private string[]? _PropertyHandleKeys;
        private bool CheckPropertyHandleKeysIgnore(string propname)
        {
            foreach (string ex in PropertyHandleKeysIgnore)
            {
                if (propname == ex) return false;
            }
            return true;
        }
        private int Ordering(string key, object[] order, int mult = 1, bool precede = false)
        {
            for (int i = 1; i < order.Length + 1; i++)
            {
                if (order[i - 1] is string s && key.Contains(s)) return i * mult;
                if (order[i - 1] is System.Text.RegularExpressions.Regex r && r.IsMatch(key)) return i * mult;
            }
            return precede ? 0 : order.Length;
        }
        //private int ReverseOrdering(string key, object[] order)
        //{
        //    int f = int.MaxValue;
        //    for (int i = order.Length - 1; i >= 0; i--)
        //    {
        //        if (order[i] is string && key.Contains((string)order[i])) f -= (order.Length - i);
        //        if (order[i] is System.Text.RegularExpressions.Regex r && r.IsMatch(key)) f -= (order.Length - i);
        //    }
        //    return f;
        //}
        private int product(IEnumerable<object> list, Func<object, int> expr) => list.Aggregate(1, (accu, next) => accu * expr(next));
        private int product_length(IEnumerable<object> list) => product(list, next => ((object[])next).Length);
        private int product_length(IEnumerable<PropertyOrder> list) => product(list, next => ((PropertyOrder)next).Length);
        public virtual int KeysOrderComparer(string key)
        {
            int i = 0;
            if (KeysOrder.Length > 0)
            {
                for (int j = 0; j < KeysOrder.Length; j++)
                {
                    int position = KeysOrder.Length - 1 - j;
                    if (!RawOrderData && KeysOrder[position].RegexAvailable)
                        i += Ordering(key, KeysOrder[position].Regex, product_length(KeysOrder.Skip(position + 1)), KeysOrder[position].Precede);
                    else
                        i += Ordering(key, KeysOrder[position].Raw, product_length(KeysOrder.Skip(position + 1)), KeysOrder[position].Precede);
                }
            }

            return i;
        }
        private void Regexize(ref object[] order)
        {
            if (order.Length == 0) return;
            System.Text.RegularExpressions.Regex[] r = new System.Text.RegularExpressions.Regex[order.Length];
            for (int i = 0; i < order.Length; i++)
            {
                if (order[i] is string) r[i] = new System.Text.RegularExpressions.Regex((string)order[i], System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            order = r;
        }
        private void PropertyHandleMakeKeys()
        {
            // collect all keys
            var messyprops = GetType().GetProperties();
            if (messyprops.Length == 0) return;

            // collect correct keys
            List<string> buffer = new();
            foreach (var prop in messyprops)
            {
                if (CheckPropertyHandleKeysIgnore(prop.Name))
                    buffer.Add(prop.Name);
            }

            // re-order collection of correct keys
            var props = (from propName in buffer
                         orderby KeysOrderComparer(propName), propName
                         select propName).ToArray();

            _PropertyHandleKeys = props;
        }
        #endregion

        #region Values
        [JsonIgnore]
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

    public class PropertyOrder
    {
        public PropertyOrder() { }
        public PropertyOrder(string[] raw, bool pre = false) { Regexize(raw); Precede = pre; }
        public PropertyOrder(System.Text.RegularExpressions.Regex[] regex, bool pre = false) { Regex = regex; Precede = pre; }
        public PropertyOrder(bool pre = false, params string[] raw) { Regexize(raw); Precede = pre; }


        public bool RawAvailable => Raw.Length > 0;
        public string[] Raw { get; set; } = Array.Empty<string>();

        public bool RegexAvailable => Regex.Length > 0;
        public System.Text.RegularExpressions.Regex[] Regex { get; set; } = Array.Empty<System.Text.RegularExpressions.Regex>();


        public int Length
        {
            get
            {
                if (Regex.Length > 0) return Regex.Length;
                else if (Raw.Length > 0) return Raw.Length;
                else return 0;
            }
        }
        public bool Precede { get; set; } = false;


        private void Regexize(string[]? raw = null)
        {
            if (raw != null) Raw = raw;
            if (Raw.Length == 0) return;
            Regex = new System.Text.RegularExpressions.Regex[Raw.Length];
            for (int i = 0; i < Raw.Length; i++)
            {
                Regex[i] = new System.Text.RegularExpressions.Regex(Raw[i], System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
        }
    }
}
