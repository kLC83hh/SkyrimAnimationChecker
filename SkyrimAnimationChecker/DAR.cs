using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker
{
    internal class DAR
    {
        VM vm;
        public DAR(VM linker) => vm = linker;
        public string Run()
        {
            vm.DARrunning = true;
            string[] list = GetDARfolders();
            string[] sublist = GetAllDARsubfolders(list);
            //Tlist(sublist);
            string res = CheckDuplicate(sublist);
            vm.DARrunning = false;
            return res;
        }
        //private string basedir = vm.dirMods;
        //private string subdir = "meshes\\actors\\character\\animations\\DynamicAnimationReplacer\\_CustomConditions";

        private string[] GetDARfolders()
        {
            string[] dirs = System.IO.Directory.GetDirectories(vm.dirMods, $"*DynamicAnimationReplacer", System.IO.SearchOption.AllDirectories);
            //T.Text = dirs.Length.ToString();
            //if (dirs.Length > 0)
            //{
            //    foreach(string dir in dirs)
            //    {
            //        T.Text += $"{dir}\n";
            //    }
            //}
            return dirs;
        }
        private string[] GetDARsubfolders(string dir)
        {
            var a = new System.IO.DirectoryInfo(dir);
            //T.Text = $"{a.EnumerateFiles().Count()}";
            if (a.EnumerateFiles().Count() > 0) {
                System.IO.FileInfo[] b = a.EnumerateFiles().ToArray();
                List<string> r = new List<string>();
                foreach (var file in b)
                {
                    r.Add(file.FullName);
                }
                return r.ToArray();
            }
            else if (a.EnumerateDirectories().Count() > 0)
            {
                //Tlist(a.EnumerateDirectories().ToArray());
                var b = new System.IO.DirectoryInfo(a.EnumerateDirectories().First().FullName);
                //T.Text = $"{b.EnumerateDirectories().Count()}";
                if (b.EnumerateDirectories().Count() > 0)
                {
                    List<string> r = new List<string>();
                    foreach (var d in b.EnumerateDirectories())
                    {
                        r.Add(d.FullName);
                    }
                    return r.ToArray();
                }
            }
            return new string[0];
        }
        private string[] GetAllDARsubfolders(string[] list)
        {
            List<string> r = new List<string>();
            foreach (var d in list)
            {
                string[] b = GetDARsubfolders(d);
                //T.Text = $"{b.Length}";
                r.AddRange(b);
            }
            return r.ToArray();
        }

        private string CheckDuplicate(string[] sublist)
        {
            string[] numbers = new string[sublist.Length];
            for (int i = 0; i < sublist.Length; i++) numbers[i] = sublist[i].Split("\\").Last();
            //Tlist(numbers);
            return $"DAR Number Duplicate => {sublist.GroupBy(n => n).Any(c => c.Count() > 1)}";
        }

    }
}
