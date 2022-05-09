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
                    if (!nif.GetTriShapes().Any(x => x.name.get().EndsWith(nameEnd))) return Array.Empty<BSTriShape>();
                    else return nif.GetTriShapes().Where(x => x.name.get().StartsWith(name)).Where(x => x.name.get().EndsWith(nameEnd)).ToArray();
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
        public static Vector3 Center(this Vector3[] arr, SkinWeight[]? weights = null)
        {
            if (weights == null) return arr.Mean();
            else return arr.MeanWeighted(weights);
        }
        /// <summary>
        /// Localized only
        /// </summary>
        /// <param name="center"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        public static Vector3 Strengthen(this Vector3 center, Vector3 strength) => new Vector3(center.x * strength.x, center.y * strength.y, center.z * strength.z);
        public static float AverageDistanceTo(this Vector3[] arr, Vector3 target)
        {
            float sum = 0;
            foreach (Vector3 v in arr)
            {
                sum += v.DistanceTo(target);
            }
            return sum / arr.Length;
        }

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
            Vector3[] vertices = new Vector3[vdArray.Length];
            for (int i = 0; i < vdArray.Length; i++) vertices[i] = vdArray[i].vert;

            return UpdateSphere(nif, sphere, vertices, Array.Empty<SkinWeight>(), sensitivity, strength);
        }
        public static (Vector3 center, float radius, string msg)? UpdateSphere(this NifFile nif, NiShape sphere, Vector3[] vertices, SkinWeight[] weights, Vector3 sensitivity, Vector3 strength, float move = 0)
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
            sphere.transform.translation = Center(vertices).Strengthen(strength);
            sphere.transform.scale = vertices.AverageDistanceTo(sphere.transform.translation);

            // update
            //sphere.transform.translation = center;
            //sphere.transform.scale = radius;

            // micro adjust
            if (sphere.name.get().StartsWith("L Breast") || sphere.name.get().StartsWith("R Breast"))
            {
                // scaling
                if (sphere.name.get().Contains('1')) sphere.transform.scale *= 0.90f;
                // translating
                move = sphere.transform.scale / 100f;
                if (side == 'L') sphere.transform.translation.x += move * 16f;
                else if (side == 'R') sphere.transform.translation.x -= move * 16f;
                sphere.transform.translation.y -= move * 33f;
                sphere.transform.translation.z -= move * 30f;
                if (sphere.name.get().Contains('1'))
                {
                    sphere.transform.translation.z += move * 15f;
                }
                if (sphere.name.get().Contains('2'))
                {
                    sphere.transform.translation.y -= move * 10f;
                }
            }
            //float div = 3;
            //if (move == 0) move = radius;
            //if (side == 'L') sphere.transform.translation.x += move / div;
            //else if (side == 'R') sphere.transform.translation.x -= move / div;
            //sphere.transform.translation.y -= move / div;
            //sphere.transform.translation.z -= move / div;

            // color emission
            nif.SetShaderEmissive(sphere, new Color4(0, 0, 1, 1));
            //if (sphere.HasShaderProperty())
            //{
            //    var shader = nif.GetBlock<BSLightingShaderProperty>(sphere.ShaderPropertyRef());
            //    if (shader != null && shader.IsEmissive())
            //    {
            //        shader.SetEmissiveMultiple(1);
            //        shader.SetEmissiveColor(new Color4(0, 0, 1, 1));
            //    }
            //}

            return (sphere.transform.translation, sphere.transform.scale, msg);
        }
        //public static (string msg)? UpdateSpheres(this NifFile nif, NiShape[] spheres, Vector3[] vertices, Vector3 sensitivity, Vector3 strength)
        //{
        //    foreach (NiShape sphere in spheres) { UpdateSphere(nif, sphere, vertices, sensitivity, strength); }
        //}
        public static bool MakeSphere(this NifFile nif, string name, Vector3[] vertices, float[] heightLimit, NiNode? parent = null)
        {
            NiNode? node = nif.GetNode(name);
            if (node == null) return false;

            Vector3 ts = node.GetTransformToParent().translation;
            float hwindow = heightLimit[0] * heightLimit[1] / 2;

            // calc
            List<Vector3> pts = new();
            foreach (Vector3 v in vertices)
            {
                if (name.StartsWith('L') && v.x > 0) continue;
                else if (name.StartsWith('R') && v.x < 0) continue;

                if (Math.Abs(v.y - ts.y) < 0.5 && Math.Abs(v.z - ts.z) < hwindow) pts.Add(v);
            }
            Vector3 av = Center(pts.ToArray());
            float d = AverageDistanceTo(pts.ToArray(), av);

            // make container node
            uint id = nif.CloneNamedNode(node.name.get());
            nif.SetNodeName(id, node.name.get() + " Location");
            nifly.NiNode? newnode = nif.GetNode(node.name.get() + " Location");
            if (newnode == null) return false;
            nif.SetParentNode(newnode, parent ?? nif.GetParentNode(node));
            //nif.AddNode(, new MatTransform() { translation = av, scale = d }, parent ?? nif.GetParentNode(node));

            // make sphere shape
            BSTriShape newshape = new();
            newshape.transform.translation = av.opSub(ts);
            newshape.transform.scale = d;
            newshape.SetVertices(true);
            newshape.SetNormals(true);
            newshape.SetTangents(true);
            newshape.UpdateBounds();
            newshape.UpdateRawVertices();
            M.D(newshape.GetBlockName());
            nif.CloneShape(newshape, node.name.get() + " Sphere");
            //VertexDesc vd = new();
            //vd.SetFlags(VertexFlags.VF_VERTEX | VertexFlags.VF_NORMAL | VertexFlags.VF_TANGENT);
            //BSTriShape bsts = new();

            //vectorVector2 uv = new(2);
            //for (int i = 0; i < uv.Count; i++) { uv[i].u = (float)i; uv[i].v = (float)i; }

            //NiShape newshape = nif.CreateShapeFromData(name + " Sphere",av.opSub(ts), , uv);
            nif.SetParentNode(newshape, newnode);

            return true;
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
        public SkinWeight GetWeight(Vector3 vertex) => Weights[_Vertices.IndexOf(vertex)];

        public int VerticesCount => _Vertices.Count;
        public Vector3[] Vertices => _Vertices.ToArray();
        public vectorVector3 VerticesVector
        {
            get
            {
                vectorVector3 buffer = new();
                foreach (Vector3 v in _Vertices) buffer.Add(v);
                return buffer;
            }
        }
        private List<Vector3> _Vertices = new();
        private void GetVertices()
        {
            foreach (SkinWeight sw in Weights)
            {
                _Vertices.Add(SkinPartition.vertData[sw.index].vert);
            }
        }

        public vectorTriangle Triangles => SkinPartition.partitions[0].triangles;
    }



}
