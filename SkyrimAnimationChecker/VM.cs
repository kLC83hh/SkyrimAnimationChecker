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
            string workdir = AppDomain.CurrentDomain.BaseDirectory;
            if (useDesktop || string.IsNullOrWhiteSpace(dirMods)) workdir = desktop;
            dirMods = @"C:\Games\FaceRim_SE-TA\SkyrimSE\mods";
            fileNIF_bodyslide1 = System.IO.Path.Combine(workdir, "femalebody_1.nif");
            fileNIF_bodyslide0 = System.IO.Path.Combine(workdir, "femalebody_0.nif");
            useCustomExample = false;
            fileNIF_sphere1 = System.IO.Path.Combine(workdir, "example_0.nif");
            fileNIF_sphere0 = System.IO.Path.Combine(workdir, "example_1.nif");
            fileNIF_out1 = System.IO.Path.Combine(workdir, "intermedium_1.nif");
            fileNIF_out0 = System.IO.Path.Combine(workdir, "intermedium_0.nif");
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

        public string fileNIF_bodyslide1 { get => Get<string>(); set => Set(value); }
        public string fileNIF_bodyslide0 { get => Get<string>(); set => Set(value); }

        public bool useCustomExample { get => Get<bool>(); set => Set(value); }
        public string fileNIF_sphere1 { get => Get<string>(); set => Set(value); }
        public string fileNIF_sphere0 { get => Get<string>(); set => Set(value); }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool overwriteInterNIFs { get => Get<bool>(); set => Set(value); }

        public string fileNIF_out1 { get => Get<string>(); set => Set(value); }
        public string fileNIF_out0 { get => Get<string>(); set => Set(value); }
        public string fileCBPC { get => Get<string>(); set => Set(value); }
        public string groupfilter { get => Get<string>(); set => Set(value); }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool writeAll { get => Get<bool>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool writeSome { get => Get<bool>(); set => Set(value); }


        public System.Collections.ObjectModel.ObservableCollection<string> CBPC_Checker { get => Get<System.Collections.ObjectModel.ObservableCollection<string>>(); set => Set(value); }
        public bool CBPCfullcopy { get => Get<bool>(); set => Set(value); }
        public bool CBPCbackup { get => Get<bool>(); set => Set(value); }



        private static string vmFilePath => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SkyrimAnimationCheckerConfig.json");
        public void Save()
        {
            using (System.IO.StreamWriter sw = new(vmFilePath, false))
            {
                System.Text.Json.JsonSerializer.Serialize(sw.BaseStream, this, typeof(VM), new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });
                sw.Flush();
            }
        }
        public bool LoadFailed { get; set; } = false;
        public static VM Load()
        {
            if (System.IO.File.Exists(vmFilePath))
            {
                using (System.IO.StreamReader sr = new(vmFilePath))
                {
                    return System.Text.Json.JsonSerializer.Deserialize<VM>(sr.BaseStream) ?? new VM() { LoadFailed = true };
                }
            }
            return new VM() { LoadFailed = true };
        }
    }
}
