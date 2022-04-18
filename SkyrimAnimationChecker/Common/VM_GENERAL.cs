using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.Common
{
    public partial class VM_GENERAL : Notify.NotifyPropertyChanged
    {
        public VM_GENERAL() => Reset();
        public void Reset()
        {
            DefaultCommon();
            //DefaultVisual3BA();
        }
        partial void DefaultCommon();
        //partial void DefaultVisual3BA();

    }
    public partial class VM_GENERAL : Notify.NotifyPropertyChanged
    {
        [JsonIgnore]
        public bool DARrunning { get => Get<bool>(); set => Set(value); }
        [JsonIgnore]
        public bool NIFrunning { get => Get<bool>(); set => Set(value); }
        [JsonIgnore]
        public bool CBPCrunning { get => Get<bool>(); set => Set(value); }

        [JsonIgnore]
        public bool CBPC_Physics_running { get => Get<bool>(); set => Set(value); }
    }
    public partial class VM_GENERAL : Notify.NotifyPropertyChanged
    {
        partial void DefaultCommon()
        {
            useDesktop = true;
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string workdir = AppDomain.CurrentDomain.BaseDirectory;
            if (useDesktop || string.IsNullOrWhiteSpace(dirMods)) workdir = desktop;

            dirMods = @"C:\Games\FaceRim_SE-TA\SkyrimSE\mods";

            panelNumber = 1;

            // NIF
            dirNIF_bodyslide = workdir;
            fileNIF_bodyslide = "femalebody_0.nif, femalebody_1.nif";
            useCustomExample = false;
            weightNumber = 1;
            fileNIF_sphere1 = System.IO.Path.Combine(workdir, "example_0.nif");
            fileNIF_sphere0 = System.IO.Path.Combine(workdir, "example_1.nif");

            readFromNIFs = false;
            fileNIF_out1 = System.IO.Path.Combine(workdir, "inter_1.nif");
            fileNIF_out0 = System.IO.Path.Combine(workdir, "inter_0.nif");

            // CBPC
            dirCBPC = workdir;
            fileCBPC_Collision = "CBPCollisionConfig_Female.txt";
            fileCBPC_Collisions = new() { "CBPCollisionConfig.txt", "CBPCollisionConfig_Female.txt" };
            groupfilter = "[NPC L Pussy02],[NPC L RearThigh],[NPC L Thigh [LThg]],[NPC L UpperArm [LUar]],[NPC L Forearm [LLar]]";

            fileCBPC_Physics = "CBPConfig_3b.txt";
            fileCBPC_Physicss = new();
            cbpc15xbeta2 = false;

            CBPC_Checker = new() { "# Collision spheres", "# Affected Nodes", "# Collider Nodes" };
            CBPCfullcopy = true;
            CBPCbackup = true;
            CBPCremoveUnnecessary = true;

        }

        public bool useDesktop { get => Get<bool>(); set => Set(value); }

        // DAR
        public string dirMods { get => Get<string>(); set => Set(value); }

        // page select
        public int panelNumber { get => Get<int>(); set => Set(value); }

        // NIF
        public string dirNIF_bodyslide { get => Get<string>(); set => Set(value.Replace("\\\\", "\\")); }
        public string fileNIF_bodyslide { get => Get<string>(); set => Set(value); }

        public bool useCustomExample { get => Get<bool>(); set => Set(value); }
        public int weightNumber { get => Get<int>(); set { if (value > -1) Set(value); } }
        public string fileNIF_sphere1 { get => Get<string>(); set => Set(value); }
        public string fileNIF_sphere0 { get => Get<string>(); set => Set(value); }

        [JsonIgnore]
        public bool overwriteInterNIFs { get => Get<bool>(); set => Set(value); }

        public bool readFromNIFs { get => Get<bool>(); set => Set(value); }
        public string fileNIF_out1 { get => Get<string>(); set => Set(value); }
        public string fileNIF_out0 { get => Get<string>(); set => Set(value); }

        // CBPC
        public string dirCBPC { get => Get<string>(); set => Set(value.Replace("\\\\", "\\")); }

        public string fileCBPC_Collision { get => Get<string>(); set => Set(value); }
        public ObservableCollection<string> fileCBPC_Collisions { get => Get<ObservableCollection<string>>(); set => Set(value); }
        public string groupfilter { get => Get<string>(); set => Set(value); }

        public string fileCBPC_Physics { get => Get<string>(); set => Set(value); }
        [JsonIgnore]
        public ObservableCollection<string> fileCBPC_Physicss { get => Get<ObservableCollection<string>>(); set => Set(value); }

        [JsonIgnore]
        public bool cbpc15xbeta2 { get => Get<bool>(); set => Set(value); }

        [JsonIgnore]
        public bool writeAll { get => Get<bool>(); set => Set(value); }
        [JsonIgnore]
        public bool writeSome { get => Get<bool>(); set => Set(value); }
        [JsonIgnore]
        public bool overwriteCBPC_Physics { get => Get<bool>(); set => Set(value); }


        public ObservableCollection<string> CBPC_Checker { get => Get<ObservableCollection<string>>(); set => Set(value); }
        public bool CBPCfullcopy { get => Get<bool>(); set => Set(value); }
        public bool CBPCbackup { get => Get<bool>(); set => Set(value); }
        public bool CBPCremoveUnnecessary { get => Get<bool>(); set => Set(value); }


        //[JsonIgnore]
        //public bool CBPCVisualLeftonly { get => Get<bool>(); set => Set(value); }



    }
}
