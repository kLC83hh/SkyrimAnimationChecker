using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nifly;
using h2NIF.DataStructure;
using h2NIF.Extensions;

namespace h2NIF.Sphere
{
    internal class UpdateParam
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nif">nif file</param>
        /// <param name="sphere">a sphere from bone</param>
        /// <param name="vertices">whole vertices from bone</param>
        /// <param name="strength"></param>
        /// <param name="wSens">window parameter</param>
        /// <param name="wBase">window parameter</param>
        /// <param name="axes">vertice split parameter</param>
        /// <param name="i">vertice split parameter</param>
        /// <param name="j">vertice split parameter</param>
        public UpdateParam(NifFile nif, NiShape sphere, Vertex[] vertices, string strength, string wSens, string wBase, bool weight, char[]? axes, params int[][] sector)
        {
            Nif = nif;
            Sphere = sphere;
            _Vertices = vertices;
            RawStrength = strength;
            RawWindowSensitivity = wSens;
            WindowBase = wBase;
            Weight = weight;
            SplitAxes = axes;
            Sector = sector;
        }

        NifFile Nif;
        public NiShape Sphere { get; set; }
        /// <summary>
        /// <c>Sphere</c>.name.get()
        /// </summary>
        public string Name => Sphere.name.get();
        public NiNode ParentNode => Nif.GetParentNode(Sphere);

        Vertex[] _Vertices;
        Vertex[] SplitVertices()
        {
            //M.D($"{Name} {SplitAxes?.Length}");
            if (SplitAxes?.Length == 1)
            {
                if (Sector.Any(x => x.Length != 1)) throw new ArgumentException();
                Vertex[][] result = _Vertices.AxisSplit(SplitAxes[0], parent: Nif.GetParentNode(Sphere).transform);
                Vertex[] output = Array.Empty<Vertex>();
                foreach (int[] s in Sector) output = output.Concat(result[s[0]]).ToArray();
                return output;
            }
            if (SplitAxes?.Length == 2)
            {
                if (Sector.Any(x => x.Length != 2)) throw new ArgumentException();
                Vertex[,][] result = _Vertices.QuadrantSplit(SplitAxes, Nif.GetParentNode(Sphere).transform);
                Vertex[] output = Array.Empty<Vertex>();
                foreach (int[] s in Sector)
                {
                    output = output.Concat(result[s[0], s[1]]).ToArray();
                    //M.D($"{Name} {s[0]},{s[1]} {result[s[0], s[1]].Length}");
                }
                //M.D($"{Name} {output.Length}");
                return output;
            }
            if (SplitAxes?.Length == 3)
            {
                Vertex[][] result = _Vertices.LinearSplit(SplitAxes[0], Nif.GetParentNode(Sphere).transform);
                Vertex[] output = Array.Empty<Vertex>();
                foreach (int[] s in Sector) output = output.Concat(result[s[0]]).ToArray();
                return output;
            }
            return _Vertices;
        }
        /// <summary>
        /// automatically split vertices or original vertices if there are no split params such as <c>axes</c>, <c>i</c>, or <c>j</c>.
        /// </summary>
        public Vertex[] Vertices => SplitVertices();

        char[]? SplitAxes = null;
        int[][] Sector;

        public string RawStrength { get; set; }
        public Vector3 Strength => Vectorize(RawStrength, 1f);
        public string RawWindowSensitivity { get; set; }
        public Vector3 WindowSensitivity => Vectorize(RawWindowSensitivity, 1f);
        /// <summary>
        /// min, max, center, default is center
        /// </summary>
        public string WindowBase { get; set; } = "center";

        public bool Weight;

        //public Action<NiShape> Adjust => AdjustManager.Get(Name);
        public Action Adjust => AdjustManager.Create(Name, Sphere, ParentNode);


        private Vector3 Vectorize(string s, Vector3? defaultVector = null)
        {
            if (!s.Contains(',')) return defaultVector ?? new();
            string[] vals = s.Split(',');
            if (vals.Length != 3) return defaultVector ?? new();
            try
            {
                float x = float.Parse(vals[0]);
                float y = float.Parse(vals[1]);
                float z = float.Parse(vals[2]);
                return new(x, y, z);
            }
            catch { return defaultVector ?? new(); }
        }
        private Vector3 Vectorize(string s, params float[] defaultValue)
        {
            if (defaultValue.Length == 1) return Vectorize(s, new Vector3(defaultValue[0], defaultValue[0], defaultValue[0]));
            else if (defaultValue.Length == 3) return Vectorize(s, new Vector3(defaultValue[0], defaultValue[1], defaultValue[2]));
            else return Vectorize(s, new Vector3());
        }

    }
}
