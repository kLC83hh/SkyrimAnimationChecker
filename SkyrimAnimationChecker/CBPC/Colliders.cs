using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.CBPC
{
    internal class Colliders : File
    {
        public Colliders(VM_GENERAL linker) : base(linker) { }
        public Colliders(VM linker) : base(linker) { }


        private string[] CollisionAll = Array.Empty<string>();

        public string[] Read(bool backup = false)
        {
            CollisionAll = ReadLines(vm.fileCBPC_Collision, backup);
            return CollisionAll;
        }
        public void Save(collider_object[] o, bool overwrite = true)
        {
            if (CanWrite(vm.fileCBPC_Collision, overwrite)) Write(MakeOrganized(o), vm.fileCBPC_Collision, overwrite, vm.CBPCbackup);
        }

        public string[] Filter()
        {
            if (CollisionAll.Length == 0) Read();
            if (CollisionAll.Length == 0) throw EE.New(1, "Can not retrieve CBPC Config data.");
            string[] lines = CollisionAll;
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

        public string MakeOrganized(collider_object[] o)
        {
            vm.CBPCrunning = true;
            if (CollisionAll.Length == 0) Read();
            if (CollisionAll.Length == 0) throw new Exception("Can not retrieve CBPC Config data.");
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
        public string MakeMessy(collider_object[] o)
        {
            vm.CBPCrunning = true;
            List<string> buffer = new();
            foreach (collider_object item in o)
            {
                buffer.Add(string.Join(Environment.NewLine, $"[{item.Name}]", item.Data, string.Empty));
            }
            vm.CBPCrunning = false;
            return string.Join(Environment.NewLine, buffer);
        }
    }
}
