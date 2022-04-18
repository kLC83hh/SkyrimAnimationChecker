using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SkyrimAnimationChecker.CBPC
{
    public class cbpc_breast_3ba : Common.PropertyHandler, Icbpc_breast_data
    {
        public cbpc_breast_3ba() : base(KeysIgnore: new string[] { "MirrorKeys", "MirrorPairs", "Name", "NameShort", "DataType" })
        {
            B1 = new(1);
            B2 = new(2);
            B3 = new(3);
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public string DataType => "3ba";
        /// <summary>
        /// A short name from bone 1. Short names of all 3 bones should be same.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
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
        public Common.physics_object_set? Find(string name)
        {
            switch (name)
            {
                case "ExtraBreast1L": return B1.Left;
                case "ExtraBreast2L": return B2.Left;
                case "ExtraBreast3L": return B3.Left;
                case "ExtraBreast1R": return B1.Right;
                case "ExtraBreast2R": return B2.Right;
                case "ExtraBreast3R": return B3.Right;
            }
            return null;
        }

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
        public Icbpc_data GetData(int? num = null)
        {
            if (num == null) throw new ArgumentNullException(nameof(num));
            switch (num)
            {
                case 1: return B1;
                case 2: return B2;
                case 3: return B3;
            }
            throw new ArgumentNullException(nameof(num));
        }

    }

}
