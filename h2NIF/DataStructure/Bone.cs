using h2NIF.Extensions;
using nifly;

namespace h2NIF.DataStructure
{
    public class Bone
    {
        public Bone(NifFile nif, NiShape skin, int index, uint id, string name)
        {
            Nif = nif;
            Skin = skin;

            Index = index;
            Id = id;
            Name = name;

            GetVertices();

        }

        protected NifFile Nif;
        protected NiShape Skin;
        protected BSDismemberSkinInstance SkinInstance => Nif.GetBlock<BSDismemberSkinInstance>(Skin.SkinInstanceRef()) ?? new();
        protected NiSkinPartition SkinPartition => Nif.GetBlock<NiSkinPartition>(SkinInstance.skinPartitionRef) ?? new();
        protected NiSkinData SkinData => Nif.GetBlock<NiSkinData>(SkinInstance.dataRef) ?? new();

        public int Index;
        public uint Id;
        public string Name;

        public NiNode Node => Nif.GetNode(Name + " Location") ?? Nif.GetRootNode();
        public string NodeName => Node?.name.get() ?? string.Empty;

        public int SpheresCount => Spheres.Length;
        public BSTriShape[] Spheres => Nif.GetNamedTriShapes(Name, "Sphere");

        public BSTriShape? Sphere => SpheresCount == 0 ? null : Spheres[0];
        public string SphereName => Sphere?.name.get() ?? string.Empty;

        public MatTransform Transform => SkinData.bones[Index].boneTransform;
        public BoundingSphere Bound => SkinData.bones[Index].bounds;
        public int WeightsCount => Weights.Count;
        public vectorSkinWeight Weights => SkinData.bones[Index].vertexWeights;
        //public SkinWeight GetWeight(Vector3 vertex) => Weights[_Vertices.IndexOf(vertex)];

        public int VerticesCount => _Vertices.Count;
        public Vertex[] Vertices => _Vertices.ToArray();
        public vectorVector3 VerticesVector
        {
            get
            {
                vectorVector3 buffer = new();
                //foreach (Vector3 v in _Vertices) buffer.Add(v);
                foreach (Vertex v in _Vertices) buffer.Add(v.vert);
                return buffer;
            }
        }
        private readonly List<Vertex> _Vertices = new();
        private void GetVertices()
        {
            foreach (SkinWeight sw in Weights)
            {
                _Vertices.Add(new Vertex(sw.index, SkinPartition.vertData[sw.index].vert, sw.weight));
                //_Vertices.Add(SkinPartition.vertData[sw.index].vert);
            }
        }

        public vectorTriangle Triangles => SkinPartition.partitions[0].triangles;
    }
}
