using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.NIF
{
    internal class Collision : NIF
    {
        public Collision(VM_GENERAL linker) : base(linker) { }
        public Collision(VM linker) : base(linker) { }

        /// <summary>
        /// GetTriShapes
        /// </summary>
        /// <param name="weight">2, 1, 0, default=use VM.weightNumber</param>
        /// <returns></returns>
        public (int code, string msg) Get(out collider_object[] o, int weight = -1)
        {
            vm.NIFrunning = true;

            string[] filter;
            try { filter = new CBPC.Collision(vm).Filter(); }
            catch (Exception e) {
                o = Array.Empty<collider_object>();
                vm.NIFrunning = false;
                return EE.Parse(e);
            }

            collider_object[] res;
            if (weight < 0) weight = vm.weightNumber;
            switch (weight)
            {
                case 2:
                    try { res = Combine(GetTriShapes(vm.fileNIF_out0, filter), GetTriShapes(vm.fileNIF_out1, filter)); }
                    catch (Exception e)
                    {
                        o = Array.Empty<collider_object>();
                        vm.NIFrunning = false;
                        return EE.Parse(e);
                    }
                    break;
                case 0:
                    res = GetTriShapes(vm.fileNIF_out0, filter);
                    try { res = Combine(res, res); }
                    catch (Exception e)
                    {
                        o = Array.Empty<collider_object>();
                        vm.NIFrunning = false;
                        return EE.Parse(e);
                    }
                    break;
                case 1:
                default:
                    res = GetTriShapes(vm.fileNIF_out1, filter);
                    try { res = Combine(res, res); }
                    catch (Exception e)
                    {
                        o = Array.Empty<collider_object>();
                        vm.NIFrunning = false;
                        return EE.Parse(e);
                    }
                    break;
            }

            o = Mirror(res, filter);
            vm.NIFrunning = false;
            return (0, String.Empty);
        }
        private collider_object[] Mirror(collider_object[] co, string[] filter)
        {
            List<collider_object> all = new();
            Func<string, string> TrimSub = (name) => { if (name.Contains('[') && name.Contains(']')) { return name.Substring(0, Math.Min(name.IndexOf('['), name.IndexOf(']'))); } else { return name; } };
            foreach (collider_object c in co)
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
                    all.Add(new collider_object() { Name = namebuffer, Data = String.Join(' ', databuffer), Write = c.Write, Group = c.Group });
                }
            }
            return all.ToArray();
        }
        private collider_object[] Combine(params collider_object[][]? colObj)
        {
            if (colObj == null) throw new ArgumentNullException(nameof(colObj));

            if (colObj[0].Length != colObj[1].Length) throw EE.New(1001);
            collider_object[] combined = new collider_object[colObj[0].Length];
            for (int i = 0; i < combined.Length; i++)
            {
                if (colObj[0][i].Write != colObj[1][i].Write) throw EE.New(1002);
                if (colObj[0][i].Group != colObj[1][i].Group) throw EE.New(1003);
                if (colObj[0][i].Name != colObj[1][i].Name) throw EE.New(1004);

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
                    combined[i] = new collider_object()
                    {
                        Write = colObj[0][i].Write,
                        Group = colObj[0][i].Group,
                        Name = colObj[0][i].Name,
                        Data = data
                    };
            }
            return combined;
        }
        private collider_object[] GetTriShapes(string file, string[] filter)
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
            List<collider_object> triShapes = new();
            foreach (nifly.NiShape node in niFile.GetShapes())
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
                    if (uniqueName) triShapes.Add(new collider_object(filtered(name), data) { Group = vm.groupfilter.Contains(filtered(name)) });
                }
            }
            return triShapes.ToArray();
        }

    }
    public static class CBPC_collider_object_nameSelector
    {
        static CBPC_collider_object_nameSelector()
        {
            Left = new List<string>() { "NPC L", "L Breast" };
            Right = new List<string>() { "NPC R", "R Breast" };
        }

        public static List<string> Left { get; set; }
        public static List<string> Right { get; set; }

        public enum Side { L = 0, R = 1 };
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
