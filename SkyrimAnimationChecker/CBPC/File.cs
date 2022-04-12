﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.CBPC
{
    public class File
    {
        protected Common.VM_GENERAL vm;
        public File(Common.VM_GENERAL linker) => vm = linker;
        public File(Common.VM linker) => vm = linker.GENERAL;

        private string AutoWriteComment = "### Automatically writed by SAC";

        protected void Backup(string path) => System.IO.File.Copy(path, System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), path.Split('\\').Last()), true);
        public string[] ReadLines(string path, bool backup = false)
        {
            string[] outputArray = Array.Empty<string>();
            if (!System.IO.File.Exists(path)) return outputArray;
            if (backup) Backup(path);
            using (System.IO.StreamReader sr = new(path))
            {
                //while (!sr.EndOfStream) { string? buffer = sr.ReadLine(); M.D(buffer); lines.Add(buffer?.Trim() ?? string.Empty); }
                string buffer = sr.ReadToEnd();
                outputArray = buffer.Split(Environment.NewLine);
            }
            List<string> list = outputArray.ToList();
            list.RemoveAll(x => x.StartsWith(AutoWriteComment));
            outputArray = list.ToArray();
            //M.D(lines.Count);
            return outputArray;
        }
        private bool SaveRefused(string path, bool overwrite = true) => System.IO.File.Exists(path) && !overwrite;
        protected bool CanSave(string path, bool overwrite = true) => !SaveRefused(path, overwrite);
        public void Save(string data, string path, bool overwrite = true, bool backup = true)
        {
            if (SaveRefused(path, overwrite)) return;
            if (backup) Backup(path);
            data += $"{Environment.NewLine}{AutoWriteComment} at {DateTime.Now}";
            using (System.IO.StreamWriter sw = new(path))
            {
                sw.Write(data);
            }
        }

    }
}
