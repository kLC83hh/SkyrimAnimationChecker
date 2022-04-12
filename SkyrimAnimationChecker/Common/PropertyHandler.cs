using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.Common
{
    public class PropertyHandler : Notify.NotifyPropertyChanged
    {
        public PropertyHandler(string[]? KeysException = null, bool? AddibleKeysException = null, object[]? KeysOrder = null, bool? RegexOrder = null)
        {
            if (KeysException != null)
            {
                if (AddibleKeysException != null && (bool)AddibleKeysException) this.PropertyHandleKeysException = this.PropertyHandleKeysException.Concat(KeysException).ToArray();
                else this.PropertyHandleKeysException = KeysException;
            }
            if (KeysOrder != null) this.KeysOrder = KeysOrder;
            if (RegexOrder != null) this.RegexOrder = (bool)RegexOrder;
            MakePropertyHandleKeys();
        }

        #region Keys
        [System.Text.Json.Serialization.JsonIgnore]
        public string[] PropertyHandleKeysException = new string[] { "Keys", "Values" };
        /// <summary>
        /// string[], Regex[] or string[] (regexpattern when RegexOrder=true)
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public object[] KeysOrder = Array.Empty<object>();
        [System.Text.Json.Serialization.JsonIgnore]
        public bool RegexOrder = false;
        [System.Text.Json.Serialization.JsonIgnore]
        public string[] Keys => _PropertyHandleKeys ?? Array.Empty<string>();

        [System.Text.Json.Serialization.JsonIgnore]
        private string[]? _PropertyHandleKeys;
        private bool CheckPropertyHandleKeyException(string propname)
        {
            foreach (string ex in PropertyHandleKeysException)
            {
                if (propname == ex) return false;
            }
            return true;
        }
        public virtual int KeysOrderComparer(string key)
        {
            for (int i = 0; i < KeysOrder.Length; i++)
            {
                if (KeysOrder[i] is string && key.Contains((string)KeysOrder[i])) return i;
                if (KeysOrder[i] is System.Text.RegularExpressions.Regex r && r.IsMatch(key)) return i;
            }
            return int.MaxValue;
        }
        private void MakePropertyHandleKeys()
        {
            var messyprops = GetType().GetProperties();
            if (messyprops.Length == 0) return;
            if (RegexOrder)
            {
                System.Text.RegularExpressions.Regex[] r = new System.Text.RegularExpressions.Regex[KeysOrder.Length];
                for (int i = 0; i < KeysOrder.Length; i++)
                {
                    if (KeysOrder[i] is string) r[i] = new System.Text.RegularExpressions.Regex((string)KeysOrder[i]);
                }
                KeysOrder = r;
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
        public object[] Values => GetPropertyHandleValues<object>();
        public virtual T[] GetPropertyHandleValues<T>()
        {
            T[] vals = new T[Keys.Length];
            for (int i = 0; i < Keys.Length; i++)
            {
                vals[i] = GetPropertyHandleValue<T>(Keys[i]);
            }
            return vals;
        }
        #endregion

        #region Property Handling
        public virtual T GetPropertyHandleValue<T>(string key) => (T)(GetType().GetProperty(key)?.GetValue(this) ?? new());
        public virtual void SetPropertyHandleValue<T>(string key, T data) => GetType().GetProperty(key)?.SetValue(this, data);
        #endregion

    }
}
