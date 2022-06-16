using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace SkyrimAnimationChecker.Common
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
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
        // No-Brainer
        [JsonIgnore]
        public bool DoItRunning { get => Get<bool>(); set => Set(value); }

        [JsonIgnore]
        public bool DARrunning { get => Get<bool>(); set => Set(value); }
        [JsonIgnore]
        public bool NIFrunning { get => Get<bool>(); set => Set(value); }
        [JsonIgnore]
        public bool CBPCrunning { get => Get<bool>(); set => Set(value); }

        [JsonIgnore]
        public bool CBPC_Physics_running { get => Get<bool>(); set => Set(value); }

        [JsonIgnore]
        public bool cbpc15xbeta3 { get => Get<bool>(); set => Set(value); }

        [JsonIgnore]
        public bool writeAll { get => Get<bool>(); set => Set(value); }
        [JsonIgnore]
        public bool writeSome { get => Get<bool>(); set => Set(value); }
        [JsonIgnore]
        public bool overwriteCBPC_Physics { get => Get<bool>(); set => Set(value); }

    }
    public partial class VM_GENERAL : Notify.NotifyPropertyChanged
    {
        partial void DefaultCommon()
        {
            Version = string.Empty;
            Height = 570;
            Width = 830;
            useDesktop = true;
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string workdir = AppDomain.CurrentDomain.BaseDirectory;
            if (useDesktop || string.IsNullOrWhiteSpace(_dirMods)) workdir = desktop;

            _dirMods = @"C:\Games\FaceRim_SE-TA\SkyrimSE\mods";

            useMO2 = false;
            useBodyChange = false;
            useAdvanced = false;

            panelNumber = 0;
            bodychangeNumber = 0;

            // NIF
            _dirNIF_bodyslide = workdir;
            fileNIF_bodyslide = "femalebody_0.nif, femalebody_1.nif";
            fileNIF_bsHands = "femalehands_0.nif, femalehands_1.nif";
            useCustomExample = false;
            weightNumber = 2;
            fileNIF_sphere = System.IO.Path.Combine(workdir, "sphere.nif");
            AutoCalcSpheres = true;

            useNIFs = true;
            fileNIF_out0 = System.IO.Path.Combine(workdir, "inter_0.nif");
            fileNIF_out1 = System.IO.Path.Combine(workdir, "inter_1.nif");

            // CBPC
            _dirCBPC = workdir;
            fileCBPC_Collision = "CBPCollisionConfig_Female.txt";
            fileCBPC_Collisions = new() { "CBPCollisionConfig.txt", "CBPCollisionConfig_Female.txt" };
            groupfilter = "[NPC L Pussy02],[NPC L RearThigh],[NPC L Thigh [LThg]],[NPC L UpperArm [LUar]],[NPC L Forearm [LLar]]";
            overrideVaginal = false;

            //fileCBPC_Physics = "CBPConfig_3b.txt";
            fileCBPC_Physicss = new();
            cbpcUpdateFromFile_DefaultLocation = workdir;

            CBPC_Checker = new() { "# Collision spheres", "# Affected Nodes", "# Collider Nodes" };
            CBPCfullcopy = true;
            CBPCbackup = true;
            CBPCremoveUnnecessary = true;

        }
        public string Version { get => Get<string>(); set => Set(value); }
        public double Height { get => Get<double>(); set => Set(value); }
        public double Width { get => Get<double>(); set => Set(value); }
        public bool useDesktop { get => Get<bool>(); set => Set(value); }

        [JsonIgnore]
        public bool useMO2 { get => Get<bool>(); set { Set(value); OnPropertyChanged("dirMods"); OnPropertyChanged("dirNIF_bodyslide"); OnPropertyChanged("dirCBPC"); } }
        public bool useBodyChange { get => Get<bool>(); set { Set(value); OnPropertyChanged("dirNIF_bodyslide"); } }
        public bool useAdvanced { get => Get<bool>(); set => Set(value); }

        // DAR
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public string _dirMods { get => Get<string>(); set => Set(value); }
        [JsonIgnore]
        public string dirMods
        {
            get
            {
                string path = _dirMods;
                if (useMO2) path = @"Data";
                return path;
            }
        }
        [JsonIgnore]
        public string darWorkProgress { get => Get<string>(); set => Set(value); }

        // NIF-CBPC
        public int panelNumber { get => Get<int>(); set => Set(value); }
        [JsonIgnore]
        public bool mo2Detected { get => Get<bool>(); set { Set(value); if (value) useMO2 = true; } }

        public int bodychangeNumber { get => Get<int>(); set => Set(value); }

        // NIF
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public string _dirNIF_bodyslide { get => Get<string>(); set => Set(value.Replace("\\\\", "\\")); }
        [JsonIgnore]
        public string dirNIF_bodyslide
        {
            get
            {
                string path = _dirNIF_bodyslide;
                if (useMO2)
                {
                    if (useBodyChange) path = $@"Data\Meshes\BodyChange\CustomSet{bodychangeNumber + 1}";
                    else path = @"Data\meshes\actors\character\character assets";
                }
                return path;
            }
        }
        public string fileNIF_bodyslide { get => Get<string>(); set => Set(value); }
        public string fileNIF_bsHands { get => Get<string>(); set => Set(value); }

        public bool useCustomExample { get => Get<bool>(); set => Set(value); }
        public int weightNumber { get => Get<int>(); set { if (value > -1) Set(value); } }
        public string fileNIF_sphere { get => Get<string>(); set => Set(value); }

        [JsonIgnore]
        public bool overwriteInterNIFs { get => Get<bool>(); set => Set(value); }
        public bool AutoCalcSpheres { get => Get<bool>(); set => Set(value); }

        public bool useNIFs { get => Get<bool>(); set => Set(value); }
        public string fileNIF_out0 { get => Get<string>(); set => Set(value); }
        public string fileNIF_out1 { get => Get<string>(); set => Set(value); }
        [JsonIgnore]
        public string[] fileNIF_outs => new string[] { fileNIF_out0, fileNIF_out1 };


        // CBPC
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public string _dirCBPC { get => Get<string>(); set => Set(value.Replace("\\\\", "\\")); }
        [JsonIgnore]
        public string dirCBPC
        {
            get
            {
                string path = _dirCBPC;
                if (useMO2) path = @"Data\SKSE\Plugins";
                return path;
            }
        }

        public string fileCBPC_Collision { get => Get<string>(); set => Set(value); }
        public ObservableCollection<string> fileCBPC_Collisions { get => Get<ObservableCollection<string>>(); set => Set(value); }
        [JsonIgnore]
        public int fileCBPC_Collision_Index { get => Get<int>(); set => Set(value); }
        public string groupfilter { get => Get<string>(); set => Set(value); }
        public bool overrideVaginal { get => Get<bool>(); set => Set(value); }

        [JsonIgnore]
        public string fileCBPC_Physics { get => Get<string>(); set => Set(value); }
        [JsonIgnore]
        public ObservableCollection<string> fileCBPC_Physicss { get => Get<ObservableCollection<string>>(); set => Set(value); }

        public string cbpcUpdateFromFile_DefaultLocation { get => Get<string>(); set => Set(value); }

        public ObservableCollection<string> CBPC_Checker { get => Get<ObservableCollection<string>>(); set => Set(value); }
        public bool CBPCfullcopy { get => Get<bool>(); set => Set(value); }
        public bool CBPCbackup { get => Get<bool>(); set => Set(value); }
        public bool CBPCremoveUnnecessary { get => Get<bool>(); set => Set(value); }


        //[JsonIgnore]
        //public bool CBPCVisualLeftonly { get => Get<bool>(); set => Set(value); }



    }
}
