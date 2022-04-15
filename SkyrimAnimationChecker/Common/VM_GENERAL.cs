using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker.Common
{
    public partial class VM_GENERAL : Notify.NotifyPropertyChanged
    {
        public VM_GENERAL() => Reset();
        public void Reset()
        {
            DefaultCommon();
            DefaultVisual3BA();
        }
        partial void DefaultCommon();
        partial void DefaultVisual3BA();

    }
    public partial class VM_GENERAL : Notify.NotifyPropertyChanged
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public bool DARrunning { get => Get<bool>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool NIFrunning { get => Get<bool>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool CBPCrunning { get => Get<bool>(); set => Set(value); }

        [System.Text.Json.Serialization.JsonIgnore]
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

            dirNIF_bodyslide = workdir;
            fileNIF_bodyslide = "femalebody_0.nif, femalebody_1.nif";
            useCustomExample = false;
            weightNumber = 1;
            fileNIF_sphere1 = System.IO.Path.Combine(workdir, "example_0.nif");
            fileNIF_sphere0 = System.IO.Path.Combine(workdir, "example_1.nif");

            readFromNIFs = false;
            fileNIF_out1 = System.IO.Path.Combine(workdir, "inter_1.nif");
            fileNIF_out0 = System.IO.Path.Combine(workdir, "inter_0.nif");
            fileCBPC_Collision = System.IO.Path.Combine(workdir, "CBPCollisionConfig_Female.txt");
            groupfilter = "[NPC L Pussy02],[NPC L RearThigh],[NPC L Thigh [LThg]],[NPC L UpperArm [LUar]],[NPC L Forearm [LLar]]";

            CBPC_Checker = new() { "# Collision spheres", "# Affected Nodes", "# Collider Nodes" };
            CBPCfullcopy = true;
            CBPCbackup = true;
            CBPCremoveUnnecessary = true;

            panelNumber = 1;
            locationCBPC_Physics = workdir;
            fileCBPC_Physics = "CBPConfig_3b.txt";
            //fileCBPC_Physics = System.IO.Path.Combine(workdir, "CBPConfig_3b.txt");
            //CBPC_Phy_Mirror_LR = true;
            //CBPC_Phy_Mirror_MinMax = true;
            //CBPC_Phy_3BA_All = true;
            //CBPC_Phy_3BA_Select = 1;
        }

        public bool useDesktop { get => Get<bool>(); set => Set(value); }

        public string dirMods { get => Get<string>(); set => Set(value); }

        public string dirNIF_bodyslide { get => Get<string>(); set => Set(value); }
        public string fileNIF_bodyslide { get => Get<string>(); set => Set(value); }

        public bool useCustomExample { get => Get<bool>(); set => Set(value); }
        public int weightNumber { get => Get<int>(); set { if (value > -1) Set(value); } }
        public string fileNIF_sphere1 { get => Get<string>(); set => Set(value); }
        public string fileNIF_sphere0 { get => Get<string>(); set => Set(value); }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool overwriteInterNIFs { get => Get<bool>(); set => Set(value); }

        public bool readFromNIFs { get => Get<bool>(); set => Set(value); }
        public string fileNIF_out1 { get => Get<string>(); set => Set(value); }
        public string fileNIF_out0 { get => Get<string>(); set => Set(value); }
        public string fileCBPC_Collision { get => Get<string>(); set => Set(value); }
        public string fileCBPC { get => fileCBPC_Collision; set => fileCBPC_Collision = value; }
        public string groupfilter { get => Get<string>(); set => Set(value); }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool writeAll { get => Get<bool>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool writeSome { get => Get<bool>(); set => Set(value); }


        public System.Collections.ObjectModel.ObservableCollection<string> CBPC_Checker { get => Get<System.Collections.ObjectModel.ObservableCollection<string>>(); set => Set(value); }
        public bool CBPCfullcopy { get => Get<bool>(); set => Set(value); }
        public bool CBPCbackup { get => Get<bool>(); set => Set(value); }
        public bool CBPCremoveUnnecessary { get => Get<bool>(); set => Set(value); }

        //[System.Text.Json.Serialization.JsonIgnore]
        //public bool CBPCVisualLeftonly { get => Get<bool>(); set => Set(value); }



        public int panelNumber { get => Get<int>(); set => Set(value); }
        public string locationCBPC_Physics { get => Get<string>(); set => Set(value); }
        public string fileCBPC_Physics { get => Get<string>(); set => Set(value); }
        //public bool CBPC_Phy_Mirror_LR { get => Get<bool>(); set => Set(value); }
        //public bool CBPC_Phy_Mirror_MinMax { get => Get<bool>(); set => Set(value); }
        //public bool CBPC_Phy_3BA_All { get => Get<bool>(); set => Set(value); }
        //public int CBPC_Phy_3BA_Select { get => Get<int>(); set => Set(value); }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool overwriteCBPC_Physics { get => Get<bool>(); set => Set(value); }


    }
}
