using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nifly;
using h2NIF.Extensions;

namespace h2NIF.Sphere
{
    internal class CapsuleAdjust
    {
        //public CapsuleAdjust(NiShape[] spheres) => Spheres = spheres;
        public CapsuleAdjust(DataStructure.Bone bone) => Bone = bone;
        public DataStructure.Bone Bone { get; set; }

        private NiShape[]? _Spheres;
        public NiShape[] Spheres
        {
            get
            {
                if (_Spheres == null)
                {
                    List<NiShape> output = new();
                    foreach (NiShape sphere in Bone.Spheres)
                    {
                        if (sphere.transform.scale > 0) output.Add(sphere);
                    }
                    _Spheres = output.ToArray();
                }
                return _Spheres;
            }
        }
        public string Name
        {
            get
            {
                if (Spheres.Length == 0) return string.Empty;
                System.Text.RegularExpressions.Regex regex = new(@" Sphere\d*$");
                return regex.Replace(Spheres[0].name.get(), string.Empty);
            }
        }
        private Vector3? _Center;
        public Vector3 Center
        {
            get
            {
                if (_Center == null)
                {
                    if (Spheres.Length == 2) _Center = Spheres[0].transform.translation.Midpoint(Spheres[1].transform.translation);
                    else _Center = new();
                }
                return _Center;
            }
        }
        private float? _Distance;
        public float Distance
        {
            get
            {
                if (_Distance == null)
                {
                    if (Spheres.Length == 2) _Distance = Spheres[0].transform.translation.DistanceTo(Spheres[1].transform.translation);
                    else _Distance = 0f;
                }
                return (float)_Distance;
            }
        }


        /// <inheritdoc cref="AdvancedAdjust.Gather(float, float)"/>
        public void Gather(float A, float B = 0) => Gather('s', A, B);
        /// <inheritdoc cref="Gather(float, float)"/>
        public void Gather(double A, double B = 0) => Gather((float)A, (float)B);
        /// <inheritdoc cref="Gather(float, float)"/>
        public void Spread(float A, float B = 0) => Gather(-A, -B);
        /// <inheritdoc cref="Gather(float, float)"/>
        public void Spread(double A, double B = 0) => Spread((float)A, (float)B);
        /// <inheritdoc cref="AdvancedAdjust.Gather(char, float, float)"/>
        public void Gather(char axis, float A, float B = 0)
        {
            //M.D($"{Name} c {Center.x},{Center.y},{Center.z}");
            if (Spheres.Length == 2)
            {
                for (int i = 0; i < Spheres.Length; i++)
                {
                    //M.D($"{Name} {i} {Spheres[i].transform.translation.x},{Spheres[i].transform.translation.y},{Spheres[i].transform.translation.z}");
                    new AdvancedAdjust(Spheres[i], Center).Gather(axis, A, B);
                }
            }
        }
        /// <inheritdoc cref="Gather(char, float, float)"/>
        public void Gather(char axis, double A, double B = 0) => Gather(axis, (float)A, (float)B);
        /// <inheritdoc cref="Gather(char, float, float)"/>
        public void Spread(char axis, float A, float B = 0) => Gather(axis, -A, -B);
        /// <inheritdoc cref="Gather(char, float, float)"/>
        public void Spread(char axis, double A, double B = 0) => Spread(axis, (float)A, (float)B);

        public void Right(float A, float B = 0)
        {
            foreach(var sphere in Spheres) new Adjust(sphere).Right(A, B);
        }
        public void Right(double A, double B = 0) => Right((float)A, (float)B);
        public void Left(float A, float B = 0) => Right(-A, -B);
        public void Left(double A, double B = 0) => Left((float)A, (float)B);
        public void Forward(float A, float B = 0)
        {
            foreach(var sphere in Spheres) new Adjust(sphere).Forward(A, B);
        }
        public void Forward(double A, double B = 0) => Forward((float)A, (float)B);
        public void Backward(float A, float B = 0) => Forward(-A, -B);
        public void Backward(double A, double B = 0) => Backward((float)A, (float)B);
        public void Raise(float A, float B = 0)
        {
            foreach (var sphere in Spheres) new Adjust(sphere).Raise(A, B);
        }
        public void Raise(double A, double B = 0) => Raise((float)A, (float)B);
        public void Lower(float A, float B = 0) => Raise(-A, -B);
        public void Lower(double A, double B = 0) => Lower((float)A, (float)B);
        public void ScaleUp(float A, float B = 0)
        {
            foreach (var sphere in Spheres) new Adjust(sphere).ScaleUp(A, B);
        }
        public void ScaleUp(double A, double B = 0) => ScaleUp((float)A, (float)B);
        public void ScaleDown(float A, float B = 0) => ScaleUp(-A, -B);
        public void ScaleDown(double A, double B = 0) => ScaleDown((float)A, (float)B);
    }
}
