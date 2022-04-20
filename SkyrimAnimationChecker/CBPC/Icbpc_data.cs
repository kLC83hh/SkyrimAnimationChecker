using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.CBPC
{
    public interface Icbpc_data : Common.IPropertyHandler
    {
        public virtual string DataType => "single";

        /// <summary>
        /// Fullname
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A name that stripped bone number and side
        /// </summary>
        public string NameShort { get; }

        public Common.physics_object_set? Find(string name);
    }
    public interface Icbpc_data_mirrored : Icbpc_data
    {
        public string[] MirrorKeys { get; set; }
        public MirrorPair[] MirrorPairs { get; set; }
    }
    public interface Icbpc_data_multibone: Icbpc_data_mirrored
    {
        public abstract Icbpc_data GetData(int? num = null);
        public string[] UsingKeys { get; }
    }
    public interface Icbpc_breast_data : Icbpc_data_multibone
    {
        //public abstract Icbpc_data_mirrored GetData(int? num = null);
        //public (Common.physics_object[] Left, Common.physics_object[] Right) GetUsingValues(string name);
    }
}
