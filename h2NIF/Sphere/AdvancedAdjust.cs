using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using h2NIF.Extensions;

namespace h2NIF.Sphere
{
    internal class AdvancedAdjust : Adjust
    {
        public AdvancedAdjust(nifly.NiShape s, nifly.Vector3? o = null) : base(s) => _origin = o ?? new();

        char? _side;
        protected char? side
        {
            get
            {
                if (_side == null) _side = sphere.GetSide();
                return _side;
            }
        }
        nifly.Vector3 _origin;
        protected nifly.Vector3 origin => _origin;

        /// <param name="axis">[sxyzSXYZ] s=side sphere by name</param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <exception cref="Exception">Not sided Exception, Can not determine sphere distribution</exception>
        public void Gather(char axis, float A, float B = 0)
        {
            //string pn = axis != 's' ? sphere.GetPN(axis, origin).ToString() : "";
            //M.D($"{sphere.name.get()} {axis}{pn} {origin.x},{origin.y},{origin.z} {sphere.transform.translation.x},{sphere.transform.translation.y},{sphere.transform.translation.z} {sphere.transform.scale}");
            switch (axis)
            {
                case 's':
                case 'S':
                    if (side == 'L') Right(A, B);
                    else if (side == 'R') Left(A, B);
                    else throw new Exception("Can not gather because sphere is not sided");
                    break;
                case 'x':
                case 'X':
                    if (sphere.GetPN('x', origin) == '-') Right(A, B);
                    else if (sphere.GetPN('x', origin) == '+') Left(A, B);
                    else throw new Exception($"Can not gather because system can not determine how the sphere is distributed in {axis} axis");
                    break;
                case 'y':
                case 'Y':
                    if (sphere.GetPN('y', origin) == '-') Forward(A, B);
                    else if (sphere.GetPN('y', origin) == '+') Backward(A, B);
                    else throw new Exception($"Can not gather because system can not determine how the sphere is distributed in {axis} axis");
                    break;
                case 'z':
                case 'Z':
                    if (sphere.GetPN('z', origin) == '-') Raise(A, B);
                    else if (sphere.GetPN('z', origin) == '+') Lower(A, B);
                    else throw new Exception($"Can not gather because system can not determine how the sphere is distributed in {axis} axis");
                    break;
            }
        }
        /// <inheritdoc cref="Gather(NiShape, char, float, float)"/>
        public void Gather(char axis, double A, double B = 0) => Gather(axis, (float)A, (float)B);
        /// <inheritdoc cref="Gather(NiShape, char, float, float)"/>
        public void Spread(char axis, float A, float B = 0) => Gather(axis, -A, -B);
        /// <inheritdoc cref="Gather(NiShape, char, float, float)"/>
        public void Spread(char axis, double A, double B = 0) => Spread(axis, (float)A, (float)B);

        /// <inheritdoc cref="Gather(NiShape, char, float, float)"/>
        public void Gather(float A, float B = 0) => Gather('s', A, B);
        /// <inheritdoc cref="Gather(NiShape, char, float, float)"/>
        public void Gather(double A, double B = 0) => Gather((float)A, (float)B);
        /// <inheritdoc cref="Gather(NiShape, char, float, float)"/>
        public void Spread(float A, float B = 0) => Gather(-A, -B);
        /// <inheritdoc cref="Gather(NiShape, char, float, float)"/>
        public void Spread(double A, double B = 0) => Spread((float)A, (float)B);
    }
}