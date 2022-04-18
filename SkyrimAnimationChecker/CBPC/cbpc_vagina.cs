using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.CBPC
{
    public class cbpc_vagina : PropertyHandler, Icbpc_data_multibone
    {
        public cbpc_vagina() : base(KeysIgnore: new string[] { "MirrorKeys", "MirrorPairs", "Name", "NameShort", "DataType" })
        {
            Vagina = new("Vagina");
            Clit = new("Clit");
            Labia = new("Labia");
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public string DataType => "vagina";
        /// <summary>
        /// A short name from bone 1. Short names of all 3 bones should be same.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
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
        public physics_object_set? Find(string name)
        {
            switch (name)
            {
                case "Vagina": return Vagina.Data;
                case "VaginaB": return Vagina.Data;
                case "Clit": return Clit.Data;
                case "LLabia": return Labia.Left;
                case "RLabia": return Labia.Right;
            }
            return null;
        }

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
        public Icbpc_data GetData(int? num = null)
        {
            if (num == null) throw new ArgumentNullException(nameof(num));
            switch (num)
            {
                case 0: return Vagina;
                case 1: return Clit;
                case 2: return Labia;
            }
            throw new ArgumentNullException(nameof(num));
        }

    }
}
