using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nifly;
using h2NIF.DataStructure;

namespace h2NIF.Extensions
{
    internal static class StatisticsExtensions
    {
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

    }
}
