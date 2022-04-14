using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.CBPC
{
    public class cbpc_butt : cbpc_data_mirrored, Icbpc_data_mirrored
    {
        public cbpc_butt() : base() { }
        public cbpc_butt(string name, physics_object_set? left = null, physics_object_set? right = null) : base(name, left, right) { }
        public cbpc_butt(int num, physics_object_set? left = null, physics_object_set? right = null) : base(num, left, right) { }


        [System.Text.Json.Serialization.JsonIgnore]
        public override string DataType => "butt";
        [System.Text.Json.Serialization.JsonIgnore]
        protected override string DefaultName => "Butt";


        public override physics_object_set? Find(string name)
        {
            switch (name)
            {
                case "LButt": return Left;
                case "RButt": return Right;
            }
            return null;
        }

    }
}
