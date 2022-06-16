using h2NIF.Extensions;
using nifly;

namespace h2NIF.DataStructure
{
    public class Vertex : Vector4
    {
        public Vertex() { Index = 0; vert = new(); weight = 1; }
        public Vertex(int index, Vector3 v, float weight)
        {
            Index = index;
            vert = v;
            this.weight = weight;
        }
        public int Index { get; set; }
        private Vector3 _vert = new();
#pragma warning disable IDE1006 // Naming Styles
        public Vector3 vert
        {
            get => _vert; set
            {
                Vector3 buffer = new()
                {
                    x = value.x,
                    y = value.y,
                    z = value.z
                };
                _vert = buffer;
            }
        }
        public float weight { get; set; }
        public new float x { get => vert.x; set => vert.x = value; }
        public new float y { get => vert.y; set => vert.y = value; }
        public new float z { get => vert.z; set => vert.z = value; }
        public new float w { get => weight; set => weight = value; }
#pragma warning restore IDE1006 // Naming Styles

        public Vertex GlobalizeTo(Vector3 parent) => new(Index, vert.opAdd(parent), weight);
        public Vertex LocalizeTo(Vector3 parent) => new(Index, vert.opSub(parent), weight);
        public Vertex RotateBy(Matrix3 rotation) => new(Index, rotation.opMult(vert), weight);
        public float DistanceTo(Vertex target) => vert.DistanceTo(target.vert);
        public Vector3 Midpoint(Vertex target) => vert.Midpoint(target.vert);
    }
}
