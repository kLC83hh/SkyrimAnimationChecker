using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.NIF
{
    internal class Collider : NIF
    {
        public Collider(Common.VM_GENERAL linker) : base(linker) { }
        public Collider(Common.VM linker) : base(linker) { }

        /// <summary>
        /// Replace3BA
        /// </summary>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public int Make(int weight = -1, bool? overwrite = null, bool? AutoCalc = null)
        {
            vm.NIFrunning = true;
            string[] localExample = new string[2] {
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "example_0.nif"),
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "example_1.nif")
            };
            int r1 = 1, r2 = 1;
            if (weight < 0) weight = vm.weightNumber;
            if (overwrite == null) overwrite = vm.overwriteInterNIFs;
            string[] bsfile = vm.fileNIF_bodyslide.Split(',').ForEach(x => x.Trim());
            if (bsfile.Length != 2) throw EE.New(1011);
            switch (weight)
            {
                case 2:
                    r1 = Replace3BA(vm.useCustomExample ? vm.fileNIF_sphere0 : localExample[0], System.IO.Path.Combine(vm.dirNIF_bodyslide, bsfile[0]), vm.fileNIF_out0, overwrite);
                    r2 = Replace3BA(vm.useCustomExample ? vm.fileNIF_sphere1 : localExample[1], System.IO.Path.Combine(vm.dirNIF_bodyslide, bsfile[1]), vm.fileNIF_out1, overwrite);
                    break;
                case 1:
                    r2 = Replace3BA(vm.useCustomExample ? vm.fileNIF_sphere1 : localExample[1], System.IO.Path.Combine(vm.dirNIF_bodyslide, bsfile[1]), vm.fileNIF_out1, overwrite);
                    break;
                case 0:
                    r1 = Replace3BA(vm.useCustomExample ? vm.fileNIF_sphere0 : localExample[0], System.IO.Path.Combine(vm.dirNIF_bodyslide, bsfile[0]), vm.fileNIF_out0, overwrite);
                    break;
                default:
                    r1 = 0;
                    break;
            }

            if (AutoCalc == null) AutoCalc = vm.AutoCalcSpheres;
            if ((bool)AutoCalc) CalsSpheres();
            vm.NIFrunning = false;
            return r1 * (r2 > 1 ? r2 + 1 : r2);
        }
        private int Replace3BA(string sphere, string bodyslide, string output, bool? overwrite = null)
        {
            if (overwrite == null) overwrite = false;
            if (System.IO.File.Exists(output) && !(bool)overwrite) return 2;
            if (!System.IO.File.Exists(sphere) || !System.IO.File.Exists(bodyslide)) return 4;
            nifly.NifFile spNI = new nifly.NifFile(), bsNI = new nifly.NifFile();
            spNI.Load(sphere);
            bsNI.Load(bodyslide);
            nifly.vectorNiShape eShapes = spNI.GetShapes(), bsShapes = bsNI.GetShapes();
            for (int i = 0; i < eShapes.Count; i++)
            {
                if (eShapes[i].name.get() == "3BA")
                {
                    nifly.NiShape? shape = null;
                    foreach (var s in bsShapes)
                    {
                        if (s.name.get() == "3BA") { shape = s; break; }
                    }
                    if (shape != null)
                    {
                        nifly.NiAlphaProperty alpha = new();
                        spNI.DeleteShape(eShapes[i]);
                        nifly.NiShape cloned = spNI.CloneShape(shape, shape.name.get(), bsNI);
                        spNI.GetShader(cloned).SetAlpha(0.5f);
                    }
                    break;
                }
            }
            spNI.FinalizeData();
            spNI.Save(output);
            return 1;
        }

        public void UpdateSpheres(Common.collider_object[] colliders)
        {
            nifly.NifFile?[] nifs = new nifly.NifFile?[] { null, null };
            nifs.For((i, x) => {
                if (System.IO.File.Exists(vm.fileNIF_outs[i])) { nifs[i] = new(); nifs[i]?.Load(vm.fileNIF_outs[i]); }
            });
            if (nifs.All(x => x == null)) throw EE.New(1101);

            foreach (var collider in colliders)
            {
                bool groupBackup = collider.Group;
                collider.Group = false;

                string[] spheres = new string[] { collider.Data };
                if (collider.Data.Contains(Environment.NewLine)) spheres = collider.Data.Split(Environment.NewLine).ForEach(x => x.Trim());

                //M.D($"HERE {collider.Name}");
                if (spheres.Length == 1)
                {
                    string[] buffer = spheres[0].Split('|').ForEach(x => x.Trim());
                    if (buffer.Length != 2) continue;
                    nifs.For((i, x) => x?.UpdateSphere(collider.Name, buffer[i]));
                }
                else if (spheres.Length == 2)
                {
                    string[][] buffer = spheres.ForEach(s => s.Split('|').ForEach(x => x.Trim()));
                    if (buffer.Length != 2 || buffer[0].Length != 2 || buffer[1].Length != 2) continue;
                    nifs.For((i, x) => x?.UpdateCapsule(collider.Name, buffer[0][i], buffer[1][i]));
                }

                collider.Group = groupBackup;
            }

            nifs.For((i, x) =>
            {
                string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), vm.fileNIF_outs[i]);
                //string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"test{i}.nif");
                //M.D(path);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                x?.Save(path);
            });
        }


        public nifly.Vector3 Vectorize(string s, nifly.Vector3? defaultVector = null)
        {
            if (!s.Contains(',')) return defaultVector ?? new();
            string[] vals = s.Split(',');
            if (vals.Length != 3) return defaultVector ?? new();
            try
            {
                float x = float.Parse(vals[0]);
                float y = float.Parse(vals[1]);
                float z = float.Parse(vals[2]);
                return new(x, y, z);
            }
            catch { return defaultVector ?? new(); }
        }
        public nifly.Vector3 Vectorize(string s, params float[] defaultValue)
        {
            if (defaultValue.Length == 1) return Vectorize(s, new nifly.Vector3(defaultValue[0], defaultValue[0], defaultValue[0]));
            else if (defaultValue.Length == 3) return Vectorize(s, new nifly.Vector3(defaultValue[0], defaultValue[1], defaultValue[2]));
            else return Vectorize(s, new nifly.Vector3());
        }
        public string[] CalsSpheres()
        {
            nifly.NifFile?[] nifs = new nifly.NifFile?[] { null, null };
            nifs.For((i, x) => {
                if (System.IO.File.Exists(vm.fileNIF_outs[i])) { nifs[i] = new(); nifs[i]?.Load(vm.fileNIF_outs[i]); }
            });
            if (nifs.All(x => x == null)) throw EE.New(1101);

            var result = nifs.For((i, x) =>
            {
                if (x != null) return CalcSphere(x, vm.fileNIF_outs[i], "1,1,1", "1,1,1", false);
                else return Array.Empty<string>();
            }).ToArray();

            return result[0].Concat(new string[] { Environment.NewLine }).Concat(result[1]).ToArray();
        }
        private string[] CalcSphere(nifly.NifFile nif, string path, string sens, string str, bool bounding)
        {
            List<object> msg = new();

            // get skinned shape
            nifly.NiShape[] skins = nif.GetAllSkinned();
            if (skins.Length == 0) goto CalcSphereEnd;
            foreach (var s in skins) nif.GetShader(s).SetAlpha(0.5f);

            // filter, Func, Action
            Action<string, string[]> calc = (name, filter) => msg = CalcInSkin(msg, filter, name, skins, nif, sens, str, bounding);

            calc("3BA", new string[] { "L Breast", "NPC L Butt", "NPC Belly", "Clitoral1", "VaginaB1" });
            calc("Hands", new string[] { "NPC L Finger" });

            nif.Save(path);
        CalcSphereEnd:
            string[] finalout = new string[msg.Count];
            for (int i = 0; i < msg.Count; i++)
            {
                finalout[i] = msg[i].ToString() ?? string.Empty;
#if DEBUG
                M.D(finalout[i]);
#endif
            }
            return finalout;
        }
        private List<object> CalcInSkin(List<object> msg, string[] filter, string name, nifly.NiShape[] skins, nifly.NifFile nif, string sens, string str, bool bounding)
        {
            nifly.NiShape skin = skins.First(x => x.name.get() == name);

            // Func, Action
            Action<string, (nifly.Vector3 center, float radius, string msg)?> UpdateSphereResult = (name, result) => {
                if (result != null)
                {
                    msg.Add($"{name} Succeed center=({result.Value.center.x:N3},{result.Value.center.y:N3},{result.Value.center.z:N3}), r={result.Value.radius:N3}");
                    msg.Add(result.Value.msg + (bounding ? " using BoundingSphere" : string.Empty));
                }
                else msg.Add($"{name} Failed");
            };
            Func<Bone, Bone> getWorkbone = (bone) =>
            {
                if (bone.Name == "VaginaB1")
                {
                    nif.GetFilteredBones(out Bone[] buffer, skins.First(x => x.name.get() == "3BBB_Vagina"), new string[] { bone.Name });
                    return buffer.First(x => x.Name == bone.Name);
                }
                return bone;
            };

            // get skin datas
            var skinDismember = new SkinDismember(nif, skin);
            if (skinDismember.Error) { msg.Add(skinDismember.Reason); return msg; }
            msg.Add($"SkinInstance {skinDismember.Partition.numVertices} {skinDismember.Partition.numPartitions}, skinBones {skinDismember.Data.bones.Count}");

            // bones
            var boneResult = nif.GetFilteredBones(out Bone[] bones, skin, filter);
            if (boneResult <= 0) { msg.Add($"Error: GetFilteredBones {boneResult}"); return msg; }
            msg.Add($"shapeBones {bones.Length}/{boneResult}");
            msg.Add($"sens:{sens} str:{str}");
            float move = 0;
            foreach (var bone in bones.Reverse())
            {
                msg.Add(string.Empty);
                msg.Add($"{bone.Index}: {bone.Name} [{bone.Id}] numVertices {bone.VerticesCount}");
                msg.Add($"{bone.Index}: bounds ({bone.Bound.center.x},{bone.Bound.center.y},{bone.Bound.center.z}) {bone.Bound.radius}");

                if (bone.SpheresCount == 0) { msg.Add($"{bone.Name} Sphere is not found"); continue; }

                (nifly.Vector3 center, float radius, string msg)? result = null;
                if (bounding)
                {
                    if (bone.Sphere != null)
                    {
                        //result = bone.Sphere?.UpdateVertColor();
                        var workbone = getWorkbone(bone);
                        result = nif.UpdateSphere(bone.Sphere, workbone.VerticesVector, workbone.Triangles);
                        //result = bone.Sphere?.UpdateSphere(bone.Bound.center, bone.Bound.radius);
                        UpdateSphereResult(bone.SphereName, result);
                    }
                }
                else
                {
                    foreach (nifly.NiShape sphere in bone.Spheres)
                    {
                        if (sphere.transform.scale == 0) continue;
                        result = nif.UpdateSphere(sphere, getWorkbone(bone).Vertices, Vectorize(sens, 1), Vectorize(str, 1), sphere.name.get().StartsWith("L Breast") ? move : 0);
                        //var result = outFile.UpdateSphere(sphere, skin, new nifly.Vector3(.5f, .1f, .2f), new nifly.Vector3(.33f, 1f, .33f));
                        UpdateSphereResult(sphere.name.get(), result);
                        if (bone.SphereName.StartsWith("L Breast03")) move = (float)(result?.radius ?? 0);
                    }
                }
            }

            return msg;
        }


    }
}
