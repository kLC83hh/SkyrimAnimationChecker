using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.CBPC
{
    internal class cbpc_leg : PropertyHandler, Icbpc_data_multibone
    {
        public cbpc_leg() : base(KeysIgnore: new string[] { "MirrorKeys", "MirrorPairs", "Name", "NameShort", "DataType" })
        {
            FrontThigh = new("FrontThigh");
            RearThigh = new("RearThigh");
            RearCalf = new("RearCalf");
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public string DataType => "leg";
        /// <summary>
        /// A short name from bone 1. Short names of all 3 bones should be same.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
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
        public physics_object_set? Find(string name)
        {
            switch (name)
            {
                case "LFrontThigh": return FrontThigh.Left;
                case "LRearThigh": return RearThigh.Left;
                case "LRearCalf": return RearCalf.Left;
                case "RFrontThigh": return FrontThigh.Right;
                case "RRearThigh": return RearThigh.Right;
                case "RRearCalf": return RearCalf.Right;
            }
            return null;
        }

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
        public Icbpc_data GetData(int? num = null)
        {
            if (num == null) throw new ArgumentNullException(nameof(num));
            switch (num)
            {
                case 0: return FrontThigh;
                case 1: return RearThigh;
                case 2: return RearCalf;
            }
            throw new ArgumentNullException(nameof(num));
        }

    }
}
