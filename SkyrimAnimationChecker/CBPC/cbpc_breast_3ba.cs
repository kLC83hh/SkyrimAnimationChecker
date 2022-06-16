using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SkyrimAnimationChecker.CBPC
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class cbpc_breast_3ba : Common.PropertyHandler, Icbpc_breast_data
    {
        public cbpc_breast_3ba() : base(KeysIgnore: new string[] { "MirrorKeys", "MirrorPairs", "Name", "NameShort", "DataType", "UsingKeys" })
        {
            B1 = new(1);
            B2 = new(2);
            B3 = new(3);
        }
        [JsonIgnore]
        public string DataType => "3ba";
        /// <summary>
        /// A short name from bone 1. Short names of all 3 bones should be same.
        /// </summary>
        [JsonIgnore]
        public string Name { get => B1.NameShort; set { } }
        public string NameShort => Name;
        public string[] MirrorKeys
        {
            get
            {
                if (B1.MirrorKeys != null) return B1.MirrorKeys;
                else if (B2.MirrorKeys != null) return B2.MirrorKeys;
                else if (B3.MirrorKeys != null) return B3.MirrorKeys;
                else throw new Exception("[Can not be happen] MirrorKeys not exists");
            }
            set
            {
                B1.MirrorKeys = value;
                B2.MirrorKeys = value;
                B3.MirrorKeys = value;
            }
        }
        public MirrorPair[] MirrorPairs
        {
            get
            {
                if (B1.MirrorPairs != null) return B1.MirrorPairs;
                else if (B2.MirrorPairs != null) return B2.MirrorPairs;
                else if (B3.MirrorPairs != null) return B3.MirrorPairs;
                else throw new Exception("[Can not be happen] MirrorPairs not exists");
            }
            set
            {
                B1.MirrorPairs = value;
                B2.MirrorPairs = value;
                B3.MirrorPairs = value;
            }
        }

        /// <summary>
        /// Find a specific data set with it fullname
        /// </summary>
        /// <param name="name">Fullname e.g. ExtraBreast1L</param>
        /// <returns></returns>
        public Common.physics_object_set? Find(string name) => name switch
        {
            "ExtraBreast1L" => B1.Left,
            "ExtraBreast2L" => B2.Left,
            "ExtraBreast3L" => B3.Left,
            "ExtraBreast1R" => B1.Right,
            "ExtraBreast2R" => B2.Right,
            "ExtraBreast3R" => B3.Right,
            _ => null,
        };

        /// <summary>
        /// Breast data for bone 1
        /// </summary>
        public cbpc_breast B1 { get => Get<cbpc_breast>(); set => Set(value); }
        /// <summary>
        /// Breast data for bone 2
        /// </summary>
        public cbpc_breast B2 { get => Get<cbpc_breast>(); set => Set(value); }
        /// <summary>
        /// Breast data for bone 3
        /// </summary>
        public cbpc_breast B3 { get => Get<cbpc_breast>(); set => Set(value); }

        /// <summary>
        /// Get breast datas for all 3 bones
        /// </summary>
        public new cbpc_breast[] Values => PropertyHandleGetValues<cbpc_breast>();

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
                1 => B1,
                2 => B2,
                3 => B3,
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
            foreach (cbpc_breast b in Values)
            {
                foreach (var item in b.Left.Values)
                {
                    if (item.Use && !keys.Contains(item.Key)) keys.Add(item.Key);
                }
                foreach (var item in b.Right.Values)
                {
                    if (item.Use && !keys.Contains(item.Key)) keys.Add(item.Key);
                }
            }
            _UsingKeys = (from propName in keys
                          orderby KeysOrderComparer(propName, B1.Left.KeysOrder), propName
                          select propName).ToArray();
        }
        //public (Common.physics_object[] Left, Common.physics_object[] Right) GetUsingValues(string name)
        //{
        //    if (!Keys.Contains(name)) return (Array.Empty<Common.physics_object>(), Array.Empty<Common.physics_object>());
        //    var buffer = PropertyHandleGetValue<cbpc_breast>(name);
        //    List<Common.physics_object> left = new(), right = new();
        //    foreach (var item in buffer.Left.Values)
        //    {
        //        if (UsingKeys.Contains(item.Key)) left.Add(item);
        //    }
        //    foreach (var item in buffer.Right.Values)
        //    {
        //        if (UsingKeys.Contains(item.Key)) right.Add(item);
        //    }
        //    return (left.ToArray(), right.ToArray());
        //}
    }

}
