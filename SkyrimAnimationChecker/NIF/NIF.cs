using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;
using nifly;

namespace SkyrimAnimationChecker.NIF
{
    internal class NIF
    {
        protected VM_GENERAL vm;
        public NIF(VM_GENERAL linker) => vm = linker;
        public NIF(VM linker) => vm = linker.GENERAL;


    }
    public static class NIFExtensions
    {
        public static T? GetBlock<T>(this NifFile nif, NiRef refer) => nif.GetBlock<T>(refer.index);
        public static T? GetBlock<T>(this NifFile nif, uint refer)
        {
            if (nif.GetHeader().GetBlockById(refer) is T output) return output;
            //else throw new InvalidCastException($"NiObject can not be converted to {typeof(T)}");
            else return default;
        }

        public static NiNode? GetNode(this NifFile nif, string name)
        {
            foreach (var node in nif.GetNodes())
            {
                if (node.name.get() == name) return node;
            }
            return null;
        }
        public static NiShape? GetShape(this NifFile nif, string name)
        {
            if (!nif.GetShapes().Any(x => x.name.get() == name)) return null;
            else return nif.GetShapes().First(x => x.name.get() == name);
        }

        public static BSTriShape? GetTriShape(this NifFile nif, string name)
        {
            if (!nif.GetTriShapes().Any(x => x.name.get() == name)) return null;
            else return nif.GetTriShapes().First(x => x.name.get() == name);
        }
        public static BSTriShape[] GetNamedTriShapes(this NifFile nif, string name, string? nameEnd = null)
        {
            if (!nif.GetTriShapes().Any(x => x.name.get().StartsWith(name))) return Array.Empty<BSTriShape>();
            else
            {
                if (string.IsNullOrEmpty(nameEnd)) return nif.GetTriShapes().Where(x => x.name.get().StartsWith(name)).ToArray();
                else
                {
                    System.Text.RegularExpressions.Regex regexEnd = new($@"{nameEnd}\d*$");
                    if (!nif.GetTriShapes().Any(x => regexEnd.IsMatch(x.name.get()))) return Array.Empty<BSTriShape>();
                    else return nif.GetTriShapes().Where(x => x.name.get().StartsWith(name)).Where(x => regexEnd.IsMatch(x.name.get())).ToArray();
                }
            }
        }
        public static BSTriShape[] GetTriShapes(this NifFile nif)
        {
            vectorNiShape shapes = nif.GetShapes();
            List<BSTriShape> bSTriShapes = new List<BSTriShape>();
            foreach (var shape in shapes)
            {
                var trishape = nif.GetBlock<BSTriShape>(nif.GetBlockID(shape));
                if (trishape != null) bSTriShapes.Add(trishape);
            }
            return bSTriShapes.ToArray();
        }

        public static NiShape? GetSkinned(this NifFile nif)
        {
            NiShape? skin = null;
            foreach (var shape in nif.GetShapes())
            {
                if (shape.IsSkinned()) skin = shape;
            }
            return skin;
        }
        public static NiShape[] GetAllSkinned(this NifFile nif)
        {
            List<NiShape> skin = new();
            foreach (var shape in nif.GetShapes())
            {
                if (shape.IsSkinned()) skin.Add(shape);
            }
            return skin.ToArray();
        }

        public static void SetTransform(this NifFile nif, string name, MatTransform transform)
        {
            if (nif.GetShapeNames().Any(x => x == name))
            {
                NiShape? shape = nif.GetShape(name);
                if (shape != null) shape.transform = transform;
            }
            else
            {
                NiNode? node = nif.GetNode(name);
                if (node != null) (node).transform = transform;
            }
        }
        public static void SetTransform(this NifFile nif, string name, string transform)
        {
            if (nif.GetShapeNames().Any(x => x == name))
            {
                NiShape? shape = nif.GetShape(name);
                if (shape != null)
                {
                    var val = GetVals(transform);
                    shape.transform.scale = val.s;
                    shape.transform.translation.x = val.x;
                    shape.transform.translation.y = val.y;
                    shape.transform.translation.z = val.z;
                }
            }
            else
            {
                NiNode? node = nif.GetNode(name);
                if (node != null)
                {
                    var val = GetVals(transform);
                    node.transform.scale = val.s;
                    node.transform.translation.x = val.x;
                    node.transform.translation.y = val.y;
                    node.transform.translation.z = val.z;
                }
            }
        }
        private static (float x, float y, float z, float s) GetVals(string data)
        {
            string[] split = data.Split(',');
            if (split.Length != 4) return (0, 0, 0, 1);

            try
            {
                float x, y, z, s;
                x = float.Parse(split[0]);
                y = float.Parse(split[1]);
                z = float.Parse(split[2]);
                s = float.Parse(split[3]);
                return (x, y, z, s);
            }
            catch { }

            return (0, 0, 0, 1);
        }

        private static List<NiShape> GetChildShapes(this NifFile nif, NiNode parent, bool all = false)
        {
            var shapes = nif.GetShapes();
            List<NiShape> children = new();
            foreach (NiShape shape in shapes)
            {
                if (nif.GetParentNode(shape).name.get() == parent.name.get() && (shape.transform.scale > 0 || all)) children.Add(shape);
            }
            return children;
        }
        public static void UpdateSphere(this NifFile nif, string name, string transform)
        {
            if (name.Contains("Finger"))
            {
                var shape = nif.GetShape(name + " Sphere");
                if (shape != null)
                {
                    nif.SetTransform(shape.name.get(), transform);
                }
            }
            else
            {
                var node = nif.GetNode(name + " Location");
                if (node != null)
                {
                    //nifly.vectoruint32 indices = new();
                    //node.childRefs.GetIndices(indices);
                    //if (indices.Count == 1) nif.SetTransform(name + " Sphere", transform);
                    List<NiShape> children = nif.GetChildShapes(node);
                    if (children.Count == 1) nif.SetTransform(children.First().name.get(), transform);
                }
            }
        }
        public static void UpdateCapsule(this NifFile nif, string name, string transformI, string transformF)
        {
            var node = nif.GetNode(name + " Location");
            if (node != null)
            {
                List<NiShape> children = nif.GetChildShapes(node, true);
                if (children.Count > 1)
                {
                    children = children.OrderBy(x => x.name.get()).ToList();
                    if (children.First().transform.scale > 0 && children.Last().transform.scale > 0)
                    {
                        nif.SetTransform(children.First().name.get(), transformI);
                        nif.SetTransform(children.Last().name.get(), transformF);
                    }
                }
            }
        }

        public static int GetFilteredBones(this NifFile nif, out Bone[] bones, NiShape skin, string[] filter)
        {
            vectorstring names = new();
            vectorint ids = new();
            nif.GetShapeBoneList(skin, names);
            nif.GetShapeBoneIDList(skin, ids);

            if (names.Count != ids.Count)
            {
                bones = Array.Empty<Bone>();
                return -Math.Abs(names.Count - ids.Count);
            }

            List<Bone> result = new();
            for (int i = 0; i < names.Count; i++)
            {
                foreach (string f in filter)
                {
                    if (names[i].StartsWith(f)) result.Add(new Bone(nif, skin, i, (uint)ids[i], names[i]));
                }
            }
            bones = result.OrderBy(x => x.Name).ToArray();
            return names.Count;
        }


        public static Vector3 ToEulerDegrees(this Matrix3 rotation)
        {
            float y = 0, p = 0, r = 0;
            rotation.ToEulerDegrees(ref y, ref p, ref r);
            return new Vector3(y, p, r);
        }

    }
    public static class SphereExtensions
    {
        #region Basics
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
            foreach (Vector3 v in arr)
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
            return filtered.ToArray();
        }

        public static Vector3 LocalizeTo(this Vector3 vector, Vector3 parent) => vector.opSub(parent);
        public static Vector3[] LocalizeTo(this Vector3[] arr, Vector3 parent)
        {
            Vector3[] result = new Vector3[arr.Length];
            for (int i = 0; i < arr.Length; i++) result[i] = arr[i].opSub(parent);
            return result;
        }
        public static Vector3 RotateTo(this Vector3 vector, Matrix3 rotation) => rotation.opMult(vector);
        public static Vector3[] RotateTo(this Vector3[] arr, Matrix3 rotation)
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
            foreach (Vector3 v in arr)
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
            return result.ToArray();
        }

        public static Vector3 Center(this Vector3[] arr, SkinWeight[]? weights = null) => arr.Mean(weights);
        public static float Radius(this Vector3[] arr, Vector3 center, SkinWeight[]? weights = null) => arr.AverageDistanceTo(center, weights);
        #endregion
        #region Vertex
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
            foreach (Vertex v in arr)
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
            return filtered.ToArray();
        }

        public static Vertex[] LocalizeTo(this Vertex[] arr, Vector3 parent)
        {
            Vertex[] result = new Vertex[arr.Length];
            for (int i = 0; i < arr.Length; i++) result[i] = arr[i].LocalizeTo(parent);
            return result;
        }
        public static Vertex[] RotateTo(this Vertex[] arr, Matrix3 rotation)
        {
            Vertex[] result = new Vertex[arr.Length];
            for (int i = 0; i < arr.Length; i++) result[i] = arr[i].RotateBy(rotation);
            return result;
        }
        /// <summary>
        /// Localized only
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        public static Vertex[] WindowBy(this Vertex[] arr, Vector3 window)
        {
            List<Vertex> result = new();
            foreach (Vertex v in arr)
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
            return result.ToArray();
        }

        public static Vector3 Center(this Vertex[] arr, bool weight) => arr.Mean(weight);
        public static float Radius(this Vertex[] arr, Vector3 center, bool weight) => arr.AverageDistanceTo(center, weight);
        #endregion
        #endregion


        public static (Vector3 center, float radius, string msg)? UpdateVertColor(this BSTriShape sphere)
        {
            M.D($"{sphere.name.get()} {sphere.rawColors.Count} {sphere.HasVertexColors()} {sphere.triangles.Count}");
            sphere.SetVertexColors(true);
            foreach (BSVertexData vert in sphere.vertData)
            {
                vert.colorData[0] = 255;
                vert.colorData[1] = 0;
                vert.colorData[2] = 0;
                vert.colorData[3] = 255;
            }
            foreach (Triangle t in sphere.triangles) { }
            M.D($"{sphere.name.get()} {sphere.rawColors.Count} {sphere.HasVertexColors()} {sphere.triangles.Count}");
            return (new(), 0, "Manual update");
        }
        public static (Vector3 center, float radius, string msg)? UpdateSphere(this NifFile nif, BSTriShape sphere, vectorVector3 vertices, vectorTriangle tris)
        {
            M.D($"{sphere.name.get()} {sphere.rawVertices.Count} {sphere.vertData.Count} {sphere.triangles.Count} {sphere.HasVertices()} {sphere.HasNormals()} {sphere.HasTangents()} {sphere.HasVertexColors()}");

            MatTransform parent = nif.GetParentNode(sphere).transform;
            Vector3 pRot = parent.rotation.ToEulerDegrees();
            Vector3 sRot = sphere.transform.rotation.ToEulerDegrees();
            M.D($"({parent.translation.x},{parent.translation.y},{parent.translation.z}),({pRot.x},{pRot.y},{pRot.z})" +
                $" ({sphere.transform.translation.x},{sphere.transform.translation.y},{sphere.transform.translation.z}),({sRot.x},{sRot.y},{sRot.z})");
            sphere.transform.scale = 1;

            sphere.SetVertexColors(true);

            vectorBSVertexData newVerts = new();
            foreach (Vector3 v in vertices)
            {
                Vector3 local = v;
                local = local.LocalizeTo(parent.translation).RotateTo(parent.rotation.Transpose());
                local = local.LocalizeTo(sphere.transform.translation).RotateTo(sphere.transform.rotation.Transpose());
                newVerts.Add(new BSVertexData() { vert = local, colorData = new arrayuint8_4(new byte[4] { 255, 0, 0, 255 }) });
            }
            sphere.SetVertexData(newVerts);

            M.D($"{vertices.Count} {tris.Count}");
            //var t = new vectoruint32(sphere.vertData.Count);
            //for (int i = 0; i < t.Count; i++) { t[i] = (uint)i; }
            var newtris = new vectorTriangle();
            foreach (Triangle t in tris)
            {
                if (t.p1 < sphere.vertData.Count && t.p2 < sphere.vertData.Count && t.p3 < sphere.vertData.Count)
                    newtris.Add(t);
            }
            sphere.SetTriangles(newtris);
            //sphere.ReorderTriangles();

            sphere.UpdateRawNormals();
            sphere.UpdateRawTangents();

            // color emission
            if (sphere.HasShaderProperty())
            {
                var shader = nif.GetBlock<BSLightingShaderProperty>(sphere.ShaderPropertyRef());
                if (shader != null && shader.IsEmissive())
                {
                    shader.SetEmissiveMultiple(1);
                    shader.SetEmissiveColor(new Color4(0, 0, 1, 1));
                }
            }

            M.D($"{sphere.name.get()} {sphere.rawVertices.Count} {sphere.vertData.Count} {sphere.triangles.Count} {sphere.HasVertices()} {sphere.HasNormals()} {sphere.HasTangents()} {sphere.HasVertexColors()}");
            return (new(), 0, "Manual update");
        }

        public static (Vector3 center, float radius, string msg)? UpdateSphere(this NiShape sphere, Vector3 center, float radius)
        {
            sphere.transform.translation = center;
            sphere.transform.scale = radius;
            return (center, radius, "Manual update");
        }
        public static (Vector3 center, float radius, string msg)? UpdateSphere(this NifFile nif, NiShape sphere, NiShape skin, Vector3 sensitivity, Vector3 strength)
        {
            // get skin datas
            var skinInstance = nif.GetBlock<BSDismemberSkinInstance>(skin.SkinInstanceRef());
            if (skinInstance == null) return (new(), 0, "Error: UpdateSphere: can not retrieve BSDismemberSkinInstance");
            var skinPartition = nif.GetBlock<NiSkinPartition>(skinInstance.skinPartitionRef);
            if (skinPartition == null) return (new(), 0, "Error: UpdateSphere: can not retrieve skinPartition");
            var skinData = nif.GetBlock<NiSkinData>(skinInstance.dataRef);
            if (skinData == null) return (new(), 0, "Error: UpdateSphere: can not retrieve skinData");
            BSVertexData[] vdArray = skinPartition.vertData.ToArray();

            //TEST.M.D($"{vdArray[0].weightBones[0]}");
            //BSSkinBoneData bd = skinData.bones[(int)vdArray[0].weightBones[0]];

            // get skin vertices
            Vertex[] vertices = new Vertex[vdArray.Length];
            for (int i = 0; i < vdArray.Length; i++) vertices[i] = new Vertex(i, vdArray[i].vert, 1);

            return UpdateSphere(nif, sphere, vertices, sensitivity, strength);
        }
        public static (Vector3 center, float radius, string msg)? UpdateSphere(this NifFile nif, NiShape sphere, Vertex[] vertices, Vector3 sensitivity, Vector3 strength, float move = 0)
        {
            // parent vector
            nifly.NiNode node = nif.GetParentNode(sphere);
            MatTransform parent = node.GetTransformToParent();

            // box size
            Vector3 width = vertices.Width();
            Vector3 window = new() { x = width.x * sensitivity.x, y = width.y * sensitivity.y, z = width.z * sensitivity.z };

            // select vertices
#if DEBUG
            int vCountAll = vertices.Length;
#endif
            char? side = sphere.GetSide();
            vertices.FilterSide(side);
#if DEBUG
            int vCountSided = vertices.Length;
#endif
            vertices = vertices.LocalizeTo(parent.translation).RotateTo(parent.rotation.Transpose());
#if DEBUG
            int vCountLocalized = vertices.Length;
#endif
            vertices = vertices.WindowBy(window);
            string msg = string.Empty;
#if DEBUG
            int vCountWindowed = vertices.Length, vCountDiff = vCountWindowed - vCountAll;
            string sidenote = side == null ? "" : $"{side} sided ";
            string vCountnote = string.Empty;
            if (vCountWindowed != vCountLocalized) vCountnote += $"w{vCountWindowed}/";
            if (vCountLocalized != vCountSided) vCountnote += $"l{vCountLocalized}/";
            if (vCountSided != vCountAll) vCountnote += $"s{vCountSided}/";
            string vCountDiffnote = vCountDiff < 0 ? $" ({vCountDiff}) {((float)vCountWindowed / (float)vCountAll) * 100f:N3}%" : string.Empty;
            msg = ($"using {sidenote}{vCountnote}{vCountAll}{vCountDiffnote} vertices windowed by ({window.x:N3},{window.y:N3},{window.z:N3}), m={move}");
#endif
            // calc
            sphere.transform.translation = Center(vertices, true).Strengthen(strength);
            sphere.transform.scale = vertices.AverageDistanceTo(sphere.transform.translation, true);
            sphere.MicroAdjust();

            // color emission
            nif.SetShaderEmissive(sphere, new Color4(0, 0, 1, 1));

            return (sphere.transform.translation, sphere.transform.scale, msg);
        }

        public static void MicroAdjust(this NiShape sphere)
        {
            SphereAdjust adjust = new SphereAdjust(sphere);
            if (sphere.name.get().StartsWith("L Breast") || sphere.name.get().StartsWith("R Breast"))
            {
                // scaling
                if (sphere.name.get().Contains('1')) adjust.Scale(0.9);

                // translating, this should be done after scaling
                adjust.Gather(0.16);
                adjust.Raise(-0.3);
                adjust.Backward(0.33);
                if (sphere.name.get().Contains('1'))
                {
                    adjust.Gather(0.08);
                    adjust.Raise(0.15);
                }
                if (sphere.name.get().Contains('2'))
                {
                    adjust.Spread(0.08, -0.2);
                    adjust.Raise(0.1, 0.1);
                    adjust.Forward(0.06, -0.3);
                    adjust.ScaleUp(0.08, -0.6);
                }
            }
            else if (sphere.name.get().StartsWith("NPC L Butt") || sphere.name.get().StartsWith("NPC R Butt"))
            {
                // scaling
                adjust.ScaleUp(0.35, -0.975);

                // translating, this should be done after scaling
                adjust.Spread(0.11);
                adjust.Forward(0.58);
                adjust.Raise(0.55);
            }
            else if (sphere.name.get().StartsWith("NPC Belly"))
            {
                // scaling
                adjust.ScaleUp(0.5);

                // translating, this should be done after scaling
                adjust.Lower(0.5);
                adjust.Backward(0, 1);
            }
        }

    }


    public class Bone
    {
        public Bone(NifFile nif, NiShape skin, int index, uint id, string name)
        {
            Nif = nif;
            Skin = skin;

            Index = index;
            Id = id;
            Name = name;

            GetVertices();

        }

        protected NifFile Nif;
        protected NiShape Skin;
        protected BSDismemberSkinInstance SkinInstance => Nif.GetBlock<BSDismemberSkinInstance>(Skin.SkinInstanceRef()) ?? new();
        protected NiSkinPartition SkinPartition => Nif.GetBlock<NiSkinPartition>(SkinInstance.skinPartitionRef) ?? new();
        protected NiSkinData SkinData => Nif.GetBlock<NiSkinData>(SkinInstance.dataRef) ?? new();

        public int Index;
        public uint Id;
        public string Name;

        public nifly.NiNode? Node => Nif.GetNode(Name + " Location");
        public string NodeName => Node?.name.get() ?? string.Empty;

        public int SpheresCount => Spheres.Length;
        public BSTriShape[] Spheres => Nif.GetNamedTriShapes(Name, "Sphere");

        public BSTriShape? Sphere => SpheresCount == 0 ? null : Spheres[0];
        public string SphereName => Sphere?.name.get() ?? string.Empty;

        public MatTransform Transform => SkinData.bones[Index].boneTransform;
        public BoundingSphere Bound => SkinData.bones[Index].bounds;
        public int WeightsCount => Weights.Count;
        public vectorSkinWeight Weights => SkinData.bones[Index].vertexWeights;
        //public SkinWeight GetWeight(Vector3 vertex) => Weights[_Vertices.IndexOf(vertex)];

        public int VerticesCount => _Vertices.Count;
        public Vertex[] Vertices => _Vertices.ToArray();
        public vectorVector3 VerticesVector
        {
            get
            {
                vectorVector3 buffer = new();
                //foreach (Vector3 v in _Vertices) buffer.Add(v);
                foreach (Vertex v in _Vertices) buffer.Add(v.vert);
                return buffer;
            }
        }
        private List<Vertex> _Vertices = new();
        private void GetVertices()
        {
            foreach (SkinWeight sw in Weights)
            {
                _Vertices.Add(new Vertex(sw.index, SkinPartition.vertData[sw.index].vert, sw.weight));
                //_Vertices.Add(SkinPartition.vertData[sw.index].vert);
            }
        }

        public vectorTriangle Triangles => SkinPartition.partitions[0].triangles;
    }
    public class Vertex : Vector4
    {
        public Vertex(int index, Vector3 v, float weight)
        {
            Index = index;
            vert = v;
            this.weight = weight;
        }
        public int Index { get; set; }
        public Vector3 vert { get; set; }
        public float weight { get; set; }
        public new float x { get => vert.x; set => vert.x = value; }
        public new float y { get => vert.y; set => vert.y = value; }
        public new float z { get => vert.z; set => vert.z = value; }
        public new float w { get => weight; set => weight = value; }

        public Vertex LocalizeTo(Vector3 parent)
        {
            vert = vert.opSub(parent);
            return this;
        }
        public Vertex RotateBy(Matrix3 rotation)
        {
            vert = rotation.opMult(vert);
            return this;
        }
    }
    public class SphereAdjust
    {
        public SphereAdjust(NiShape sphere)
        {
            this.sphere = sphere;
            side = sphere.GetSide();
        }
        public NiShape sphere;
        public char? side;

        /// <summary>
        /// f = <c>sphere.transform.scale</c>*<c>A</c>+<c>B</c>
        /// </summary>
        /// <param name="A">ratio to scale</param>
        /// <param name="B">raw translation</param>
        private delegate void Adjust(float A, float B);


        /// <inheritdoc cref="Adjust"/>
        /// <exception cref="Exception">Not sided Exception</exception>
        public void Gather(float A, float B = 0)
        {
            if (side == 'L') Right(A, B);
            else if (side == 'R') Left(A, B);
            else throw new Exception("Can not gather because sphere is not sided");
        }
        /// <inheritdoc cref="Adjust"/>
        public void Gather(double A, double B = 0) => Gather((float)A, (float)B);
        /// <inheritdoc cref="Adjust"/>
        public void Spread(float A, float B = 0) => Gather(-A, -B);
        /// <inheritdoc cref="Adjust"/>
        public void Spread(double A, double B = 0) => Spread((float)A, (float)B);

        /// <inheritdoc cref="Adjust"/>
        public void Left(float A, float B = 0) => sphere.transform.translation.x -= (sphere.transform.scale * A + B);
        /// <inheritdoc cref="Adjust"/>
        public void Left(double A, double B = 0) => Left((float)A, (float)B);
        /// <inheritdoc cref="Adjust"/>
        public void Right(float A, float B = 0) => sphere.transform.translation.x += (sphere.transform.scale * A + B);
        /// <inheritdoc cref="Adjust"/>
        public void Right(double A, double B = 0) => Right((float)A, (float)B);

        /// <inheritdoc cref="Adjust"/>
        public void Raise(float A, float B = 0) => sphere.transform.translation.z += (sphere.transform.scale * A + B);
        /// <inheritdoc cref="Adjust"/>
        public void Raise(double A, double B = 0) => Raise((float)A, (float)B);
        /// <inheritdoc cref="Adjust"/>
        public void Lower(float A, float B = 0) => sphere.transform.translation.z -= (sphere.transform.scale * A + B);
        /// <inheritdoc cref="Adjust"/>
        public void Lower(double A, double B = 0) => Lower((float)A, (float)B);

        /// <inheritdoc cref="Adjust"/>
        public void Forward(float A, float B = 0) => sphere.transform.translation.y += (sphere.transform.scale * A + B);
        /// <inheritdoc cref="Adjust"/>
        public void Forward(double A, double B = 0) => Forward((float)A, (float)B);
        /// <inheritdoc cref="Adjust"/>
        public void Backward(float A, float B = 0) => sphere.transform.translation.y -= (sphere.transform.scale * A + B);
        /// <inheritdoc cref="Adjust"/>
        public void Backward(double A, double B = 0) => Backward((float)A, (float)B);


        /// <inheritdoc cref="Adjust"/>
        public void ScaleUp(float A, float B = 0) => sphere.transform.scale += (sphere.transform.scale * A + B);
        /// <inheritdoc cref="Adjust"/>
        public void ScaleUp(double A, double B = 0) => ScaleUp((float)A, (float)B);
        /// <inheritdoc cref="Adjust"/>
        public void ScaleDown(float A, float B = 0) => sphere.transform.scale -= (sphere.transform.scale * A + B);
        /// <inheritdoc cref="Adjust"/>
        public void ScaleDown(double A, double B = 0) => ScaleDown((float)A, (float)B);

        /// <summary>
        /// <code>sphere.transform.scale *= value</code>
        /// </summary>
        /// <param name="value">ratio to adjust</param>
        public void Scale(float value) => sphere.transform.scale *= value;

        /// <inheritdoc cref="Scale(float)"/>
        public void Scale(double value) => Scale((float)value);

    }


}
