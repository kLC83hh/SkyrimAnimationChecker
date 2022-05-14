using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nifly;
using h2NIF.DataStructure;

namespace h2NIF.Extensions
{
    internal static class MathExtensions
    {
        public static Vector3 Width(this Vector3[] arr, bool symetric = true)
        {
            if (symetric)
            {
                Vector3 max = new();
                foreach (Vector3 v in arr)
                {
                    max.x = Math.Max(max.x, Math.Abs(v.x));
                    max.y = Math.Max(max.y, Math.Abs(v.y));
                    max.z = Math.Max(max.z, Math.Abs(v.z));
                }
                return max;
            }
            else
            {
                Vector3 min = new(), max = new();
                foreach (Vector3 v in arr)
                {
                    min.x = Math.Min(min.x, v.x);
                    min.y = Math.Min(min.y, v.y);
                    min.z = Math.Min(min.z, v.z);
                    max.x = Math.Max(max.x, v.x);
                    max.y = Math.Max(max.y, v.y);
                    max.z = Math.Max(max.z, v.z);
                }
                return new Vector3(Math.Abs(max.x - min.x), Math.Abs(max.y - min.y), Math.Abs(max.z - min.z));
            }
        }
        public static SkinWeight[] Normalize(this SkinWeight[] weights)
        {
            float max = 0;
            foreach (SkinWeight weight in weights) max = Math.Max(max, weight.weight);
            SkinWeight[] newWeights = new SkinWeight[weights.Length];
            for (int i = 0; i < weights.Length; i++)
            {
                newWeights[i] = new();
                newWeights[i].weight = weights[i].weight / max;
            }
            return newWeights;
        }
        public static float AverageDistanceTo(this Vector3[] arr, Vector3 target, SkinWeight[]? weights = null, bool normalize = true)
        {
            if (weights != null && arr.Length != weights.Length) throw new ArgumentException($"Argument lengths are different {arr.Length}, {weights.Length}");

            Vertex[] vertices = new Vertex[arr.Length];
            for (var i = 0; i < arr.Length; i++)
            {
                vertices[i] = new Vertex(i, arr[i], weights != null ? weights[i].weight : 1f);
            }
            return AverageDistanceTo(vertices, target, weights != null, normalize);
        }


        public static Vector3 Width(this Vertex[] arr)
        {
            (Vertex max, Vertex min) = arr.MaxMin();
            return max.vert.opSub(min.vert);
        }
        public static Vector3 Width(this Vertex[] arr, bool symetric)
        {
            if (symetric)
            {
                Vector3 max = new();
                foreach (Vertex v in arr)
                {
                    max.x = Math.Max(max.x, Math.Abs(v.x));
                    max.y = Math.Max(max.y, Math.Abs(v.y));
                    max.z = Math.Max(max.z, Math.Abs(v.z));
                }
                return max;
            }
            else
            {
                Vector3 min = new(), max = new();
                foreach (Vertex v in arr)
                {
                    min.x = Math.Min(min.x, v.x);
                    min.y = Math.Min(min.y, v.y);
                    min.z = Math.Min(min.z, v.z);
                    max.x = Math.Max(max.x, v.x);
                    max.y = Math.Max(max.y, v.y);
                    max.z = Math.Max(max.z, v.z);
                }
                return new Vector3(Math.Abs(max.x - min.x), Math.Abs(max.y - min.y), Math.Abs(max.z - min.z));
            }
        }
        //Parellelized
        public static Vertex[] Normalize(this Vertex[] arr)
        {
            float max = 0;
            foreach (Vertex v in arr) max = Math.Max(max, v.weight);
            Vertex[] newArr = new Vertex[arr.Length];
            Parallel.For(0, arr.Length, i =>
            {
                newArr[i] = new(arr[i].Index, arr[i].vert, arr[i].weight / max);
            });
            return newArr;
        }
        public static float AverageDistanceTo(this Vertex[] arr, Vector3 target, bool weight = true, bool normalize = true)
        {
            var a = (weight && normalize) ? arr.Normalize() : arr;

            float sum = 0, wsum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                sum += a[i].vert.DistanceTo(target) * (weight ? a[i].w : 1);
                wsum += (weight ? a[i].w : 1);
            }
            return sum / wsum;
        }

        public static (Vertex max, Vertex min) MaxMin(this Vertex[] array, Matrix3? rotation = null)
        {
            Vertex[] arr = array.Deepcopy();
            if (rotation != null) arr = arr.RotateBy(rotation.Transpose());

            Vertex max = new(), min = new();

            max.x = arr.MaxBy(v => v.x)?.x ?? 0f;
            max.y = arr.MaxBy(v => v.y)?.y ?? 0f;
            max.z = arr.MaxBy(v => v.z)?.z ?? 0f;
            max.w = arr.MaxBy(v => v.w)?.w ?? 0f;

            min.x = arr.MinBy(v => v.x)?.x ?? 0f;
            min.y = arr.MinBy(v => v.y)?.y ?? 0f;
            min.z = arr.MinBy(v => v.z)?.z ?? 0f;
            min.w = arr.MinBy(v => v.w)?.w ?? 0f;

            return (max, min);
        }
        public static (Vertex max, Vertex min) MaxMin(this Vertex[] array, char axis, Matrix3? rotation = null)
        {
            Vertex[] arr = array.Deepcopy();
            if (rotation != null) arr = arr.RotateBy(rotation.Transpose());

            if (arr.Length == 1) return (arr[0], arr[0]);
            else if (arr.Length > 1)
            {
                var total = arr.MaxMin();
                Vector3 center = total.max.Midpoint(total.min);
                Vertex max = new(0, center, 1), min = new(0, center, 1);
                switch (axis)
                {
                    case 'x':
                    case 'X':
                        max.x = arr.MaxBy(v => v.x)?.x ?? 0f;
                        min.x = arr.MinBy(v => v.x)?.x ?? 0f;
                        return (max, min);
                    case 'y':
                    case 'Y':
                        max.y = arr.MaxBy(v => v.y)?.y ?? 0f;
                        min.y = arr.MinBy(v => v.y)?.y ?? 0f;
                        return (max, min);
                    case 'z':
                    case 'Z':
                        max.z = arr.MaxBy(v => v.z)?.z ?? 0f;
                        min.z = arr.MinBy(v => v.z)?.z ?? 0f;
                        return (max, min);
                    case 'w':
                    case 'W':
                        max.w = arr.MaxBy(v => v.w)?.w ?? 0f;
                        min.w = arr.MinBy(v => v.w)?.w ?? 0f;
                        return (max, min);
                }
            }
            return (new(), new());
        }

        /// <summary>
        /// Parellelized.
        /// direction 0=+, 1=-
        /// </summary>
        /// <param name="axis">[xyz] must have length of 1</param>
        /// <param name="center">split base</param>
        /// <param name="parent">parent transform matrix</param>
        /// <returns></returns>
        public static Vertex[][] AxisSplit(this Vertex[] array, char axis, Vector3? center = null, MatTransform? parent = null)
        {
            Vertex[] arr = parent != null ? array.RotateBy(parent.rotation.Transpose()) : array.Deepcopy();
            if (center == null)
            {
                var total = arr.MaxMin();
                center = total.max.Midpoint(total.min);
            }
            //List<Vertex> hi = new(), lo = new();
            //for(int i = 0; i < arr.Length; i++)
            //{
            //    switch (axis)
            //    {
            //        case 'x':
            //        case 'X':
            //            if (arr[i].x >= center.x) hi.Add(arr[i]);
            //            if (arr[i].x <= center.x) lo.Add(arr[i]);
            //            break;
            //        case 'y':
            //        case 'Y':
            //            if (arr[i].y >= center.y) hi.Add(arr[i]);
            //            if (arr[i].y <= center.y) lo.Add(arr[i]);
            //            break;
            //        case 'z':
            //        case 'Z':
            //            if (arr[i].z >= center.z) hi.Add(arr[i]);
            //            if (arr[i].z <= center.z) lo.Add(arr[i]);
            //            break;
            //    }
            //}
            System.Collections.Concurrent.ConcurrentBag<Vertex> hi = new(), lo = new();
            Parallel.For(0, arr.Length, i =>
            {
                switch (axis)
                {
                    case 'x':
                    case 'X':
                        if (arr[i].x >= center.x) hi.Add(arr[i]);
                        if (arr[i].x <= center.x) lo.Add(arr[i]);
                        break;
                    case 'y':
                    case 'Y':
                        if (arr[i].y >= center.y) hi.Add(arr[i]);
                        if (arr[i].y <= center.y) lo.Add(arr[i]);
                        break;
                    case 'z':
                    case 'Z':
                        if (arr[i].z >= center.z) hi.Add(arr[i]);
                        if (arr[i].z <= center.z) lo.Add(arr[i]);
                        break;
                }
            });
            return new Vertex[][] {
                parent != null?hi.ToArray().RotateBy(parent.rotation):hi.ToArray(),
                parent != null?lo.ToArray().RotateBy(parent.rotation):lo.ToArray()
            };
        }
        /// <summary>
        /// Parellelized.
        /// direction 0=+, 1=-
        /// </summary>
        /// <param name="axis">[xyz] must have length of 1</param>
        /// <param name="parent">parent transform matrix</param>
        /// <param name="multiplier">distance multiplier</param>
        /// <returns></returns>
        public static Vertex[][] LinearSplit(this Vertex[] array, char axis, MatTransform? parent = null, float multiplier = 1f)
        {
            Vertex[] arr = parent != null ? array.RotateBy(parent.rotation.Transpose()) : array.Deepcopy();

            (Vertex max, Vertex min) = arr.MaxMin(axis);
            float halfdistance = min.DistanceTo(max) / 2f * Clamp(multiplier, 2f);

            //List<Vertex> hi = new(), lo = new();
            //foreach (Vertex v in arr)
            //{
            //    if (v.DistanceTo(max) < halfdistance) { hi.Add(v); }
            //    if (v.DistanceTo(min) < halfdistance) { lo.Add(v); }
            //}
            System.Collections.Concurrent.ConcurrentBag<Vertex> hi = new(), lo = new();
            Parallel.ForEach(arr, v =>
            {
                if (v.DistanceTo(max) < halfdistance) { hi.Add(v); }
                if (v.DistanceTo(min) < halfdistance) { lo.Add(v); }
            });
            return new Vertex[][] {
                parent != null?hi.ToArray().RotateBy(parent.rotation):hi.ToArray(),
                parent != null?lo.ToArray().RotateBy(parent.rotation):lo.ToArray()
            };
        }
        /// <summary>
        /// direction 0=+, 1=-
        /// </summary>
        /// <param name="axes">[xyz] must have length of 2</param>
        /// <param name="parent">parent transform matrix</param>
        /// <exception cref="ArgumentException"></exception>
        public static Vertex[,][] QuadrantSplit(this Vertex[] array, char[] axes, MatTransform? parent = null)
        {
            if (axes.Length != 2) throw new ArgumentException("direction must has length of 2");
            Vertex[] arr = parent != null ? array.RotateBy(parent.rotation.Transpose()) : array.Deepcopy();

            var total = arr.MaxMin();
            Vector3 center = total.max.Midpoint(total.min);

            Vertex[,][] quad = new Vertex[2, 2][];
            Vertex[][] first = arr.AxisSplit(axes[0], center);
            for (int i = 0; i < first.Length; i++)
            {
                Vertex[][] second = first[i].AxisSplit(axes[1], center);
                for (int j = 0; j < second.Length; j++)
                {
                    quad[i, j] = parent != null ? second[j].RotateBy(parent.rotation) : second[j];
                }
            }
            return quad;
        }



        public static Vector3 Midpoint(this Vector3 origin, Vector3 target) => origin.opAdd(target).opDiv(2f);

        public static float Clamp(this float value, float max = 0, float min = 1)
        {
            if (value < min) value = min;
            if (value > max) value = max;
            return value;
        }
        public static Vector3 ToEulerDegrees(this Matrix3 rotation)
        {
            float y = 0, p = 0, r = 0;
            rotation.ToEulerDegrees(ref y, ref p, ref r);
            return new Vector3(y, p, r);
        }

    }
}
