using SkyrimAnimationChecker.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SkyrimAnimationChecker.CBPC
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class cbpc_vagina : PropertyHandler, Icbpc_data_multibone
    {
        public cbpc_vagina() : base(KeysIgnore: new string[] { "MirrorKeys", "MirrorPairs", "Name", "NameShort", "DataType", "UsingKeys" })
        {
            Vagina = new("Vagina");
            Clit = new("Clit");
            Labia = new("Labia");
        }
        [JsonIgnore]
        public string DataType => "vagina";
        /// <summary>
        /// A short name from bone 1. Short names of all 3 bones should be same.
        /// </summary>
        [JsonIgnore]
        public string Name { get => Vagina.NameShort; set { } }
        public string NameShort => Name;
        public string[] MirrorKeys
        {
            get
            {
                if (Labia.MirrorKeys != null) return Labia.MirrorKeys;
                else throw new Exception("[Can not be happen] MirrorKeys not exists");
            }
            set
            {
                Labia.MirrorKeys = value;
            }
        }
        public MirrorPair[] MirrorPairs
        {
            get
            {
                if (Labia.MirrorPairs != null) return Labia.MirrorPairs;
                else throw new Exception("[Can not be happen] MirrorPairs not exists");
            }
            set
            {
                Labia.MirrorPairs = value;
            }
        }

        /// <summary>
        /// Find a specific data set with it fullname
        /// </summary>
        /// <param name="name">Fullname e.g. ExtraBreast1L</param>
        /// <returns></returns>
        public physics_object_set? Find(string name) => name switch
        {
            "Vagina" => Vagina.Data,
            "VaginaB" => Vagina.Data,
            "Clit" => Clit.Data,
            "LLabia" => Labia.Left,
            "RLabia" => Labia.Right,
            _ => null,
        };

        /// <summary>
        /// Breast data for bone 1
        /// </summary>
        public cbpc_data Vagina { get => Get<cbpc_data>(); set => Set(value); }
        /// <summary>
        /// Breast data for bone 2
        /// </summary>
        public cbpc_data Clit { get => Get<cbpc_data>(); set => Set(value); }
        /// <summary>
        /// Breast data for bone 3
        /// </summary>
        public cbpc_data_mirrored Labia { get => Get<cbpc_data_mirrored>(); set => Set(value); }

        /// <summary>
        /// Get breast datas for all 3 bones
        /// </summary>
        public new Icbpc_data[] Values => PropertyHandleGetValues<Icbpc_data>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num">1,2,3</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Icbpc_data GetData(int? num = null) => num switch
        {
            null => throw new ArgumentNullException(nameof(num)),
            _ => num switch
            {
                0 => Vagina,
                1 => Clit,
                2 => Labia,
                _ => throw new ArgumentException(null, nameof(num)),
            }
        };


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

            foreach (var item in Vagina.Data.Values)
            {
                if (item.Use && !keys.Contains(item.Key)) keys.Add(item.Key);
            }
            foreach (var item in Clit.Data.Values)
            {
                if (item.Use && !keys.Contains(item.Key)) keys.Add(item.Key);
            }
            foreach (var item in Labia.Left.Values)
            {
                if (item.Use && !keys.Contains(item.Key)) keys.Add(item.Key);
            }
            foreach (var item in Labia.Right.Values)
            {
                if (item.Use && !keys.Contains(item.Key)) keys.Add(item.Key);
            }
            _UsingKeys = (from propName in keys
                          orderby KeysOrderComparer(propName, Vagina.Data.KeysOrder), propName
                          select propName).ToArray();
        }
    }
}
