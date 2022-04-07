using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker
{
    internal class VM : Notify.NotifyPropertyChanged
    {
        public VM() => DefaultValue();
        private void DefaultValue()
        {
            useDesktop = true;
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            dirMods = @"C:\Games\FaceRim_SE-TA\SkyrimSE\mods";
            string workdir = AppDomain.CurrentDomain.BaseDirectory;
            if (useDesktop || string.IsNullOrWhiteSpace(dirMods)) workdir = desktop;
            fileNIFO1 = System.IO.Path.Combine(workdir, "femalebody_1.nif");
            fileNIFO0 = System.IO.Path.Combine(workdir, "femalebody_0.nif");
            useCustomExample = false;
            fileNIFE1 = System.IO.Path.Combine(workdir, "example_0.nif");
            fileNIFE0 = System.IO.Path.Combine(workdir, "example_1.nif");
            fileNIF1 = System.IO.Path.Combine(workdir, "intermedium_1.nif");
            fileNIF0 = System.IO.Path.Combine(workdir, "intermedium_0.nif");
            fileCBPC = System.IO.Path.Combine(workdir, "CBPCollisionConfig_Female.txt");
            groupfilter = "[NPC L Pussy02],[NPC L RearThigh],[NPC L Thigh [LThg]],[NPC L UpperArm [LUar]],[NPC L Forearm [LLar]]";
            CBPC_Checker = new System.Collections.ObjectModel.ObservableCollection<string> { "# Collision spheres", "# Affected Nodes", "# Collider Nodes" };
            CBPCfullcopy = true;
            CBPCbackup = true;
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool DARrunning { get => Get<bool>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool NIFrunning { get => Get<bool>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool CBPCrunning { get => Get<bool>(); set => Set(value); }

        public bool useDesktop { get => Get<bool>(); set => Set(value); }

        public string dirMods { get => Get<string>(); set => Set(value); }

        public string fileNIFO1 { get => Get<string>(); set => Set(value); }
        public string fileNIFO0 { get => Get<string>(); set => Set(value); }

        public bool useCustomExample { get => Get<bool>(); set => Set(value); }
        public string fileNIFE1 { get => Get<string>(); set => Set(value); }
        public string fileNIFE0 { get => Get<string>(); set => Set(value); }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool overwriteInterNIFs { get => Get<bool>(); set => Set(value); }

        public string fileNIF1 { get => Get<string>(); set => Set(value); }
        public string fileNIF0 { get => Get<string>(); set => Set(value); }
        public string fileCBPC { get => Get<string>(); set => Set(value); }
        public string groupfilter { get => Get<string>(); set => Set(value); }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool writeAll { get => Get<bool>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool writeSome { get => Get<bool>(); set => Set(value); }


        public System.Collections.ObjectModel.ObservableCollection<string> CBPC_Checker { get => Get<System.Collections.ObjectModel.ObservableCollection<string>>(); set => Set(value); }
        public bool CBPCfullcopy { get => Get<bool>(); set => Set(value); }
        public bool CBPCbackup { get => Get<bool>(); set => Set(value); }



        public void Save()
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SkyrimAnimationCheckerConfig.json");
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path, false))
            {
                System.Text.Json.JsonSerializer.Serialize(sw.BaseStream, this, typeof(VM), new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });
                sw.Flush();
            }
        }
        public VM Load()
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SkyrimAnimationCheckerConfig.json");
            if (System.IO.File.Exists(path))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
                {
                    return System.Text.Json.JsonSerializer.Deserialize<VM>(sr.BaseStream) ?? new VM();
                }
            }
            return new VM();
        }
    }
}
