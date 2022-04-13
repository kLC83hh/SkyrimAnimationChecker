using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.CBPC
{
    public class Physics : File
    {
        public Physics(VM_GENERAL linker) : base(linker) { }
        public Physics(VM linker) : base(linker) { }


        private string[] PhysicsAll = Array.Empty<string>();
        private string path => System.IO.Path.Combine(vm.locationCBPC_Physics, vm.fileCBPC_Physics);
        public string[] Read(bool backup = false, bool force = false)
        {
            if (PhysicsAll == null || PhysicsAll.Length == 0 || force)
                PhysicsAll = ReadLines(path, backup);
            return PhysicsAll;
        }
        public int Save(object o, bool overwrite = false)
        {
            string? data = null;

            if (o is Icbpc_breast_data c) data = MakeBreast(c);

            if (string.IsNullOrEmpty(data)) return 2;

            if (CanWrite(path, overwrite)) Write(data, path, overwrite, vm.CBPCbackup);
            else return 4;
            return 0;
        }


        public Icbpc_data GetPhysics() => Parse_Type();
        private Icbpc_data Parse_Type()
        {
            if (PhysicsAll.Length == 0) Read();
            if (PhysicsAll.Length == 0) throw EE.New(2001);
            string[] lines = PhysicsAll;

            // valid names
            string[] bbp = new string[] { "LBreast", "RBreast" };
            string[] _3ba = new string[] { "ExtraBreast1L", "ExtraBreast2L", "ExtraBreast3L", "ExtraBreast1R", "ExtraBreast2R", "ExtraBreast3R" };
            // type check
            string? type = null;
            foreach (string line in lines)
            {
                if (line.StartsWith('#') || string.IsNullOrWhiteSpace(line)) { continue; }
                (string name, string property, double[] data) = Parseline(line);
                if (_3ba.Contains(name)) { type = "3ba"; break; }
                else if (bbp.Contains(name)) { type = "bbp"; break; }
            }

            // try get data
            Icbpc_data? o = Parse_Breast(lines, type);

            if (o != null) return o;
            else throw EE.New(2008);
        }
        private Icbpc_breast_data? Parse_Breast(string[] lines, string? type)
        {
            Func<string?, Icbpc_breast_data?> newObject = (type) =>
            {
                if (type == "3ba") return new cbpc_breast_3ba();
                else if (type == "bbp") return new cbpc_breast_bbp();
                else return null;
            };

            Icbpc_breast_data? CBPCBREAST = newObject(type);
            if (CBPCBREAST != null)
            {
                foreach (string line in lines)
                {
                    if (line.StartsWith('#') || string.IsNullOrWhiteSpace(line)) { continue; }
                    (string name, string property, double[] data) = Parseline(line);
                    CBPCBREAST.Find(name)?.SetObject(property, new physics_object(name, property, data));
                };
            }
            return CBPCBREAST;
        }
        private (string, string, double[]) Parseline(string line)
        {
            string[] part = line.Trim().Split(' ').ForEach(x => x.Trim());
            if (part.Length != 3 && part.Length != 2) throw EE.New(2201, $"Invalid data: {line}");
            string[] vs = part[0].Split('.').ForEach(x => x.Trim());
            if (vs.Length != 2) throw EE.New(2202, $"Invalid data: {line}");

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
                throw EE.New(2203, $"Invalid data (conversion error): {line}");
            }
            return (name, property, buffer);
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
        public string MakeBreast(Icbpc_breast_data o)
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
                double[]? values = o.Find(name)?.GetPhysics(key);
                if (values == null) continue;
                string newline = $"{name}.{key} {values[0]}";
                if (values.Length > 1) newline += $" {values[1]}";
                data[i] = newline;
            }

            return string.Join(Environment.NewLine, data);
        }

    }
}
