using nifly;

namespace h2NIF.Sphere
{
    internal class UpdateResult
    {
        public UpdateResult(string n, Vector3 c, float r, string m) { Name = n; Center = c; Radius = r; Message = m; }
        public UpdateResult(NiShape sphere, string m)
        {
            Name = sphere.name.get();
            Center = sphere.transform.translation;
            Radius = sphere.transform.scale;
            Message = m;
        }
        public string Name { get; set; }
        public Vector3 Center { get; set; }
        public float Radius { get; set; }
        public string Message { get; set; }

        public override string ToString() => $"{Name} Succeed center=({Center.x:N3},{Center.y:N3},{Center.z:N3}), r={Radius:N3}\n{Message}";
    }
}
