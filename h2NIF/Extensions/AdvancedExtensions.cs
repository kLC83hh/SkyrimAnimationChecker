using h2NIF.DataStructure;
using nifly;

namespace h2NIF.Extensions
{
    internal static class AdvancedExtensions
    {
        /// <summary>
        /// Get value is positive(+) or negative(-).
        /// </summary>
        /// <param name="value"></param>
        /// <returns>[+-]</returns>
        public static char GetPN(this float value) => value < 0 ? '-' : '+';
        /// <summary>
        /// Get sphere is distributed pos/neg direction on the <c>axis</c>.
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="axis">[xyzXYZ]</param>
        /// <param name="origin"></param>
        /// <returns>[+-]</returns>
        /// <exception cref="ArgumentException">axis is not [xyzXYZ]</exception>
        public static char GetPN(this NiShape sphere, char axis, Vector3? origin = null)
        {
            if (origin == null) origin = new();
            switch (axis)
            {
                case 'x':
                case 'X':
                    return GetPN(sphere.transform.translation.x - origin.x);
                case 'y':
                case 'Y':
                    return GetPN(sphere.transform.translation.y - origin.y);
                case 'z':
                case 'Z':
                    return GetPN(sphere.transform.translation.z - origin.z);
            }
            throw new ArgumentException("axis must be x or y or z");
        }
        /// <summary>
        /// Determine a <c>sphere</c> is which sided or not via its name.
        /// </summary>
        /// <param name="sphere"></param>
        /// <returns>[LR]</returns>
        public static char? GetSide(this NiShape sphere)
        {
            if (sphere.name.get().StartsWith('L') || sphere.name.get().StartsWith("NPC L")) return 'L';
            else if (sphere.name.get().StartsWith('R') || sphere.name.get().StartsWith("NPC R")) return 'R';
            return null;
        }
        /// <summary>
        /// Localized only
        /// </summary>
        /// <param name="center"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        public static Vector3 Strengthen(this Vector3 center, Vector3 strength) => new Vector3(center.x * strength.x, center.y * strength.y, center.z * strength.z);

        /// <summary>
        /// Color4(r, g, b, multplier)
        /// </summary>
        /// <param name="nif"></param>
        /// <param name="shape"></param>
        /// <param name="color">r, g, b, multplier</param>
        public static void SetShaderEmissive(this NifFile nif, NiShape shape, Color4 color)
        {
            if (shape.HasShaderProperty())
            {
                var shader = nif.GetBlock<BSLightingShaderProperty>(shape.ShaderPropertyRef());
                if (shader != null && shader.IsEmissive())
                {
                    shader.SetEmissiveMultiple(color.a);
                    shader.SetEmissiveColor(new Color4(color.r, color.g, color.b, 1f));
                }
            }
        }

        #region Vector3
        /// <summary>
        /// Only work with global vectors
        /// </summary>
        /// <param name="arr">Should be global vectors</param>
        /// <param name="side">L, R</param>
        /// <returns></returns>
        public static Vector3[] FilterSide(this Vector3[] arr, char? side)
        {
            if (side == null) return arr;
            List<Vector3> filtered = new();
            foreach (var v in arr)
            {
                switch (side)
                {
                    case 'L':
                    case 'l':
                        if (v.x <= 0) filtered.Add(v);
                        break;
                    case 'R':
                    case 'r':
                        if (v.x >= 0) filtered.Add(v);
                        break;
                }
            }
            //System.Collections.Concurrent.ConcurrentBag<Vector3> filtered = new();
            //Parallel.ForEach(arr, v =>
            //{
            //    switch (side)
            //    {
            //        case 'L':
            //        case 'l':
            //            if (v.x <= 0) filtered.Add(v);
            //            break;
            //        case 'R':
            //        case 'r':
            //            if (v.x >= 0) filtered.Add(v);
            //            break;
            //    }
            //});
            return filtered.ToArray();
        }

        public static Vector3 GlobalizeTo(this Vector3 vector, Vector3 parent) => vector.opAdd(parent);
        public static Vector3 LocalizeTo(this Vector3 vector, Vector3 parent) => vector.opSub(parent);
        public static Vector3[] LocalizeTo(this Vector3[] arr, Vector3 parent)
        {
            Vector3[] result = new Vector3[arr.Length];
            for (int i = 0; i < arr.Length; i++) result[i] = arr[i].opSub(parent);
            return result;
        }
        public static Vector3 RotateBy(this Vector3 vector, Matrix3 rotation) => rotation.opMult(vector);
        public static Vector3[] RotateBy(this Vector3[] arr, Matrix3 rotation)
        {
            Vector3[] result = new Vector3[arr.Length];
            for (int i = 0; i < arr.Length; i++) result[i] = rotation.opMult(arr[i]);
            return result;
        }
        /// <summary>
        /// Localized only
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        public static Vector3[] WindowBy(this Vector3[] arr, Vector3 window)
        {
            List<Vector3> result = new();
            foreach (var v in arr)
            {
                if (Math.Abs(v.x) < window.x &&
                    Math.Abs(v.y) < window.y &&
                    Math.Abs(v.z) < window.z)
                    result.Add(v);
                //else
                //{
                //    string mx = string.Empty, my = string.Empty, mz = string.Empty, msg = string.Empty;
                //    if (Math.Abs(v.x) < window.x == false) { float r = v.x / window.x; if (r > 0) { r -= 1; } else { r += 1; } mx = $"x:({v.x}/{window.x}) {r * 100f:N3}%"; msg += mx; }
                //    if (Math.Abs(v.y) < window.y == false) { float r = v.y / window.y; if (r > 0) { r -= 1; } else { r += 1; } my = $"y:({v.y}/{window.y}) {r * 100f:N3}%"; msg += my; }
                //    if (Math.Abs(v.z) < window.z == false) { float r = v.z / window.z; if (r > 0) { r -= 1; } else { r += 1; } mz = $"z:({v.z}/{window.z}) {r * 100f:N3}%"; msg += mz; }
                //    TEST.M.D($"{mx} {my} {mz}");//TEST.M.D(msg);
                //}
            }
            //System.Collections.Concurrent.ConcurrentBag<Vector3> result = new();
            //Parallel.ForEach(arr, v =>
            //{
            //    if (Math.Abs(v.x) < window.x &&
            //        Math.Abs(v.y) < window.y &&
            //        Math.Abs(v.z) < window.z)
            //        result.Add(v);
            //});
            return result.ToArray();
        }

        public static Vector3 Center(this Vector3[] arr, SkinWeight[]? weights = null) => arr.Mean(weights);
        public static float Radius(this Vector3[] arr, Vector3 center, SkinWeight[]? weights = null) => arr.AverageDistanceTo(center, weights);
        #endregion

        #region Vertex
        //Parellelized
        public static Vertex[] Deepcopy(this Vertex[] arr)
        {
            Vertex[] vertices = new Vertex[arr.Length];
            //for (int i = 0; i < arr.Length; i++) vertices[i] = new Vertex(arr[i].Index, arr[i].vert, arr[i].weight);
            Parallel.For(0, vertices.Length, i =>
            {
                vertices[i] = new Vertex(arr[i].Index, arr[i].vert, arr[i].weight);
            });
            if (vertices == null) throw new Exception("Deepcopy Failed");
            return vertices;
        }

        /// <summary>
        /// Only work with global vectors
        /// </summary>
        /// <param name="arr">Should be global vectors</param>
        /// <param name="side">L, R</param>
        /// <returns></returns>
        public static Vertex[] FilterSide(this Vertex[] arr, char? side)
        {
            if (side == null) return arr;
            List<Vertex> filtered = new();
            foreach (var v in arr)
            {
                switch (side)
                {
                    case 'L':
                    case 'l':
                        if (v.x <= 0) filtered.Add(v);
                        break;
                    case 'R':
                    case 'r':
                        if (v.x >= 0) filtered.Add(v);
                        break;
                }
            }
            //System.Collections.Concurrent.ConcurrentBag<Vertex> filtered = new();
            //Parallel.ForEach(arr, v =>
            //{
            //    switch (side)
            //    {
            //        case 'L':
            //        case 'l':
            //            if (v.x <= 0) filtered.Add(v);
            //            break;
            //        case 'R':
            //        case 'r':
            //            if (v.x >= 0) filtered.Add(v);
            //            break;
            //    }
            //});
            return filtered.ToArray();
        }

        public static Vertex[] LocalizeTo(this Vertex[] arr, Vector3 parent)
        {
            Vertex[] result = new Vertex[arr.Length];
            for (int i = 0; i < arr.Length; i++) result[i] = arr[i].LocalizeTo(parent);
            return result;
        }
        //Parellelized
        public static Vertex[] RotateBy(this Vertex[] arr, Matrix3 rotation)
        {
            Vertex[] result = new Vertex[arr.Length];
            //for (int i = 0; i < arr.Length; i++) result[i] = arr[i].RotateBy(rotation);
            Parallel.For(0, arr.Length, i => result[i] = arr[i].RotateBy(rotation));
            return result;
        }
        /// <summary>
        /// Localized only
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="window"></param>
        /// <param name="standard">min,max,center</param>
        /// <returns></returns>
        public static Vertex[] WindowBy(this Vertex[] arr, Vector3 window, string wBase = "center")
        {
            Vertex std;
            switch (wBase)
            {
                case "min": (_, std) = arr.MaxMin(); break;
                case "max": (std, _) = arr.MaxMin(); break;
                case "center":
                default:
                    std = new();
                    (Vertex max, Vertex min) = arr.MaxMin();
                    std.vert = max.vert.opAdd(min.vert).opDiv(2f);
                    break;
            }
            Vector3 win = window;
            if (wBase == "center") win = window.opDiv(2f);

            List<Vertex> result = new();
            foreach (var v in arr)
            {
                if (Math.Abs(v.x - std.x) <= win.x &&
                    Math.Abs(v.y - std.y) <= win.y &&
                    Math.Abs(v.z - std.z) <= win.z)
                    result.Add(v);
                //else
                //    TEST.M.D($"{Math.Abs(v.x - std.x)}/{win.x}({Math.Abs(v.x - std.x) < win.x}), {Math.Abs(v.y - std.y)}/{win.y}({Math.Abs(v.y - std.y) < win.y}), {Math.Abs(v.z - std.z)}/{win.z}({Math.Abs(v.z - std.z) < win.z})");
            }
            //System.Collections.Concurrent.ConcurrentBag<Vertex> result = new();
            //Parallel.ForEach(arr, v =>
            //{
            //    if (Math.Abs(v.x - std.x) <= win.x &&
            //        Math.Abs(v.y - std.y) <= win.y &&
            //        Math.Abs(v.z - std.z) <= win.z)
            //        result.Add(v);
            //});
            return result.ToArray();
        }

        public static Vector3 Center(this Vertex[] arr, bool weight) => arr.Mean(weight);
        public static float Radius(this Vertex[] arr, Vector3 center, bool weight) => arr.AverageDistanceTo(center, weight);
        #endregion


    }
}
