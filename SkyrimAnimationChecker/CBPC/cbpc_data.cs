using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.CBPC
{
    public class cbpc_data : PropertyHandler, Icbpc_data
    {
        public cbpc_data()
               : base(KeysIgnore: new string[] { "Name", "NameShort", "Number", "DataType", "DefaultName", "UsingKeys" }) => Init();
        public cbpc_data(string name, physics_object_set? data = null)
             : base(KeysIgnore: new string[] { "Name", "NameShort", "Number", "DataType", "DefaultName", "UsingKeys" }) => Init(name, data);
        public cbpc_data(int num, physics_object_set? data = null)
             : base(KeysIgnore: new string[] { "Name", "NameShort", "Number", "DataType", "DefaultName", "UsingKeys" }) => Init(num, data);
        private void Init(object? param = null, physics_object_set? data = null)
        {
            Name = DefaultName;
            if (param != null)
            {
                if (param is string s) Name = NameParser(s);
                if (param is int n) Name = NumSideParser(n);
            }
            Data = data ?? new("");
        }

        public virtual string DataType => "single";
        protected virtual string DefaultName => "SingleData";

        protected virtual string NameParser(string name) => name;
        protected virtual string NumSideParser(int num) => DefaultName;


        public string Name { get => Get<string>(); set { Set(value); NameParser(value); } }
        public string NameShort => GetNameShort();
        /// <summary>
        /// Get a name that stripped bone number and side
        /// </summary>
        /// <param name="name">Fullname</param>
        /// <returns></returns>
        public virtual string GetNameShort(string? name = null)
        {
            string n = name ?? Name;
            if (n.EndsWith("1") || n.EndsWith("2") || n.EndsWith("3")) n = n.Substring(0, n.Length - 1);
            return n;
        }

        /// <summary>
        /// Bone number for multiple bone
        /// </summary>
        //public int Number { get => Get<int>(); set => Set(value); }


        public physics_object_set Data { get => Get<physics_object_set>(); set => Set(value); }


        public virtual physics_object_set? Find(string name) => Data;


        private string[] _UsingKeys = Array.Empty<string>();
        public string[] UsingKeys
        {
            get
            {
                if (_UsingKeys.Length > 0) return _UsingKeys;
                else { GetUsingKeys(); return _UsingKeys; }
            }
        }
        public void GetUsingKeys()
        {
            List<string> keys = new();
            foreach (var item in Data.Values)
            {
                if (item.Use && !keys.Contains(item.Key)) keys.Add(item.Key);
            }
            _UsingKeys = (from propName in keys
                          orderby KeysOrderComparer(propName, Data.KeysOrder), propName
                          select propName).ToArray();
        }

    }
}
