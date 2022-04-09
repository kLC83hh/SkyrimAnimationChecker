using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker
{
    internal class CBPC
    {
        VM vm;
        public CBPC(VM linker) => vm = linker;

        private string[] All = Array.Empty<string>();
        public string[] ReadLines(bool backup = false)
        {
            string path = vm.fileCBPC;
            if (!System.IO.File.Exists(path)) return Array.Empty<string>();
            if (backup) Backup();
            using (System.IO.StreamReader sr = new(path))
            {
                //while (!sr.EndOfStream) { string? buffer = sr.ReadLine(); M.D(buffer); lines.Add(buffer?.Trim() ?? string.Empty); }
                string buffer = sr.ReadToEnd();
                All = buffer.Split(Environment.NewLine);
            }
            //M.D(lines.Count);
            return All;
        }
        public void Save(CBPC_collider_object[] o, bool overwrite = true)
        {
            string path = vm.fileCBPC;
            if (System.IO.File.Exists(path) && !overwrite) return;
            string data = MakeOrganized(o);
            using (System.IO.StreamWriter sw = new(path))
            {
                sw.Write(data);
            }
        }

        public string[] Filter()
        {
            if (All.Length == 0) ReadLines();
            if (All.Length == 0) throw new Exception("Can not retrieve CBPC Config data.");
            string[] lines = All;
            //string[] checker = { "# Collision spheres", "# Affected Nodes", "# Collider Nodes" };
            string[] checker = vm.CBPC_Checker.ToArray();
            if (lines.Contains(checker[0]) || lines.Contains(checker[1]) && lines.Contains(checker[2]))
            {
                List<string> output = new List<string>();
                //string lineName = string.Empty;
                //List<string> lineNumbers = new List<string>();
                bool initializer = false;
                foreach (string line in lines)
                {
                    //M.D(line);
                    if (initializer)
                    {
                        if (line.StartsWith("[") && line.EndsWith("]")) output.Add(line.Substring(1, line.Length - 2));
                    }
                    else if (line.StartsWith(checker[0]) || line.StartsWith(checker[1]) || line.StartsWith(checker[2])) initializer = true;
                }
                return output.ToArray();
            }
            return new string[] { };
        }

        private void Backup() => System.IO.File.Copy(vm.fileCBPC, System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), vm.fileCBPC.Split('\\').Last()), true);
        public string MakeOrganized(CBPC_collider_object[] o)
        {
            vm.CBPCrunning = true;
            if (All.Length == 0) ReadLines();
            if (All.Length == 0) throw new Exception("Can not retrieve CBPC Config data.");
            string[] lines = All;
            if (vm.CBPCbackup) Backup();

            Func<string, bool> Found = (name) =>
            {
                bool res = false;
                foreach (var item in o) {
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
            Func<string[], int, int> MoveNextKey = (lines, i) => {
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
            for (int i = 0; i < lines.Length; i++)
            {
                if (Found(lines[i]))
                {
                    string buffer = string.Join(Environment.NewLine, lines[i], Data(lines[i]));
                    output.Add(buffer);
                    i = MoveNextKey(lines, i);
                }
                else output.Add(lines[i]);
            }
            vm.CBPCrunning = false;
            return string.Join(Environment.NewLine, output);
        }
        public string MakeMessy(CBPC_collider_object[] o)
        {
            vm.CBPCrunning = true;
            List<string> buffer = new();
            foreach (CBPC_collider_object item in o)
            {
                buffer.Add(string.Join(Environment.NewLine, $"[{item.Name}]", item.Data, string.Empty));
            }
            vm.CBPCrunning = false;
            return string.Join(Environment.NewLine, buffer);
        }
    }
}
