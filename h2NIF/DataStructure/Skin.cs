using h2NIF.Extensions;
using nifly;

namespace h2NIF.DataStructure
{
    internal class Skin
    {
        public Skin(NifFile nif, NiShape shape)
        {
            Nif = nif;
            Shape = shape;
            dismember = new SkinDismember(nif, shape);
        }
        private readonly NifFile Nif;
        public NiShape Shape { get; set; }

        public string Name => Shape.name.get();

        private readonly SkinDismember dismember;

        public bool Error => dismember.Error;
        public string Reason => dismember.Reason;

        public BSDismemberSkinInstance Instance => dismember.Instance;
        public NiSkinPartition Partition => dismember.Partition;
        public NiSkinData Data => dismember.Data;

        public int GetBones(out Bone[] bones, string[] filter) => Nif.GetFilteredBones(out bones, Shape, filter);
    }
}
