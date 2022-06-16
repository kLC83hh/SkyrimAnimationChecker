using System;
using System.Collections.Generic;
using System.Linq;

namespace SkyrimAnimationChecker.DAR
{
    internal class DAR
    {
        protected Common.VM_GENERAL vm;
        public DAR(Common.VM_GENERAL linker) => vm = linker;
        public DAR(Common.VM linker) => vm = linker.GENERAL;
        public Dictionary<int, List<string>> Run()
        {
            vm.DARrunning = true;
            string[] list = GetDARfolders();
            DAR_MOD[] sublist = GetAllDARsubfolders(list);
            //bool res = CheckDuplicate(sublist, out string[] dups);
            Dictionary<int, List<string>> res = CheckDuplicate(sublist);
            vm.darWorkProgress = string.Empty;
            vm.DARrunning = false;
            return res;
        }
        //private string basedir = vm.dirMods;
        //private string subdir = "meshes\\actors\\character\\animations\\DynamicAnimationReplacer\\_CustomConditions";

        private string[] GetDARfolders()
        {
            vm.darWorkProgress = $"1/1 1/4";
            string[] dirs = System.IO.Directory.GetDirectories(vm.dirMods, $"*DynamicAnimationReplacer", System.IO.SearchOption.AllDirectories);
            return dirs;
        }
        private static string[] GetDARsubfolders(string dir)
        {
            var a = new System.IO.DirectoryInfo(dir);
            //T.Text = $"{a.EnumerateFiles().Count()}";
            if (a.EnumerateFiles().Any())
            {
                System.IO.FileInfo[] b = a.EnumerateFiles().ToArray();
                List<string> r = new();
                foreach (var file in b)
                {
                    r.Add(file.FullName);
                }
                return r.ToArray();
            }
            else if (a.EnumerateDirectories().Any())
            {
                //Tlist(a.EnumerateDirectories().ToArray());
                var b = new System.IO.DirectoryInfo(a.EnumerateDirectories().First().FullName);
                //T.Text = $"{b.EnumerateDirectories().Count()}";
                if (b.EnumerateDirectories().Any())
                {
                    List<string> r = new();
                    foreach (var d in b.EnumerateDirectories())
                    {
                        r.Add(d.FullName);
                    }
                    return r.ToArray();
                }
            }
            return Array.Empty<string>();
        }
        private DAR_MOD[] GetAllDARsubfolders(string[] list)
        {
            List<DAR_MOD> r = new();
            int i = 0;
            foreach (var d in list)
            {
                //string[] b = GetDARsubfolders(d);
                //r.AddRange(b);
                vm.darWorkProgress = $"{i++}/{list.Length} 2/4";
                r.Add(new DAR_MOD(d, GetDARsubfolders(d)));
            }
            return r.ToArray();
        }

        private Dictionary<int, List<string>> CheckDuplicate(DAR_MOD[] list)
        {
            List<int> all = new();
            Dictionary<int, List<string>> dups = new();
            int i = 0;
            list.ForEach(x =>
            {
                vm.darWorkProgress = $"{i++}/{list.Length} 3/4";
                x.Numbers.ForEach(n =>
                {
                    if (all.Contains(n) && !dups.ContainsKey(n)) dups.Add(n, new List<string>());
                    all.Add(n);
                });
            });

            i = 0;
            dups.ForEach(x =>
            {
                vm.darWorkProgress = $"{i++}/{list.Length} 4/4";
                list.ForEach(y => { if (y.Numbers.Contains(x.Key)) { x.Value.Add(y.Name); } });
            });
            //list.ForEach(x => { if (numbers.Count(y => y == x) > 1 && !dups.Contains(x)) { dups.Add(x); } });
            //duplicates = dups.ToArray();

            //return numbers.GroupBy(n => n).Any(c => c.Count() > 1);
            //return duplicates.Length > 0;
            return dups;
        }

    }

    public class DAR_MOD
    {
        public DAR_MOD(string name, string[] sub)
        {
            Name = name;
            Numbers = new int[sub.Length];
            for (int i = 0; i < sub.Length; i++)
            {
                try { Numbers[i] = Convert.ToInt32(sub[i].Split("\\").Last()); }
                catch { throw EE.New(31001); }
            }
        }
        public string Name { get; set; }

        public int[] Numbers { get; set; }
    }
}
