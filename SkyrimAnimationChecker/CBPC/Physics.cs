using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyrimAnimationChecker.Common;

namespace SkyrimAnimationChecker.CBPC
{
    public class Physics : CBPC
    {
        public Physics(VM_GENERAL linker) : base(linker) { }
        public Physics(VM linker) : base(linker) { }


        private string[] PhysicsAll = Array.Empty<string>();
        private string path => System.IO.Path.Combine(vm.dirCBPC, vm.fileCBPC_Physics);
        public string[] Read(bool backup = false, bool force = false)
        {
            if (PhysicsAll == null || PhysicsAll.Length == 0 || force)
                PhysicsAll = ReadLines(path, backup);
            return PhysicsAll;
        }
        public int Save(object o, bool overwrite = false)
        {
            string? data = null;

            if (o is Icbpc_data c) data = MakeCBPConfig(c);

            if (string.IsNullOrEmpty(data)) return 2;

            if (CanWrite(path, overwrite)) Write(data, path, overwrite, vm.CBPCbackup);
            else return 4;
            return 0;
        }

        public void ReplaceFile(string path) => Replace(string.Join(Environment.NewLine, ReadLines(path)));
        public void Replace(string data)
        {
            PhysicsAll = data.Split(Environment.NewLine);
            try
            {
                _ = GetPhysics();
                if (CanWrite(this.path, true)) Write(data, this.path, true);
                //Parse_Type(true);
            }
            catch { }
        }


        public Icbpc_data GetPhysics() => Parse_Type();
        private Icbpc_data Parse_Type(bool forceread = false)
        {
            if (PhysicsAll.Length == 0 || forceread) Read();
            if (PhysicsAll.Length == 0) throw EE.New(2001);
            string[] lines = PhysicsAll;

            // type check
            string? type = null;
            foreach (string line in lines)
            {
                if (line.StartsWith('#') || string.IsNullOrWhiteSpace(line)) { continue; }
                (string name, string property, double[] data) = Parseline(line);
                if (CBPC_Physics_Keywords.IsFound(name, out type)) break;
            }

            // try get data
            Icbpc_data? o = Parse(lines, type);

            if (o != null) return o;
            else throw EE.New(2008);
        }
        private Icbpc_data? Parse(string[] lines, string? type)
        {
            Func<string?, Icbpc_data?> newObject = (type) =>
            {
                if (type == "3ba") return new cbpc_breast_3ba();
                else if (type == "bbp") return new cbpc_data_mirrored();
                else if (type == "butt") return new cbpc_data_mirrored();
                else if (type == "belly") return new cbpc_data();
                else if (type == "leg") return new cbpc_leg();
                else if (type == "vagina") return new cbpc_vagina();
                else return null;
            };

            Icbpc_data? CBPCDATA = newObject(type);
            if (CBPCDATA != null)
            {
                bool named = false;
                foreach (string line in lines)
                {
                    if (line.StartsWith('#') || string.IsNullOrWhiteSpace(line)) { continue; }
                    (string name, string property, double[] data) = Parseline(line);
                    if (!named) CBPCDATA.Name = name;//M.D(property);
                    if (vm.cbpc15xbeta2) Beta_auto_key_update_cbpc15xbeta2(ref property);
                    CBPCDATA.Find(name)?.SetObject(property, new physics_object(name, property, data));
                };
            }
            return CBPCDATA;
        }
        private (string, string, double[]) Parseline(string line)
        {
            string[] part = line.Trim().Split(' ').ForEach(x => x.Trim());
            if (part.Length != 3 && part.Length != 2) throw EE.New(2201, $"Invalid data: {line}");
            string[] vs = part[0].Split('.').ForEach(x => x.Trim());
            if (vs.Length != 2) throw EE.New(2202, $"Invalid data: {line}");

            string name = vs[0];
            string key = vs[1];

            string[] dBuffer = new string[2] { part[1], part[1] };
            if (part.Length == 3) dBuffer[0] = part[2];

            double[] buffer = new double[dBuffer.Length];
            try
            {
                for (int i = 0; i < dBuffer.Length; i++) buffer[i] = Convert.ToDouble(dBuffer[i]);
            }
            catch (Exception)
            {
                throw EE.New(2203, $"Invalid data (conversion error): {line}");
            }
            return (name, key, buffer);
        }
        private void Beta_auto_key_update_cbpc15xbeta2(ref string key)
        {
            Dictionary<string, string> changes = new();
            changes.Add("linearXspreadforceY", "linearXspreadforceYRot");
            changes.Add("linearXspreadforceZ", "linearXspreadforceZRot");
            changes.Add("linearYspreadforceX", "linearYspreadforceXRot");
            changes.Add("linearYspreadforceZ", "linearYspreadforceZRot");
            changes.Add("linearZspreadforceX", "linearZspreadforceXRot");
            changes.Add("linearZspreadforceY", "linearZspreadforceYRot");
            //if (changes.Keys.Contains(key)) key = changes[key];
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
        public string MakeCBPConfig(Icbpc_data o)
        {
            if (o == null) return string.Empty;
            if (PhysicsAll == null || PhysicsAll.Length == 0) Read();
            if (PhysicsAll == null || PhysicsAll.Length == 0) return string.Empty;
            string[] data = PhysicsAll;
            
            for (int i = 0; i < data.Length; i++)
            {
                string buffer = data[i].TrimEnd(), comment = string.Empty;
                if (string.IsNullOrWhiteSpace(buffer) || buffer.StartsWith('#')) continue;

                if (buffer.Contains('#'))
                {
                    comment = buffer.Substring(buffer.IndexOf('#'));
                    buffer = buffer.Substring(0, buffer.IndexOf('#'));
                }
                string[] b1 = buffer.Split(' ');
                if (b1 == null || (b1.Length != 2 && b1.Length != 3) || !b1[0].Contains('.')) continue;

                string[] b2 = b1[0].Split('.').ForEach(x => x.Trim());
                if (b2 == null || b2.Length != 2) continue;

                string name = b2[0], key = b2[1];
                if (vm.cbpc15xbeta2) Beta_auto_key_update_cbpc15xbeta2(ref key);
                double[]? values = o.Find(name)?.GetPhysics(key);
                if (values == null) { M.D($"{name} {key}"); continue; }
                
                string newline = $"{b1[0]} {values[1]}";
                if (values.Length > 1 && values[1] != values[0] || !vm.CBPCremoveUnnecessary)
                    newline += $" {values[0]}";
                if (string.IsNullOrEmpty(comment)) newline += comment;
                data[i] = newline;
            }

            return string.Join(Environment.NewLine, data);
        }

    }

    public static class CBPC_Physics_Keywords
    {
        public enum Keywords { _3ba, bbp, belly, butt, leg, vagina }
        public static string GetKeyword(Keywords k)
        {
            switch (k)
            {
                case Keywords._3ba: return "3ba";
                case Keywords.bbp: return "bbp";
                case Keywords.belly: return "belly";
                case Keywords.butt: return "butt";
                case Keywords.leg: return "leg";
                case Keywords.vagina: return "vagina";
            }
            throw EE.New(101);
        }
        public static string[] ValidNames(Keywords k)
        {
            switch (k)
            {
                case Keywords._3ba:
                    return new string[] { "ExtraBreast1L", "ExtraBreast2L", "ExtraBreast3L", "ExtraBreast1R", "ExtraBreast2R", "ExtraBreast3R" };
                case Keywords.bbp:
                    return new string[] { "LBreast", "RBreast" };
                case Keywords.belly:
                    return new string[] { "Belly", "NPCBelly" };
                case Keywords.butt:
                    return new string[] { "LButt", "RButt" };
                case Keywords.leg:
                    return new string[] { "LFrontThigh", "LRearThigh", "LRearCalf", "RFrontThigh", "RRearThigh", "RRearCalf" };
                case Keywords.vagina:
                    return new string[] { "Vagina", "Labia", "VaginaB", "Clit", "LLabia", "RLabia" };
            }
            return Array.Empty<string>();
        }
        public static bool IsFound(string name, out string? type)
        {
            foreach (Keywords n in Enum.GetValues(typeof(Keywords)))
            {
                if (ValidNames(n).Contains(name)) { type = GetKeyword(n); return true; }
            }
            type = null;
            return false;
        }
    }
}
