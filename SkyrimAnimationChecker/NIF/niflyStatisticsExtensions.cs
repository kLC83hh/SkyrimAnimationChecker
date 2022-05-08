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

        public static Vector3 Mean(this Vector3[] arr)
        {
            Vector3 sum = new();
            foreach (Vector3 v in arr)
            {
                sum.x += v.x;
                sum.y += v.y;
                sum.z += v.z;
            }
            return new Vector3(sum.x / arr.Length, sum.y / arr.Length, sum.z / arr.Length);
        }
        public static Vector3 MeanWeighted(this Vector3[] arr, SkinWeight[] weights)
        {
            Vector3 sum = new();
            if (arr.Length != weights.Length) throw new ArgumentException($"Argument lengths are different {arr.Length}, {weights.Length}");
            var w = weights.Normalize();
            for (int i = 0; i < arr.Length; i++)
            {
                sum.x += arr[i].x * w[i].weight;
                sum.y += arr[i].y * w[i].weight;
                sum.z += arr[i].z * w[i].weight;
            }
            return new Vector3(sum.x / arr.Length, sum.y / arr.Length, sum.z / arr.Length);
        }

        public static Vector3 Variance(this Vector3[] arr, Vector3 mean)
        {
            Vector3 sum = new();
            foreach (Vector3 v in arr)
            {
                sum.x += (float)Math.Pow(v.x - mean.x, 2f);
                sum.y += (float)Math.Pow(v.y - mean.y, 2f);
                sum.z += (float)Math.Pow(v.z - mean.z, 2f);
            }
            return sum;
        }
        public static Vector3 Variance(this Vector3[] arr) => Variance(arr, Mean(arr));
        public static Vector3 StdDev(this Vector3[] arr, Vector3 mean)
        {
            Vector3 var = Variance(arr, mean);
            return new Vector3((float)Math.Sqrt(var.x), (float)Math.Sqrt(var.y), (float)Math.Sqrt(var.z));
        }
        public static Vector3 StdDev(this Vector3[] arr) => StdDev(arr, Mean(arr));
        public static Vector3 RMS(this Vector3[] arr, Vector3 mean) => StdDev(arr, mean);
        public static Vector3 RMS(this Vector3[] arr) => StdDev(arr);
    }
}
