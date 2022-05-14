using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nifly;

namespace SkyrimAnimationChecker.NIF
{
    public static class niflyStatisticsExtensions
    {
        #region Vector3
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

        public static Vector3 Mean(this Vector3[] arr, SkinWeight[]? weights = null, bool normalize = true)
        {
            if (weights != null && arr.Length != weights.Length) throw new ArgumentException($"Argument lengths are different {arr.Length}, {weights.Length}");

            Vertex[] vertices = new Vertex[arr.Length];
            for (var i = 0; i < arr.Length; i++)
            {
                vertices[i] = new Vertex(i, arr[i], weights != null ? weights[i].weight : 1f);
            }
            return Mean(vertices, weights != null, normalize);
        }
        public static Vector3 Variance(this Vector3[] arr, Vector3? mean = null)
        {
            if (mean == null) mean = Mean(arr);
            Vector3 sum = new();
            foreach (Vector3 v in arr)
            {
                sum.x += (float)Math.Pow(v.x - mean.x, 2f);
                sum.y += (float)Math.Pow(v.y - mean.y, 2f);
                sum.z += (float)Math.Pow(v.z - mean.z, 2f);
            }
            return sum;
        }
        public static Vector3 StdDev(this Vector3[] arr, Vector3? mean = null)
        {
            if (mean == null) mean = Mean(arr);
            Vector3 var = Variance(arr, mean);
            return new Vector3((float)Math.Sqrt(var.x), (float)Math.Sqrt(var.y), (float)Math.Sqrt(var.z));
        }
        public static Vector3 RMS(this Vector3[] arr, Vector3? mean = null) => StdDev(arr, mean);

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
        #endregion
        #region Vertex
        public static Vector3 Width(this Vertex[] arr)
        {
            (Vertex max, Vertex min) = arr.MinMax();
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
        public static Vertex[] Normalize(this Vertex[] arr)
        {
            float max = 0;
            foreach (Vertex v in arr) max = Math.Max(max, v.weight);
            Vertex[] newArr = new Vertex[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                newArr[i] = new(arr[i].Index, arr[i].vert, arr[i].weight / max);
            }
            return newArr;
        }

        public static Vector3 Mean(this Vertex[] arr, bool weight = true, bool normalize = true)
        {
            var a = (weight && normalize) ? arr.Normalize() : arr;
            Vector4 sum = new();
            for (int i = 0; i < arr.Length; i++)
            {
                sum.x += a[i].x * (weight ? a[i].w : 1);
                sum.y += a[i].y * (weight ? a[i].w : 1);
                sum.z += a[i].z * (weight ? a[i].w : 1);
                sum.w += (weight ? a[i].w : 1);
            }
            return new Vector3(sum.x / sum.w, sum.y / sum.w, sum.z / sum.w);
        }
        public static Vector3 Variance(this Vertex[] arr, Vector3? mean = null)
        {
            if (mean == null) mean = Mean(arr);
            Vector3 sum = new();
            foreach (Vertex v in arr)
            {
                sum.x += (float)Math.Pow(v.x - mean.x, 2f);
                sum.y += (float)Math.Pow(v.y - mean.y, 2f);
                sum.z += (float)Math.Pow(v.z - mean.z, 2f);
            }
            return sum;
        }
        public static Vector3 StdDev(this Vertex[] arr, Vector3? mean = null)
        {
            if (mean == null) mean = Mean(arr);
            Vector3 var = Variance(arr, mean);
            return new Vector3((float)Math.Sqrt(var.x), (float)Math.Sqrt(var.y), (float)Math.Sqrt(var.z));
        }
        public static Vector3 RMS(this Vertex[] arr, Vector3? mean = null) => StdDev(arr, mean);

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

        public static (Vertex max, Vertex min) MinMax(this Vertex[] array, Matrix3? rotation = null)
        {
            Vertex[] arr = new Vertex[array.Length];
            if (rotation == null) arr = array;
            else
            {
                for (int i = 0; i < array.Length; i++) { arr[i] = new(); arr[i].vert = array[i].vert.RotateBy(rotation); }
            }

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
        public static (Vertex max, Vertex min) MinMax(this Vertex[] array, char axis, Matrix3? rotation = null)
        {
            Vertex[] arr = new Vertex[array.Length];
            if (rotation == null) arr = array;
            else
            {
                for (int i = 0; i < array.Length; i++) { arr[i] = new(); arr[i].vert = array[i].vert.RotateBy(rotation); }
            }

            if (arr.Length == 1) return (arr[0], arr[0]);
            else if (arr.Length > 1)
            {
                switch (axis)
                {
                    case 'x':
                    case 'X':
                        return (arr.MaxBy(v => v.x) ?? new(), arr.MinBy(v => v.x) ?? new());
                    case 'y':
                    case 'Y':
                        return (arr.MaxBy(v => v.y) ?? new(), arr.MinBy(v => v.y) ?? new());
                    case 'z':
                    case 'Z':
                        return (arr.MaxBy(v => v.z) ?? new(), arr.MinBy(v => v.z) ?? new());
                    case 'w':
                    case 'W':
                        return (arr.MaxBy(v => v.w) ?? new(), arr.MinBy(v => v.w) ?? new());
                }
            }
            return (new(), new());
        }
        /// <summary>
        /// direction 0=+, 1=-
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="axis">[xyz] must have length of 1</param>
        /// <param name="multiplier">0~2</param>
        /// <returns></returns>
        public static Vertex[][] LinearSplit(this Vertex[] array, char axis, MatTransform? parent = null, float multiplier = 1f)
        {
            Vertex[] arr = new Vertex[array.Length];
            for (int i = 0; i < array.Length; i++) arr[i] = new(array[i].Index, array[i].vert, array[i].w);
            if (parent != null) { for (int i = 0; i < arr.Length; i++) arr[i] = arr[i].RotateBy(parent.rotation.Transpose()); }
            
            (Vertex max, Vertex min) = arr.MinMax(axis);
            float halfdistance = min.DistanceTo(max) / 2f * Clamp(multiplier, 2f);
            List<Vertex> mins = new(), maxs = new();
            foreach (Vertex v in arr)
            {
                if (v.DistanceTo(max) < halfdistance) { maxs.Add(parent != null ? v.RotateBy(parent.rotation) : v); }
                if (v.DistanceTo(min) < halfdistance) { mins.Add(parent != null ? v.RotateBy(parent.rotation) : v); }
            }
            return new Vertex[][] { maxs.ToArray(), mins.ToArray() };
        }
        /// <summary>
        /// direction 0=+, 1=-
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="axes">[xyz] must have length of 2</param>
        /// <param name="multiplier">0~2</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Vertex[,][] QuadrantSplit(this Vertex[] array, char[] axes, MatTransform? parent = null, float multiplier = 1f)
        {
            Vertex[] arr = new Vertex[array.Length];
            for (int i = 0; i < array.Length; i++) arr[i] = new(array[i].Index, array[i].vert, array[i].w);
            if (parent != null) { for (int i = 0; i < arr.Length; i++) arr[i] = arr[i].RotateBy(parent.rotation.Transpose()); }

            if (axes.Length != 2) throw new ArgumentException("direction must has length of 2");
            Vertex[,] axis = new Vertex[2, 2];
            float[] halfdistance = new float[2];
            for (int i = 0; i < 2; i++) (axis[i, 0], axis[i, 1]) = arr.MinMax(axes[i]);
            for (int i = 0; i < 2; i++) halfdistance[i] = axis[i, 0].DistanceTo(axis[i, 1]) / 2f * Clamp(multiplier, 2f);
            List<Vertex>[,] quad = new List<Vertex>[,] {
                { new List<Vertex>() , new List<Vertex>() },
                { new List<Vertex>() , new List<Vertex>() }
            };
            foreach (Vertex v in arr)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (v.DistanceTo(axis[0, i]) < halfdistance[0] &&
                            v.DistanceTo(axis[1, j]) < halfdistance[1])
                            quad[i, j].Add(parent != null ? v.RotateBy(parent.rotation) : v);
                    }
                }
            }
            Vertex[,][] vertices = new Vertex[2, 2][];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    vertices[i, j] = quad[i, j].ToArray();
                }
            }
            return vertices;
        }
        #endregion

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
