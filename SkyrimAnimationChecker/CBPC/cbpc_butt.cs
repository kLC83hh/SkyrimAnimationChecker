using SkyrimAnimationChecker.Common;
using System.Text.Json.Serialization;

namespace SkyrimAnimationChecker.CBPC
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class cbpc_butt : cbpc_data_mirrored, Icbpc_data_mirrored
    {
        public cbpc_butt() : base() { }
        public cbpc_butt(string name, physics_object_set? left = null, physics_object_set? right = null) : base(name, left, right) { }
        public cbpc_butt(int num, physics_object_set? left = null, physics_object_set? right = null) : base(num, left, right) { }


        [JsonIgnore]
        public override string DataType => "butt";
        [JsonIgnore]
        protected override string DefaultName => "Butt";


        public override physics_object_set? Find(string name) => name switch
        {
            "LButt" => Left,
            "RButt" => Right,
            _ => null,
        };

    }
}
