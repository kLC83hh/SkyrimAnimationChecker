using SkyrimAnimationChecker.Common;
using System.Text.Json.Serialization;

namespace SkyrimAnimationChecker.CBPC
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class cbpc_belly : cbpc_data, Icbpc_data
    {
        public cbpc_belly() : base() { }
        public cbpc_belly(string name, physics_object_set? data = null) : base(name, data) { }
        public cbpc_belly(int num, physics_object_set? data = null) : base(num, data) { }


        [JsonIgnore]
        public override string DataType => "belly";
        [JsonIgnore]
        protected override string DefaultName => "NPCBelly";

        public override physics_object_set? Find(string name) => Data;

        protected override string NameParser(string name) => name;
        protected override string NumSideParser(int num = 0)
        {
            if (num == 0) return "NPCBelly";
            //if (num == 1 || num == 2 || num == 3)
            //{
            //    return $"ExtraBreast{num}";
            //}
            throw EE.New(2902);
        }



    }
}
