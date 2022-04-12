using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.CBPC
{
    public class FPhysics : File
    {
        public FPhysics(VM_GENERAL linker) : base(linker) { }
        public FPhysics(VM linker) : base(linker) { }


        private string[] PhysicsAll = Array.Empty<string>();
        public string[] Read(bool backup = false, bool force = false)
        {
            if (PhysicsAll == null || PhysicsAll.Length == 0 || force)
                PhysicsAll = ReadLines(vm.fileCBPC_Physics, backup);
            return PhysicsAll;
        }
        public int Save(object o, bool overwrite = false)
        {
            string? data = null;

            if (o is breast_3ba_object c) data = Make3BA(c);

            if (string.IsNullOrEmpty(data)) return 2;

            if (CanSave(vm.fileCBPC_Physics, overwrite)) Save(data, vm.fileCBPC_Physics, overwrite, vm.CBPCbackup);
            else return 4;
            return 0;
        }


        public breast_3ba_object GetPhysics() => Parse3BA();
        private breast_3ba_object Parse3BA()
        {
            if (PhysicsAll.Length == 0) Read();
            if (PhysicsAll.Length == 0) throw new Exception("Can not retrieve CBPC Config data.");
            string[] lines = PhysicsAll;
            //vm.CBPC_Physics_running = true;

            Func<string, (string, string, double[])> parse = (line) =>
            {
                string[] part = line.Split(' ').ForEach(x => x.Trim());
                if (part.Length != 3 && part.Length != 2) throw new Exception($"Invalid data: {line}");
                string[] vs = part[0].Split('.').ForEach(x => x.Trim());
                if (vs.Length != 2) throw new Exception($"Invalid data: {line}");

                string name = vs[0];
                string property = vs[1];

                string[] dBuffer = new string[2] { part[1], part[1] };
                if (part.Length == 3) dBuffer[1] = part[2];

                double[] buffer = new double[dBuffer.Length];
                try
                {
                    for (int i = 0; i < dBuffer.Length; i++) buffer[i] = Convert.ToDouble(dBuffer[i]);
                }
                catch (Exception)
                {
                    throw new Exception($"Invalid data (conversion error): {line}");
                }
                return (name, property, buffer);
            };
            breast_3ba_object CBPC3BA = new();
            //lines.ForEach((line) =>
            foreach (string line in lines)
            {
                if (line.StartsWith('#') || string.IsNullOrWhiteSpace(line)) { continue; }
                (string name, string property, double[] data) = parse(line);
                CBPC3BA.Find(name)?.Data.SetObject(property, new physics_object(name, property, data));
            };

            //vm.CBPC_Physics_running = false;
            return CBPC3BA;
        }


        private (string[], string[]) GetFilter()
        {
            if (PhysicsAll == null || PhysicsAll.Length == 0) Read();
            if (PhysicsAll == null || PhysicsAll.Length == 0) return (Array.Empty<string>(), Array.Empty<string>());

            List<string> bonefilter = new();
            List<string> keyfilter = new List<string>();
            foreach (string line in PhysicsAll)
            {
                string buffer = line;
                if (string.IsNullOrWhiteSpace(buffer) || buffer.StartsWith('#')) continue;
                if (buffer.Contains('#')) buffer = buffer.Substring(0, buffer.IndexOf('#'));
                string[] b1 = buffer.Split(' ');
                if (b1 == null || b1.Length < 1 || !b1[0].Contains('.')) continue;
                string[] b2 = b1[0].Split('.');
                if (b2 == null || b2.Length < 2) continue;

                if (!bonefilter.Contains(b2[0])) bonefilter.Add(b2[0]);
                if (!keyfilter.Contains(b2[1])) keyfilter.Add(b2[1]);
            }
            return (bonefilter.ToArray(), keyfilter.ToArray());
        }
        public string Make3BA(breast_3ba_object o)
        {
            if (o == null) return string.Empty;
            if (PhysicsAll == null || PhysicsAll.Length == 0) Read();
            if (PhysicsAll == null || PhysicsAll.Length == 0) return string.Empty;
            string[] data = PhysicsAll;
            
            for (int i = 0; i < data.Length; i++)
            {
                string buffer = data[i];
                if (string.IsNullOrWhiteSpace(buffer) || buffer.StartsWith('#')) continue;
                if (buffer.Contains('#')) buffer = buffer.Substring(0, buffer.IndexOf('#'));
                string[] b1 = buffer.Split(' ');
                if (b1 == null || (b1.Length != 2 && b1.Length != 3) || !b1[0].Contains('.')) continue;
                string[] b2 = b1[0].Split('.');
                if (b2 == null || b2.Length != 2) continue;
                string name = b2[0], key = b2[1];
                double[]? values = o.Find(name)?.Data.GetPhysics(key);
                if (values == null) continue;
                string newline = $"{name}.{key} {values[0]}";
                if (values.Length > 1) newline += $" {values[1]}";
                data[i] = newline;
            }

            return string.Join(Environment.NewLine, data);
        }

    }
}
