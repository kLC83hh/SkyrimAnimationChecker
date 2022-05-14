using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nifly;
using h2NIF.Extensions;
using h2NIF.DataStructure;

namespace h2NIF.Sphere
{
    public class Calculator
    {
        #region Initialize
        /// <summary>
        /// <c>nif</c> must have a skinned mesh.
        /// </summary>
        /// <param name="nif"></param>
        /// <exception cref="Exception">skinned meshes are not exists</exception>
        public Calculator(NifFile nif)
        {
            Nif = nif;
            // get all skins
            if (Skins.Count == 0) throw new Exception("This file doesn't have any skinned mesh");
            // set all skin alpha to 0.5
            foreach (var s in Skins.Values) nif.GetShader(s.Shape).SetAlpha(0.5f);
            // make ready micro-adjust action set
            //if (!AdjustManager.IsAvailable) AdjustManager.Create(nif);
        }

        List<bool> Errors = new();
        /// <summary>
        /// <para><c>true</c>: all cleared</para>
        /// <para><c>false</c>: some error has occurred</para>
        /// </summary>
        public bool Result => Errors.All(x => !x);
        List<object> _Message = new();
        public string[] Message
        {
            get
            {
                string[] finalout = new string[_Message.Count];
                for (int i = 0; i < _Message.Count; i++)
                {
                    finalout[i] = _Message[i]?.ToString() ?? string.Empty;
#if DEBUG
                    M.D(finalout[i]);
#endif
                }
                return finalout;
            }
        }
        public string MessageSingle
        {
            get
            {
                StringBuilder sb = new();
                sb.AppendJoin(Environment.NewLine, Message);
                return sb.ToString();
            }
        }

        NifFile Nif { get; set; }
        Dictionary<string, Skin>? _Skins = null;
        Dictionary<string, Skin> Skins
        {
            get
            {
                if (_Skins == null)
                {
                    _Skins = new();
                    var buffer = Nif.GetAllSkinned();
                    foreach(var s in buffer)
                    {
                        var item = new Skin(Nif, s);
                        if (item.Error) { _Message.Add(item.Reason); }
                        _Message.Add($"{item.Name} Instance {item.Partition.numVertices}, Partition {item.Partition.numPartitions}, Bones {item.Data.bones.Count}");
                        _Skins.Add(s.name.get(), item);
                    }
                    //if (_Skins.Any(x => x.Error)) throw new Exception("There are some of invalid skinned meshes");
                }
                return _Skins;
            }
        }
        bool Bounding = false;
        #endregion

        /// <summary>
        /// Logs are stored in <c>Message</c>
        /// </summary>
        /// <param name="sens"></param>
        /// <param name="str"></param>
        /// <param name="bounding"></param>
        /// <returns><para><c>true</c>: all cleared</para>
        /// <para><c>false</c>: some error has occurred</para></returns>
        public bool Calculate(string str, bool bounding)
        {
            Bounding = bounding;
            Action<Dictionary<string, string[]>> calc = (par) =>
            {
                foreach (var pair in par) Errors.Add(CalcSkin(pair.Value, pair.Key, str));
            };
            Dictionary<string, string[]> cPar = new(), bPar = new();

            cPar.Add("3BA", new string[] { "L Breast", "NPC L Butt", "NPC Belly", "Clitoral1", "NPC L Pussy02", "NPC L Calf [LClf]", "NPC L Thigh [LThg]", "NPC L Forearm [LLar]", "NPC L UpperArm [LUar]", "NPC L RearThigh" });
            cPar.Add("3BBB_Vagina", new string[] { "VaginaB1" });
            cPar.Add("Hands", new string[] { "NPC L Finger", "NPC L Hand [LHnd]" });

            //"NPC Spine1 [Spn1]", "NPC Pelvis [Pelv]", "NPC L RearThigh"
            bPar.Add("3BA", new string[] { "NPC L Thigh [LThg]" });
            //bPar.Add("Hands", new string[] { "NPC L Hand [LHnd]" });

            if (Bounding) calc(bPar);
            else calc(cPar);

            return Result;
        }

        private bool CalcSkin(string[] filter, string name, string str)
        {
            // bones
            var boneResult = Skins[name].GetBones(out Bone[] bones, filter);
            if (boneResult <= 0) { _Message.Add($"Error: GetFilteredBones {boneResult}"); return true; }
            _Message.Add($"\nSkin: {name} shapeBones {bones.Length}/{boneResult}, str:{str}");

            //float move = 0;
            //foreach (var bone in bones.Reverse()) _Message.AddRange(CalcBone(bone, str));
            System.Collections.Concurrent.ConcurrentBag<List<string>> msgBag = new();
            bones.AsParallel().ForAll(bone => msgBag.Add(CalcBone(bone, str)));
            //Parallel.ForEach(bones, bone => _Message.AddRange(CalcBone(bone, str)));//
            foreach (var msgs in msgBag) _Message.AddRange(msgs);

            return false;
        }
        private List<string> CalcBone(Bone bone, string str)
        {
            List<string> msg = new();
            msg.Add(string.Empty);
            msg.Add($"{bone.Index}: {bone.Name} [{bone.Id}] numVertices {bone.VerticesCount}");
            msg.Add($"{bone.Index}: bounds ({bone.Bound.center.x},{bone.Bound.center.y},{bone.Bound.center.z}) {bone.Bound.radius}");
            if (bone.SpheresCount == 0) { msg.Add($"{bone.Name} Sphere is not found"); return msg; }

            // Func, Action
            Func<Bone, Bone> getWorkbone = (bone) =>
            {
                if (bone.Name == "VaginaB1")
                {
                    Nif.GetFilteredBones(out Bone[] buffer, Skins["3BBB_Vagina"].Shape, new string[] { bone.Name });
                    return buffer.First(x => x.Name == bone.Name);
                }
                else if (bone.Name.StartsWith("NPC L Hand [LHnd]"))
                {
                    Nif.GetFilteredBones(out Bone[] buffer, Skins["Hands"].Shape, new string[] { bone.Name });
                    return buffer.First(x => x.Name == bone.Name);
                }
                return bone;
            };

            UpdateResult? result = null;
            if (Bounding)
            {
                if (bone.Sphere != null)
                {
                    //result = bone.Sphere?.UpdateVertColor();
                    var workbone = getWorkbone(bone);
                    result = UpdateSphere(bone.Sphere, workbone.VerticesVector, workbone.Triangles);
                    msg.Add(result?.ToString() ?? $"{bone.Sphere.name.get()} Failed to update");
                }
            }
            else
            {
                int n = -1;
                foreach (NiShape sphere in bone.Spheres)
                {
                    if (sphere.transform.scale == 0) continue;
                    result = UpdateSphere(ParamManager.Create(Nif, sphere, getWorkbone(bone).Vertices, str, ++n));
                    msg.Add(result?.ToString() ?? $"{sphere.name.get()} Failed to update");
                    //if (bone.SphereName.StartsWith("L Breast03")) move = (float)(result?.Radius ?? 0);
                }
                if (result != null) CapsuleManager.Create(bone).Invoke();
            }
            return msg;
        }


        private UpdateResult UpdateSphere(BSTriShape sphere, vectorVector3 vertices, vectorTriangle tris)
        {
            MatTransform parent = Nif.GetParentNode(sphere).transform;
#if DEBUG
            M.D($"{sphere.name.get()} {sphere.rawVertices.Count} {sphere.vertData.Count} {sphere.triangles.Count} {sphere.HasVertices()} {sphere.HasNormals()} {sphere.HasTangents()} {sphere.HasVertexColors()}");
            Vector3 pRot = parent.rotation.ToEulerDegrees();
            Vector3 sRot = sphere.transform.rotation.ToEulerDegrees();
            M.D($"({parent.translation.x},{parent.translation.y},{parent.translation.z}),({pRot.x},{pRot.y},{pRot.z})" +
                $" ({sphere.transform.translation.x},{sphere.transform.translation.y},{sphere.transform.translation.z}),({sRot.x},{sRot.y},{sRot.z})");
#endif
            sphere.transform.scale = 1;
            sphere.SetVertexColors(true);

            vectorBSVertexData newVerts = new();
            foreach (Vector3 v in vertices)
            {
                Vector3 local = v;
                local = local.LocalizeTo(parent.translation).RotateBy(parent.rotation.Transpose());
                local = local.LocalizeTo(sphere.transform.translation).RotateBy(sphere.transform.rotation.Transpose());
                newVerts.Add(new BSVertexData() { vert = local, colorData = new arrayuint8_4(new byte[4] { 255, 0, 0, 255 }) });
            }
            sphere.SetVertexData(newVerts);

#if DEBUG
            M.D($"{vertices.Count} {tris.Count}");
#endif
            var newtris = new vectorTriangle();
            foreach (Triangle t in tris)
            {
                if (t.p1 < sphere.vertData.Count && t.p2 < sphere.vertData.Count && t.p3 < sphere.vertData.Count)
                    newtris.Add(t);
            }
            sphere.SetTriangles(newtris);
            //var triIndexes = new vectoruint32(newtris.Count);
            //for (int i = 0; i < triIndexes.Count; i++) { triIndexes[i] = (uint)i; }
            //sphere.ReorderTriangles(triIndexes);

            sphere.UpdateRawNormals();
            sphere.UpdateRawTangents();

            // color emission
            Nif.SetShaderEmissive(sphere, new Color4(0, 0, 1, 1));

#if DEBUG
            M.D($"{sphere.name.get()} {sphere.rawVertices.Count} {sphere.vertData.Count} {sphere.triangles.Count} {sphere.HasVertices()} {sphere.HasNormals()} {sphere.HasTangents()} {sphere.HasVertexColors()}");
#endif
            return new UpdateResult(sphere, "Manual update");
        }
        private UpdateResult UpdateSphere(UpdateParam par)
        {
            // parent vector
            MatTransform parent = Nif.GetParentNode(par.Sphere).GetTransformToParent();

            // deep copy vertices
            Vertex[] vertices = par.Vertices.Deepcopy();

            // select vertices
#if DEBUG
            int vCountAll = vertices.Length;
#endif
            char? side = par.Sphere.GetSide();
            vertices.FilterSide(side);
#if DEBUG
            int vCountSided = vertices.Length;
#endif
            vertices = vertices.LocalizeTo(parent.translation).RotateBy(parent.rotation.Transpose());
#if DEBUG
            int vCountLocalized = vertices.Length;
#endif
            Vector3 window = vertices.Width().opMult(par.WindowSensitivity);
            vertices = vertices.WindowBy(window, par.WindowBase);
            string msg = string.Empty;
#if DEBUG
            int vCountWindowed = vertices.Length, vCountDiff = vCountWindowed - vCountAll;
            string sidenote = side == null ? "" : $"{side} sided ";
            string vCountnote = string.Empty;
            if (vCountWindowed != vCountLocalized) vCountnote += $"w{vCountWindowed}/";
            if (vCountLocalized != vCountSided) vCountnote += $"l{vCountLocalized}/";
            if (vCountSided != vCountAll) vCountnote += $"s{vCountSided}/";
            string vCountDiffnote = vCountDiff < 0 ? $" ({vCountDiff}) {((float)vCountWindowed / (float)vCountAll) * 100f:N3}%" : string.Empty;
            string weighted = par.Weight ? "" : ", unweighted";
            msg = ($"using {sidenote}{vCountnote}{vCountAll}{vCountDiffnote} vertices windowed by ({window.x:N3},{window.y:N3},{window.z:N3})[{par.RawWindowSensitivity},{par.WindowBase}]{weighted}");
#endif
            // calc
            par.Sphere.transform.translation = vertices.Center(par.Weight).Strengthen(par.Strength);
            par.Sphere.transform.scale = vertices.AverageDistanceTo(par.Sphere.transform.translation, par.Weight);
            //MicroAdjust(sphere, parent);
            //if (AdjustSet.IsAvailable) AdjustSet.Get(par.Name).Invoke(par.Sphere);
            par.Adjust();

            // color emission
            Nif.SetShaderEmissive(par.Sphere, new Color4(0, 0, 1, 1));

            return new UpdateResult(par.Sphere, msg);
        }
    }
    internal static class ParamManager
    {
        public static UpdateParam Create(NifFile nif, NiShape sphere, Vertex[] vertices, string strength, int n = -1)
        {
            int[][] sector = { new[] { n } };
            string? axes = null;
            string wSens = "1,1,1", wBase = "center";//min,max,center
            bool weight = true;//axes == null

            if (sphere.name.get().StartsWith("NPC L Pussy")) axes = "y--";
            else if (sphere.name.get().StartsWith("NPC L Calf [LClf]")) { axes = "z"; wSens = "1,1,0.33"; wBase = "max"; }
            else if (sphere.name.get().StartsWith("NPC L Thigh [LThg]")) { axes = "z"; wSens = "1,1,0.33"; }
            else if (sphere.name.get().StartsWith("NPC L RearThigh")) { axes = "z"; }
            else if (sphere.name.get().StartsWith("NPC L Hand [LHnd]"))
            {
                axes = "yz";
                //if (n == 0) sector = new[] { new[] { 1, 0 } };
                //else if (n == 1) sector = new[] { new[] { 0, 1 }, new[] { 0, 0 }, new[] { 1, 1 } };
                if (n == 0) sector = new[] { new[] { 0, 0 } };
                else if (n == 1) sector = new[] { new[] { 1, 1 } };
            }
            else if (sphere.name.get().StartsWith("NPC L UpperArm [LUar]"))
            {
                axes = "xz";
                wSens = "1,1,0.5";
                weight = false;
                if (n == 0) { sector = new[] { new[] { 1, 1 } }; wBase = "min"; }
                else if (n == 1) { sector = new[] { new[] { 1, 1 } }; wBase = "max"; }
            }
            else if (sphere.name.get().StartsWith("NPC L Forearm [LLar]"))
            {
                axes = "xz";
                wSens = "1,1,0.5";
                weight = false;
                if (n == 0) { sector = new[] { new[] { 0, 0 } }; wBase = "max"; }
                else if (n == 1) { sector = new[] { new[] { 1, 1 } }; wBase = "min"; }
            }

            return new UpdateParam(nif, sphere, vertices, strength, wSens, wBase, weight, axes?.ToCharArray(), sector);
        }
    }
    internal static class AdjustManager
    {
        private static Action nullAction = new Action(() => { });
        public static Action Create(string name, NiShape sphere, NiNode parent)
        {
            //SphereAdjust adjust = new SphereAdjust(sphere);
            // scaling
            // translating, this should be done after scaling
            if (name.StartsWith("L Breast") || name.StartsWith("R Breast"))
            {
                return new(() =>
                {
                    SphereAdjust adjust = new SphereAdjust(sphere);
                    // scaling
                    if (name.Contains('1')) adjust.Scale(0.9);

                    // translating, this should be done after scaling
                    adjust.Gather(0.16);
                    adjust.Raise(-0.3);
                    adjust.Backward(0.33);
                    if (name.Contains('1'))
                    {
                        adjust.Gather(0.08);
                        adjust.Raise(0.15);
                    }
                    if (name.Contains('2'))
                    {
                        adjust.Spread(0.08, -0.2);
                        adjust.Raise(0.1, 0.1);
                        adjust.Forward(0.06, -0.3);
                        adjust.ScaleUp(0.08, -0.6);
                    }
                });
            }
            else if (name.StartsWith("NPC L Butt") || name.StartsWith("NPC R Butt"))
            {
                return new(() =>
                {
                    SphereAdjust adjust = new SphereAdjust(sphere);
                    // scaling
                    adjust.ScaleUp(0.35, -0.975);

                    // translating, this should be done after scaling
                    adjust.Spread(0.11);
                    adjust.Forward(0.58);
                    adjust.Raise(0.55);
                });
            }
            else if (name.StartsWith("NPC Belly"))
            {
                return new(() =>
                {
                    SphereAdjust adjust = new SphereAdjust(sphere);
                    // scaling
                    adjust.ScaleUp(0.5);

                    // translating, this should be done after scaling
                    adjust.Lower(0.5);
                    adjust.Backward(0, 1);
                });
            }
            else if (name.StartsWith("VaginaB1"))
            {
                return new(() =>
                {
                    SphereAdjust adjust = new SphereAdjust(sphere);
                    // scaling
                    adjust.ScaleDown(0.15);
                    // translating, this should be done after scaling
                    adjust.Lower(1);
                    adjust.Forward(0.2);
                });
            }
            else if (name.StartsWith("Clitoral1"))
            {
                return new(() =>
                {
                    SphereAdjust adjust = new SphereAdjust(sphere);
                    // scaling
                    adjust.ScaleUp(0.15);
                    // translating, this should be done after scaling
                    adjust.Lower(0.3);
                });
            }
            else return nullAction;
        }
    }
    internal static class CapsuleManager
    {
        private static Action nullAction = new Action(() => { });
        public static Action Create(Bone bone)
        {
            if (bone.Name.StartsWith("NPC L Pussy"))
            {
                return new(() =>
                {
                    CapsuleAdjust adjust = new CapsuleAdjust(bone);
                    // scaling
                    adjust.ScaleUp(0.2);
                    // translating, this should be done after scaling
                    adjust.Spread('y', 0, 0.67);
                    adjust.Backward(0, 0.2);

                    MatTransform tf = bone.Node.GetTransformToParent();
                    foreach (NiShape sphere in bone.Spheres)
                    {
                        //if (Math.Abs(sphere.transform.translation.x + tf.translation.x) < sphere.transform.scale)
                        //    adjust.Spread(0, sphere.transform.scale - Math.Abs(sphere.transform.translation.x + tf.translation.x));
                        sphere.transform.translation.x = -tf.translation.x - sphere.transform.scale;
                    }
                });
            }
            else if (bone.Name.StartsWith("NPC L Hand [LHnd]"))
            {
                return new(() =>
                {
                    // this sphere's parent node is rotated along with the hand, x=fore-back, y=in-out, z=arm-hand
                    CapsuleAdjust adjust = new CapsuleAdjust(bone);
                    // scaling
                    //adjust.ScaleUp(0.2);
                    // translating, this should be done after scaling
                    adjust.Gather('y', 0.5);
                    adjust.Forward(-0.1, 1);
                    adjust.Lower(0, 2);
                });
            }
            else if (bone.Name.StartsWith("NPC L Calf [LClf]"))
            {
                return new(() =>
                {
                    // this sphere's parent node is upside down
                    CapsuleAdjust adjust = new CapsuleAdjust(bone);
                    // scaling
                    adjust.ScaleDown(0.05);
                    //translating, this should be done after scaling
                    adjust.Gather('z', 0.45);
                    adjust.Raise(0.55);
                    adjust.Backward(0.15, -0.5);
                    adjust.Backward(0.15);
                });
            }
            else if (bone.Name.StartsWith("NPC L Thigh [LThg]"))
            {
                return new(() =>
                {
                    // this sphere's parent node is upside down, left-right flipped, and biased in z axis
                    CapsuleAdjust adjust = new CapsuleAdjust(bone);
                    // scaling
                    adjust.ScaleDown(0.1);
                    // translating, this should be done after scaling
                    adjust.Gather('z', 0.5);
                    //adjust.Spread('x', 0.16);
                    adjust.Right(0.92, -4.75);
                    adjust.Lower(0, -1);
                    adjust.Right(0, 0.5);
                    adjust.Forward(0.05, 0.5);
                });
            }
            else if (bone.Name.StartsWith("NPC L RearThigh"))
            {
                return new(() =>
                {
                    // this sphere's parent node is upside down, left-right flipped, and biased in z axis
                    CapsuleAdjust adjust = new CapsuleAdjust(bone);
                    // scaling
                    adjust.ScaleUp(0.5, -3);
                    // translating, this should be done after scaling
                    adjust.Gather('x', 0.2);
                    adjust.Right(0.2);
                    adjust.Forward(0.5);
                });
            }
            else if (bone.Name.StartsWith("NPC L UpperArm [LUar]"))
            {
                return new(() =>
                {
                    // this sphere's parent node is rotated along with the forearm, x=in-out, y=fore-back, z=arm-hand
                    CapsuleAdjust adjust = new CapsuleAdjust(bone);
                    // scaling
                    //adjust.ScaleUp(0.5, -1);
                    // translating, this should be done after scaling
                    adjust.Spread('x', 0.2);
                    adjust.Gather('y', 0, 0.3);
                    adjust.Spread('z', 1.5);
                    adjust.Right(0, 1.5);
                });
            }
            else if (bone.Name.StartsWith("NPC L Forearm [LLar]"))
            {
                return new(() =>
                {
                    // this sphere's parent node is rotated along with the forearm, x=in-out, y=fore-back, z=arm-hand
                    CapsuleAdjust adjust = new CapsuleAdjust(bone);
                    // scaling
                    adjust.ScaleUp(1, -2.5);
                    // translating, this should be done after scaling
                    adjust.Gather('x', 0, 1.125);
                    adjust.Right(0, 0.5);
                });
            }
            else return nullAction;
        }
    }
}
