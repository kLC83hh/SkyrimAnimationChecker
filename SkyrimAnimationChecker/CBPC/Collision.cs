using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.CBPC
{
    internal class Collision : CBPC
    {
        public Collision(VM_GENERAL linker) : base(linker) { }
        public Collision(VM linker) : base(linker) { }


        private string[] CollisionAll = Array.Empty<string>();
        private string path => System.IO.Path.Combine(vm.dirCBPC, vm.fileCBPC_Collision);
        public string[] Read(bool backup = false)
        {
            CollisionAll = ReadLines(path, backup);
            return CollisionAll;
        }
        public void Save(collider_object[] o, (cc_options_object, cc_extraoptions_object)? ops, bool overwrite = true)
        {
            if (CanWrite(path, overwrite)) Write(MakeOrganized(o, ops), path, overwrite, vm.CBPCbackup);
        }

        public (cc_options_object, cc_extraoptions_object) Option()
        {
            if (CollisionAll.Length == 0) Read();
            if (CollisionAll.Length == 0) throw EE.New(5201);
            string[] lines = CollisionAll;

            cc_options_object options = new();
            cc_extraoptions_object extraoptions = new();
            string[] opkey = options.Keys;
            string[] exopkey = extraoptions.Keys;

            foreach (string line in lines)
            {
                if (line.StartsWith('#') || line.StartsWith('[')) continue;
                if (line.Contains('='))
                {
                    string[] buffer = line.Split('=').ForEach(x => x.Trim());
                    if (buffer[1].Contains('#')) buffer[1] = buffer[1].Substring(0, buffer[1].IndexOf('#')).Trim();
                    if (buffer.Length == 2)
                    {
                        if (buffer[0] == "Conditions")
                        {
                            options.PropertyHandleSetValue(buffer[0], buffer[1]);
                            options.Use = true;
                        }
                        else if (buffer[0] == "Priority")
                        {
                            try
                            {
                                options.PropertyHandleSetValue(buffer[0], Convert.ToInt32(buffer[1]));
                                options.Use = true;
                            }
                            catch { throw EE.New(5202); }
                        }
                        else if (exopkey.Contains(buffer[0]))
                        {
                            try
                            {
                                extraoptions.PropertyHandleSetValue(buffer[0], Convert.ToDouble(buffer[1]));
                                extraoptions.Use = true;
                            }
                            catch { throw EE.New(5203); }
                        }
                    }
                }
            }
            return (options, extraoptions);
        }

        public string[] Filter()
        {
            if (CollisionAll.Length == 0) Read();
            if (CollisionAll.Length == 0) throw EE.New(5001);
            string[] lines = CollisionAll;
            string[] checker = vm.CBPC_Checker.ToArray();
            if (lines.Contains(checker[0]) || lines.Contains(checker[1]) && lines.Contains(checker[2]))
            {
                List<string> output = new List<string>();
                bool initializer = false;
                foreach (string line in lines)
                {
                    if (initializer)
                    {
                        if (line.StartsWith("["))
                        {
                            string linebuffer = line;
                            if (line.Contains(':')) linebuffer = line.Split(':')[0].Trim();
                            if (linebuffer.EndsWith("]")) output.Add(linebuffer.Substring(1, linebuffer.Length - 2));
                        }
                    }
                    else if (line.StartsWith(checker[0]) || line.StartsWith(checker[1]) || line.StartsWith(checker[2])) initializer = true;
                }
                return output.ToArray();
            }
            return new string[] { };
        }
        public collider_object[] GetColliders()
        {
            if (CollisionAll.Length == 0) Read();
            if (CollisionAll.Length == 0) throw EE.New(5005);
            string[] lines = CollisionAll;
            string[] checker = vm.CBPC_Checker.ToArray();

            List<collider_object> colliders = new List<collider_object>();
            if (lines.Contains(checker[0]) || lines.Contains(checker[1]) && lines.Contains(checker[2]))
            {
                Func<string[], int, int> MoveNextKey = (lines, i) =>
                {
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^[\d-]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    do
                    {
                        i++;
                        if (i >= lines.Length) break;
                    }
                    while (regex.IsMatch(lines[i]));
                    return --i;
                };
                bool initializer = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (initializer)
                    {
                        if (lines[i].StartsWith("["))
                        {
                            string linebuffer = lines[i];
                            if (lines[i].Contains(':')) linebuffer = lines[i].Split(':')[0].Trim();
                            if (linebuffer.EndsWith("]"))
                            {
                                string name = linebuffer.Substring(1, linebuffer.Length - 2);
                                int n = MoveNextKey(lines, i);
                                //M.D($"{i} {n} {lines[n]}");
                                List<string> data = new();
                                for (int j = i + 1; j <= n; j++) data.Add(lines[j]);
                                colliders.Add(new collider_object(name, string.Join(Environment.NewLine, data)) { Group = data.Any(x => x.Contains('&')) });
                            }
                        }
                    }
                    else if (lines[i].StartsWith(checker[0]) || lines[i].StartsWith(checker[1]) || lines[i].StartsWith(checker[2])) initializer = true;
                }
            }

            return colliders.ToArray();
        }

        private List<string> MakeOptions((cc_options_object op, cc_extraoptions_object eop)? ops)
        {
            List<string> buffer = new();
            if (ops != null && ops.Value.op.Use)
            {
                buffer.Add("[Options]");
                buffer.Add($"Conditions={ops.Value.op.Conditions}");
                buffer.Add($"Priority={ops.Value.op.Priority}");
            }
            if (ops != null && ops.Value.eop.Use)
            {
                buffer.Add("[ExtraOptions]");
                foreach (string key in ops.Value.eop.Keys)
                {
                    buffer.Add($"{key}={ops.Value.eop.PropertyHandleGetValue<double>(key)}");
                }
            }
            return buffer;
        }
        public string MakeOrganized(collider_object[] o, (cc_options_object op, cc_extraoptions_object eop)? ops)
        {
            vm.CBPCrunning = true;
            if (CollisionAll.Length == 0) Read();
            if (CollisionAll.Length == 0) throw EE.New(5011);
            string[] lines = CollisionAll;

            Func<string, bool> Found = (name) =>
            {
                bool res = false;
                foreach (var item in o)
                {
                    if (name.StartsWith($"[{item.Name}]")) res = true;
                }
                return res;
            };
            Func<string, string> Data = (name) =>
            {
                string res = string.Empty;
                foreach (var item in o)
                {
                    if (name.StartsWith($"[{item.Name}]")) res = item.Data;
                }
                return res;
            };
            Func<string[], int, int> MoveNextKey = (lines, i) =>
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^[\d-]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                do
                {
                    i++;
                    if (i >= lines.Length) break;
                }
                while (regex.IsMatch(lines[i]));
                return --i;
            };

            List<string> output = new();
            List<string> options = MakeOptions(ops);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains('=') && ops != null)
                {
                    string[] buffer = lines[i].Split('=').ForEach(x => x.Trim());
                    if (buffer.Length == 2)
                    {
                        if (ops.Value.op.Keys.Contains(buffer[0]) || ops.Value.eop.Keys.Contains(buffer[0]))
                        {
                            string comment = string.Empty;
                            if (buffer[1].Contains('#')) comment = buffer[1].Substring(buffer[1].IndexOf('#'));

                            output.Add(
                                string.IsNullOrEmpty(comment)
                                ? options.First(x => x.StartsWith(buffer[0]))
                                : string.Join(' ', options.First(x => x.StartsWith(buffer[0])), comment)
                                );
                        }
                        else output.Add(lines[i]);
                    }
                }
                else if (Found(lines[i]))
                {
                    output.Add(string.Join(Environment.NewLine, lines[i], Data(lines[i])));
                    i = MoveNextKey(lines, i);
                }
                else output.Add(lines[i]);
            }
            vm.CBPCrunning = false;
            return string.Join(Environment.NewLine, output);
        }
        public string MakeMessy(collider_object[] o, (cc_options_object, cc_extraoptions_object)? ops)
        {
            vm.CBPCrunning = true;
            List<string> buffer = new();
            buffer.AddRange(MakeOptions(ops));
            foreach (collider_object item in o)
            {
                buffer.Add(string.Join(Environment.NewLine, $"[{item.Name}]", item.Data, string.Empty));
            }
            vm.CBPCrunning = false;
            return string.Join(Environment.NewLine, buffer);
        }
    }
}
