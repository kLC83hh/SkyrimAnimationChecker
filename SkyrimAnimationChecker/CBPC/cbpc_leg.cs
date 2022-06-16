using SkyrimAnimationChecker.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SkyrimAnimationChecker.CBPC
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    internal class cbpc_leg : PropertyHandler, Icbpc_data_multibone
    {
        public cbpc_leg() : base(KeysIgnore: new string[] { "MirrorKeys", "MirrorPairs", "Name", "NameShort", "DataType", "UsingKeys" })
        {
            FrontThigh = new("FrontThigh");
            RearThigh = new("RearThigh");
            RearCalf = new("RearCalf");
        }
        [JsonIgnore]
        public string DataType => "leg";
        /// <summary>
        /// A short name from bone 1. Short names of all 3 bones should be same.
        /// </summary>
        [JsonIgnore]
        public string Name { get => FrontThigh.NameShort; set { } }
        public string NameShort => Name;
        public string[] MirrorKeys
        {
            get
            {
                if (FrontThigh.MirrorKeys != null) return FrontThigh.MirrorKeys;
                else if (RearThigh.MirrorKeys != null) return RearThigh.MirrorKeys;
                else if (RearCalf.MirrorKeys != null) return RearCalf.MirrorKeys;
                else throw new Exception("[Can not be happen] MirrorKeys not exists");
            }
            set
            {
                FrontThigh.MirrorKeys = value;
                RearThigh.MirrorKeys = value;
                RearCalf.MirrorKeys = value;
            }
        }
        public MirrorPair[] MirrorPairs
        {
            get
            {
                if (FrontThigh.MirrorPairs != null) return FrontThigh.MirrorPairs;
                else if (RearThigh.MirrorPairs != null) return RearThigh.MirrorPairs;
                else if (RearCalf.MirrorPairs != null) return RearCalf.MirrorPairs;
                else throw new Exception("[Can not be happen] MirrorPairs not exists");
            }
            set
            {
                FrontThigh.MirrorPairs = value;
                RearThigh.MirrorPairs = value;
                RearCalf.MirrorPairs = value;
            }
        }

        /// <summary>
        /// Find a specific data set with it fullname
        /// </summary>
        /// <param name="name">Fullname e.g. ExtraBreast1L</param>
        /// <returns></returns>
        public physics_object_set? Find(string name) => name switch
        {
            "LFrontThigh" => FrontThigh.Left,
            "LRearThigh" => RearThigh.Left,
            "LRearCalf" => RearCalf.Left,
            "RFrontThigh" => FrontThigh.Right,
            "RRearThigh" => RearThigh.Right,
            "RRearCalf" => RearCalf.Right,
            _ => null,
        };

        /// <summary>
        /// Breast data for bone 1
        /// </summary>
        public cbpc_data_mirrored FrontThigh { get => Get<cbpc_data_mirrored>(); set => Set(value); }
        /// <summary>
        /// Breast data for bone 2
        /// </summary>
        public cbpc_data_mirrored RearThigh { get => Get<cbpc_data_mirrored>(); set => Set(value); }
        /// <summary>
        /// Breast data for bone 3
        /// </summary>
        public cbpc_data_mirrored RearCalf { get => Get<cbpc_data_mirrored>(); set => Set(value); }

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
                0 => FrontThigh,
                1 => RearThigh,
                2 => RearCalf,
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
            foreach (cbpc_data_mirrored b in Values)
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
                          orderby KeysOrderComparer(propName, FrontThigh.Left.KeysOrder), propName
                          select propName).ToArray();
        }
    }
}
