using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.CBPC
{
    internal class cbpc_breast_bbp : Common.PropertyHandler, Icbpc_breast_data
    {
        public cbpc_breast_bbp() : base(KeysIgnore: new string[] { "MirrorKeys", "Name", "DataType" })
        {
            B0 = new(0);
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public string DataType => "bbp";
        /// <summary>
        /// A short name from bone 0
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public string Name => B0.NameShort;
        public string[] MirrorKeys
        {
            get
            {
                if (B0.MirrorKeys != null) return B0.MirrorKeys;
                else throw new Exception("[Can not be happen] MirrorKeys not exists");
            }
            set
            {
                B0.MirrorKeys = value;
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
                case "LBreast": return B0.Left;
                case "RBreast": return B0.Right;
            }
            return null;
        }

        /// <summary>
        /// breast data of bone 0. bbp data has only one bone.
        /// </summary>
        public cbpc_breast B0 { get => Get<cbpc_breast>(); set => Set(value); }


        public cbpc_breast GetData(int? num = null) => B0;
        //public new cbpc_breast[] Values => GetPropertyHandleValues<cbpc_breast>();
    }
}
