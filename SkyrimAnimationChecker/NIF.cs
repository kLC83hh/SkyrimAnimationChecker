using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker
{
    internal class NIF
    {
        VM vm;
        public NIF(VM linker) => vm = linker;

        /// <summary>
        /// Replace3BA
        /// </summary>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public int Run1(int weight = 1, bool overwrite = false)
        {
            vm.NIFrunning = true;
            string[] localExample = new string[2] {
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "example_0.nif"),
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "example_1.nif")
            };
            int r1 = 1, r2 = 1;
            switch (weight)
            {
                case 2:
                    r1 = Replace3BA(vm.useCustomExample ? localExample[0] : vm.fileNIF_sphere0, vm.fileNIF_bodyslide0, vm.fileNIF_out0, overwrite);
                    r2 = Replace3BA(vm.useCustomExample ? localExample[1] : vm.fileNIF_sphere1, vm.fileNIF_bodyslide1, vm.fileNIF_out1, overwrite);
                    break;
                case 1:
                    r2 = Replace3BA(vm.useCustomExample ? localExample[1] : vm.fileNIF_sphere1, vm.fileNIF_bodyslide1, vm.fileNIF_out1, overwrite);
                    break;
                case 0:
                    r1 = Replace3BA(vm.useCustomExample ? localExample[0] : vm.fileNIF_sphere0, vm.fileNIF_bodyslide0, vm.fileNIF_out0, overwrite);
                    break;
                default:
                    r1 = 0;
                    break;
            }
            vm.NIFrunning = false;
            return r1 * (r2 > 1 ? r2 + 1 : r2);
        }
        private int Replace3BA(string sphere, string bodyslide, string output, bool overwrite = false)
        {
            if (System.IO.File.Exists(output) && !overwrite) return 2;
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
                        spNI.DeleteShape(eShapes[i]);
                        spNI.CloneShape(shape, shape.name.get(), bsNI);
                        //new nifly.NiAlphaProperty().Put(0.34);
                        //new nifly.BSLightingShaderProperty(eNI.GetHeader().GetVersion()).
                        //eNI.GetAlphaProperty(shape).Put(new nifly.NiOStream());
                    }
                    break;
                }
            }
            spNI.FinalizeData();
            spNI.Save(output);
            return 1;
        }

        /// <summary>
        /// GetTriShapes
        /// </summary>
        /// <param name="weight">0,1,2</param>
        /// <returns></returns>
        public CBPC_collider_object[] Run2(int weight = 1)
        {
            vm.NIFrunning = true;
            string[] filter = new CBPC(vm).Filter();
            CBPC_collider_object[] res = new CBPC_collider_object[filter.Length];
            switch (weight)
            {
                case 2:
                    res = Combine(GetTriShapes(vm.fileNIF_out0, filter), GetTriShapes(vm.fileNIF_out1, filter));
                    break;
                case 0:
                    res = GetTriShapes(vm.fileNIF_out0, filter);
                    res = Combine(res, res);
                    break;
                case 1:
                default:
                    res = GetTriShapes(vm.fileNIF_out1, filter);
                    res = Combine(res, res);
                    break;
            }
            vm.NIFrunning = false;
            return Mirror(res, filter);
        }
        private CBPC_collider_object[] Mirror(CBPC_collider_object[] co, string[] filter)
        {
            List<CBPC_collider_object> all = new();
            Func<string, string> TrimSub = (name) => { if (name.Contains('[') && name.Contains(']')) { return name.Substring(0, Math.Min(name.IndexOf('['), name.IndexOf(']'))); } else { return name; } };
            foreach (CBPC_collider_object c in co)
            {
                all.Add(c);
                if (CBPC_collider_object_nameSelector.Check(c.Name, CBPC_collider_object_nameSelector.Side.L))
                {
                    string namebuffer = c.Name;
                    for (int i = 0; i < CBPC_collider_object_nameSelector.Left.Count; i++)
                    {
                        if (c.Name.Contains(CBPC_collider_object_nameSelector.Left[i]))
                            namebuffer = filter.First(n => n.Contains(TrimSub(c.Name.Replace(CBPC_collider_object_nameSelector.Left[i], CBPC_collider_object_nameSelector.Right[i]))));
                    }
                    string[] databuffer = c.Data.Split(' ').ForEach(x => x.Trim());
                    for (int i = 0; i < databuffer.Length; i++)
                    {
                        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^[\d-]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        if (regex.IsMatch(databuffer[i]))
                        {
                            if (databuffer[i].StartsWith('-')) databuffer[i] = databuffer[i].Substring(1);
                            else databuffer[i] = $"-{databuffer[i]}";
                        }
                    }
                    all.Add(new CBPC_collider_object() { Name = namebuffer, Data = String.Join(' ', databuffer), Write = c.Write, Group = c.Group });
                }
            }
            return all.ToArray();
        }
        private CBPC_collider_object[] Combine(params CBPC_collider_object[][]? colObj)
        {
            if (colObj == null) throw new ArgumentNullException(nameof(colObj));

            if (colObj[0].Length != colObj[1].Length) throw new Exception("Invalid data: Length of both outputs are different.");
            CBPC_collider_object[] combined = new CBPC_collider_object[colObj[0].Length];
            for (int i = 0; i < combined.Length; i++)
            {
                if (colObj[0][i].Write != colObj[1][i].Write) throw new Exception("Invalid data: Write statuses are different.");
                if (colObj[0][i].Group != colObj[1][i].Group) throw new Exception("Invalid data: Group statuses are different.");
                if (colObj[0][i].Name != colObj[1][i].Name) throw new Exception("Invalid data: Sphere names are different.");

                string data = string.Empty;
                if (colObj[0][i].Data.Contains(Environment.NewLine) && colObj[1][i].Data.Contains(Environment.NewLine) &&
                    colObj[0][i].Data.Count(Environment.NewLine) == colObj[1][i].Data.Count(Environment.NewLine)
                    )
                {
                    string[] buffer0 = colObj[0][i].Data.Split(Environment.NewLine).ForEach(x => x.Trim());
                    string[] buffer1 = colObj[1][i].Data.Split(Environment.NewLine).ForEach(x => x.Trim());
                    string[] buffer = new string[buffer0.Length];
                    for (int j = 0; j < buffer.Length; j++) buffer[j] = string.Join(" | ", buffer0[j], buffer1[j]);
                    data = string.Join(Environment.NewLine, buffer);
                }
                else if (!colObj[0][i].Data.Contains(Environment.NewLine) && !colObj[1][i].Data.Contains(Environment.NewLine))
                    data = $"{colObj[0][i].Data} | {colObj[1][i].Data}";
                if (!string.IsNullOrEmpty(data))
                    combined[i] = new CBPC_collider_object()
                    {
                        Write = colObj[0][i].Write,
                        Group = colObj[0][i].Group,
                        Name = colObj[0][i].Name,
                        Data = data
                    };
            }
            return combined;
        }
        private CBPC_collider_object[] GetTriShapes(string file, string[] filter)
        {
            Func<string, bool> nameCheck = (name) => {
                if (!CBPC_collider_object_nameSelector.Right.ForAll(x => !name.Contains(x))) return false;
                foreach (string s in filter)
                {
                    if (name.StartsWith(s)) return true;
                }
                return false;
            };
            Func<string, string> filtered = (name) =>
            {
                foreach (string s in filter)
                {
                    if (name.StartsWith(s)) return s;
                }
                return name;
            };
            nifly.NifFile niFile = new nifly.NifFile();
            niFile.Load(file);
            //N.Text = niFile.GetHeader().GetVersion().IsSSE().ToString();
            M.D(niFile.GetNodes().Count);
            List<CBPC_collider_object> triShapes = new();
            foreach (var node in niFile.GetShapes())
            {
                string name = node.name.get().Trim();
                if (name.EndsWith("Sphere")) name = name.Substring(0, name.Length - 6).Trim();
                if (name.EndsWith("Sphere2")) name = name.Substring(0, name.Length - 7).Trim();
                //M.D(name);
                if (nameCheck(name) && node.transform.scale != 0)
                {
                    var t = node.transform.translation;
                    string data = $"{t.x},{t.y},{t.z},{node.transform.scale}";
                    bool uniqueName = true;
                    for (int i = 0; i < triShapes.Count; i++)
                    {
                        if (triShapes[i].Name == $"{filtered(name)}")
                        {
                            if (triShapes[i].Group) triShapes[i].Data += $" & {data}";
                            else triShapes[i].Data += $"{Environment.NewLine}{data}";
                            uniqueName = false;
                        }
                    }
                    if (uniqueName) triShapes.Add(new CBPC_collider_object(filtered(name), data) { Group = vm.groupfilter.Contains(filtered(name)) });
                }
            }
            return triShapes.ToArray();
        }

    }
    //public class CBPC_collider_object_nameSelector : Notify.NotifyPropertyChanged
    //{
    //    public CBPC_collider_object_nameSelector()
    //    {
    //        Left = new System.Collections.ObjectModel.ObservableCollection<string>() { "NPC L", "L Breast" };
    //        Right = new System.Collections.ObjectModel.ObservableCollection<string>() { "NPC R", "R Breast" };
    //    }

    //    public System.Collections.ObjectModel.ObservableCollection<string> Left
    //    {
    //        get => Get<System.Collections.ObjectModel.ObservableCollection<string>>();
    //        set => Set(value);
    //    }
    //    public System.Collections.ObjectModel.ObservableCollection<string> Right
    //    {
    //        get => Get<System.Collections.ObjectModel.ObservableCollection<string>>();
    //        set => Set(value);
    //    }
    //}
    public static class CBPC_collider_object_nameSelector
    {
        static CBPC_collider_object_nameSelector()
        {
            Left = new List<string>() { "NPC L", "L Breast" };
            Right = new List<string>() { "NPC R", "R Breast" };
        }

        public static List<string> Left { get; set; }
        public static List<string> Right { get; set; }

        public enum Side { L= 0, R= 1 };
        public static bool Check(string name, Side side)
        {
            switch (side)
            {
                case Side.L:
                    foreach (string s in Left)
                    {
                        if (name.StartsWith(s)) return true;
                    }
                    break;
                case Side.R:
                    foreach (string s in Right)
                    {
                        if (name.StartsWith(s)) return true;
                    }
                    break;
            }
            return false;
        }
    }

}
