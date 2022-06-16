using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.CBPC
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class cbpc_breast_bbp : cbpc_data_mirrored, Icbpc_data_mirrored
    {

        public cbpc_breast_bbp() : base() { }
        public cbpc_breast_bbp(string name, physics_object_set? left = null, physics_object_set? right = null) : base(name, left, right) { }
        public cbpc_breast_bbp(int num, physics_object_set? left = null, physics_object_set? right = null) : base(num, left, right) { }


        [System.Text.Json.Serialization.JsonIgnore]
        public override string DataType => "bbp";
        [System.Text.Json.Serialization.JsonIgnore]
        protected override string DefaultName => "Breast";

        /// <summary>
        /// Find a specific data set with it fullname
        /// </summary>
        /// <param name="name">Fullname e.g. ExtraBreast1L</param>
        /// <returns></returns>
        public override physics_object_set? Find(string name) => name switch
        {
            "LBreast" => Left,
            "RBreast" => Right,
            _ => null,
        };


        protected override string NameParser(string name)
        {
            //if (name.Contains('1')) Number = 1;
            //else if (name.Contains('2')) Number = 2;
            //else if (name.Contains('3')) Number = 3;
            //else Number = 0;
            //else throw EE.New(2902);

            //if (name.StartsWith('L') || name.EndsWith('L')) Side = Sides.L;
            //else if (name.StartsWith('R') || name.EndsWith('R')) Side = Sides.R;
            //else throw EE.New(2901);

            return name;
        }
        protected override string NumSideParser(int num = 0)
        {
            if (num == 0) return "Breast";
            //if (num == 1 || num == 2 || num == 3)
            //{
            //    return $"ExtraBreast{num}";
            //}
            throw EE.New(2902);
        }
    }
}
