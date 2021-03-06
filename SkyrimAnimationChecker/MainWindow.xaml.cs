using SkyrimAnimationChecker.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SkyrimAnimationChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // vm linking
            vmm = new VM();
            try { if (!vmm.Load()) throw EE.New(101); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            this.vm = vmm.GENERAL;
            this.DataContext = vm;

            this.SetBinding(Window.HeightProperty, new Binding("Height") { Source = DataContext, Mode = BindingMode.TwoWay });
            this.SetBinding(Window.WidthProperty, new Binding("Width") { Source = DataContext, Mode = BindingMode.TwoWay });

            // events and shortcuts
            this.Closing += MainWindow_Closing;
            this.ContentRendered += MainWindow_ContentRendered;

            // title manipulation (version)
            foreach (System.Reflection.Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.GetName().Name == "SkyrimAnimationChecker")
                {
                    string v = a.GetName().Version?.ToString() ?? string.Empty;
                    while (v.EndsWith(".0")) { v = v.Remove(v.Length - 2); }
                    if (vm.Version != v)
                    {
                        if (!string.IsNullOrEmpty(vm.Version)) appUpdates = true;
                        vm.Version = v;
                    }
                    this.Title += $" {v}";
                }
            }
#if DEBUG
            this.Title += " Debug";
#endif

            // general background tasks
            _ = Notifying_RightPanel();
        }

        #region ui/ux operation
        private readonly VM vmm;
        private readonly VM_GENERAL vm;
        private readonly bool appUpdates = false;
        /* general events */
        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            vmm.Save();
            cts.Cancel();
        }
        private void VMreset_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Reset settings to default", "Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                vmm.Reset();
                CheckWhenStart();
            }
        }
        private void MainWindow_ContentRendered(object? sender, EventArgs e) => CheckWhenStart();
        private void CheckWhenStart()
        {
            if (appUpdates)
            {
                if (MessageBox.Show("The app is newly updated.\nDo you want to reset settings to default?", "Choose", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    vmm.Reset();
                }
            }
            if (CheckMO2()) vm.mo2Detected = true;
            //if (!vm.mo2Detected) vm.useAdvanced = true;
            LoadPhysicsLocation();
        }
        private static bool CheckMO2()
        {
            //string dir = System.IO.Directory.GetCurrentDirectory();
            //if (System.IO.Directory.Exists(System.IO.Path.Combine(dir, @"SKSE\Plugins")) &&
            //    System.IO.Directory.Exists(System.IO.Path.Combine(dir, @"meshes\actors\character\character assets")))
            //MessageBox.Show(
            //    $"{System.IO.Path.GetDirectoryName(@"data\SKSE\Plugins")}\n" +
            //    $"{System.IO.Directory.Exists(@"data\SKSE\Plugins")}" +
            //    $"{System.IO.Directory.Exists(@"data\meshes\actors\character\character assets")}"
            //    );
            if (System.IO.Directory.Exists(@"data\SKSE\Plugins") &&
                System.IO.Directory.Exists(@"data\meshes\actors\character\character assets"))
                return true;
            else
                return false;
        }

        /* shortcut commands */
        private static RoutedCommand MakeCommand(KeyGesture gesture, [System.Runtime.CompilerServices.CallerMemberName] string name = "")
            => new(name, typeof(Window), new InputGestureCollection() { gesture });
        public static readonly RoutedCommand SaveCommand = MakeCommand(new KeyGesture(Key.S, ModifierKeys.Control));
        private async void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command is RoutedCommand com)
            {
                switch (com.Name)
                {
                    case var value when value == SaveCommand.Name:
                        switch (vm.panelNumber)
                        {
                            case 0:
                                await PushToCBPC();
                                break;
                            case 1:
                                await WritePhysics();
                                break;
                        }
                        break;
                }
            }
        }

        /* text notifying */
        private readonly System.Threading.CancellationTokenSource cts = new();
        private readonly Queue<string> NotifyRightCQ = new(), NotifyRightPQ = new();
        private async Task Notifying_RightPanel()
        {
            int showtime = 1050, timerC = 0, timerP = 0;
            try
            {
                while (true)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    if (NotifyRightCQ.Count > 0 && timerC <= 0)
                    {
                        string buffer = NotifyRightCQ.Dequeue();
                        if (!string.IsNullOrWhiteSpace(buffer)) Dispatcher?.Invoke(() => { NotifyC.Text = $" - {buffer}"; });
                        timerC = showtime;
                    }
                    if (NotifyRightPQ.Count > 0 && timerP <= 0)
                    {
                        string buffer = NotifyRightPQ.Dequeue();
                        if (!string.IsNullOrWhiteSpace(buffer)) Dispatcher?.Invoke(() => { NotifyP.Text = $" - {buffer}"; });
                        timerP = showtime;
                    }
                    await Task.Delay(50);
                    timerC -= 50;
                    timerP -= 50;
                    if (timerC <= 0)
                        Dispatcher?.Invoke(() => { if (NotifyC.Text != string.Empty) NotifyC.Text = string.Empty; });
                    if (timerP <= 0)
                        Dispatcher?.Invoke(() => { if (NotifyP.Text != string.Empty) NotifyP.Text = string.Empty; });
                }
            }
            catch (OperationCanceledException) { }
        }

        /* flashing ui */
        private readonly List<int> flashing = new();
        private async Task FlashUI(Control c)
        {
            if (flashing.Contains(c.GetHashCode())) return;
            flashing.Add(c.GetHashCode());
            int delay = 100;
            if (c is CheckBox || c is Label)
            {
                c.FontWeight = FontWeights.Bold;
                await Task.Delay(delay);
                c.FontWeight = FontWeights.Normal;
                await Task.Delay(delay);
                c.FontWeight = FontWeights.Bold;
                await Task.Delay(delay);
                c.FontWeight = FontWeights.Normal;
            }
            flashing.RemoveAll(item => item == c.GetHashCode());
        }
        #endregion


        #region DAR
        private async void RunDAR()
        {
            Dictionary<int, List<string>> dar = await Task.Run(new DAR.DAR(vmm).Run);
            string dupres = dar.Count > 0 ? $"Found [{dar.Count}]" : "None";
            T.Text = $"DAR Number Duplicate => {dupres}";

            string sres = string.Empty;
            dar.ForEach(x => sres += $"{x.Key}={string.Join(',', x.Value.ToArray())}\n");
            if (sres.EndsWith('\n')) sres = sres[..^1];
            darBox.Text = sres.Trim();
        }
        private void DARDuplicateCheck_Button_Click(object sender, RoutedEventArgs e) => RunDAR();
        #endregion

        // no-brainer
        private async void DoIt_Button_Click(object sender, RoutedEventArgs e) => await DoIt();
        private async Task DoIt()
        {
            try
            {
                vm.DoItRunning = true;
                // flag for checking new NIFs were made
                bool newmade = false;

                // step for make NIFs
                // check weight 0
                bool do0 = (vm.weightNumber == 2 || vm.weightNumber == 0) && !System.IO.File.Exists(vm.fileNIF_out0);
                // check weight 1
                bool do1 = (vm.weightNumber == 2 || vm.weightNumber == 1) && !System.IO.File.Exists(vm.fileNIF_out1);
                if (do0 || do1)
                {
                    newmade = true;
                    if (vm.weightNumber == 2)
                    {
                        try
                        {
                            if (System.IO.File.Exists(vm.fileNIF_out0)) vm.weightNumber = 1;
                            else if (System.IO.File.Exists(vm.fileNIF_out1)) vm.weightNumber = 0;
                            await RunMakeInterNIF();
                            await RetrieveFromCBPC();
                            if (vm.AutoCalcSpheres) await CalcSpheres();
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        finally { vm.weightNumber = 2; }
                    }
                    else
                    {
                        await RunMakeInterNIF();
                        await RetrieveFromCBPC();
                        if (vm.AutoCalcSpheres) await CalcSpheres();
                    }
                }

                // step for update cbpc config
                // check weight 0
                do0 = System.IO.File.Exists(vm.fileNIF_out0);
                // check weight 1
                do1 = System.IO.File.Exists(vm.fileNIF_out1);
                //bool do2 = do0 && do1;
                if (do0 || do1)
                {
                    bool useNIFsBackup = vm.useNIFs, writeAllBackup = vm.writeAll, writeSomeBackup = vm.writeSome;
                    try
                    {
                        //if (DATA_Colliders == null) newmade = true;
                        if (newmade || vm.useNIFs)
                        {
                            vm.useNIFs = true;
                            await MakeCBPCDatas();
                        }
                        else if (DATA_Colliders == null) await MakeCBPCDatas();
                        vm.writeAll = true;
                        vm.writeSome = true;
                        await PushToCBPC();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                    finally
                    {
                        if (!newmade) vm.useNIFs = useNIFsBackup;
                        vm.writeAll = writeAllBackup;
                        vm.writeSome = writeSomeBackup;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { vm.DoItRunning = false; }
        }

        #region Collision
        // make inter nif
        private async void MakeIntermediumNIFs_1_Button_Click(object sender, RoutedEventArgs e) => await RunMakeInterNIF();
        private async Task RunMakeInterNIF()
        {
            //MessageBox.Show(vm.dirNIF_bodyslide);
            int res = await Task.Run(() => new NIF.Collider(vm).Make());
            if (res != 1)
            {
                void msg(string o) => MessageBox.Show($"Error Code 01-{res}: {o}");
                switch (res)
                {
                    case 0: msg("Nothing is done"); break;
                    case 2: await FlashUI(overwriteCheckBox); break;//msg("Output_0 file exists"); 
                    case 3: await FlashUI(overwriteCheckBox); break;//msg("Output_1 file exists"); 
                    case 4: msg("Some of input_0 files are not exists"); break;
                    case 5: msg("Some of input_1 files are not exists"); break;
                    case 6: await FlashUI(overwriteCheckBox); break;//msg("Output_0 and Output_1 files are exists"); 
                    case 10: msg("Output_0 file exists and some of input_1 files are not exists"); await FlashUI(overwriteCheckBox); break;
                    case 12: msg("Output_1 file exists and some of input_0 files are not exists"); await FlashUI(overwriteCheckBox); break;
                    case 20: msg("Some of input_0 and input_1 files are not exists"); break;
                }
            }
        }

        // uato calc collision spheres
        private async void CalcSpheres_Button_Click(object sender, RoutedEventArgs e) => await CalcSpheres();
        private async Task CalcSpheres()
        {
            try
            {
                vm.NIFrunning = true;
                await Task.Run(() => new NIF.Collider(vm).CalsSpheres());
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { vm.NIFrunning = false; }
        }

        private void CollisionFile_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string? buffer = ColFileCB.Items.GetItemAt(ColFileCB.SelectedIndex).ToString();
            //if (buffer != null)
            //{
            //vm.fileCBPC_Collision = buffer;
            //RunNIF_CBPC();
            //}
        }
        // update nif from cbpc
        private async void SphereSize_Retrieve_Button_Click(object sender, RoutedEventArgs e) => await RetrieveFromCBPC();
        private async Task RetrieveFromCBPC()
        {
            try
            {
                vm.NIFrunning = true;
                if (DATA_Colliders == null)
                {
                    bool useNifsBackup = vm.useNIFs;
                    vm.useNIFs = false;
                    try { await MakeCBPCDatas(); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                    finally { vm.useNIFs = useNifsBackup; }
                }
                if (DATA_Colliders != null && DATA_Colliders.Length > 0)
                {
                    await Task.Run(() => { new NIF.Collider(vm).UpdateSpheres(DATA_Colliders); });
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { vm.NIFrunning = false; }
        }
        // get sphere data
        private async void SphereSize_2_Button_Click(object sender, RoutedEventArgs e) => await MakeCBPCDatas();
        collider_object[]? DATA_Colliders;
        (cc_options_object op, cc_extraoptions_object eop)? Options;
        private async Task MakeCBPCDatas()
        {
            try
            {
                if (vm.fileCBPC_Collision_Index == -1) vm.fileCBPC_Collision_Index = vm.fileCBPC_Collisions.IndexOf("CBPCollisionConfig_Female.txt");
                try { Options = await Task.Run(() => new CBPC.Collision(vm).Option()); }
                catch (Exception ex) { if (EE.Parse(ex).code == 5201) Options = null; }
                if (vm.useNIFs)
                {
                    var (code, msg) = await Task.Run(() => new NIF.Collision(vm).Get(out DATA_Colliders));
                    if (code > 0) MessageBox.Show(msg);
                }
                else
                {
                    var buffer = await Task.Run(() => new CBPC.Collision(vm).GetColliders());
                    if (buffer.Length > 0) DATA_Colliders = buffer;
                }
                if (DATA_Colliders != null && DATA_Colliders.Length > 0)
                {
                    Clist(DATA_Colliders);
                    if (Options != null)
                    {
                        ColOptions.Options = Options.Value.op;
                        ColOptions.ExtraOptions = Options.Value.eop;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void Clist(collider_object[] o)
        {
            //C.Text = string.Empty;
            CBPC_panel.Children.Clear();
            foreach (collider_object d in o)
            {
                //C.Text += $"{d}\n";
                d.Write = vm.writeAll;
                var uc = new CBPC_collider() { ColliderObject = d };
                uc.ColliderUpdateEvent += Uc_ColliderUpdateEvent;
                CBPC_panel.Children.Add(uc);
            }
        }
        private void Uc_ColliderUpdateEvent(collider_object collider)
        {
            //System.Diagnostics.Debug.WriteLine("Uc_ColliderUpdateEvent");
            if (collider.Write)
            {
                vm.writeSome = true;
                if (IsEveryWriteCheckbox(true)) vm.writeAll = true;
            }
            else
            {
                vm.writeAll = false;
                if (IsEveryWriteCheckbox(false)) vm.writeSome = false;
            }
        }

        // save data to cbpc
        private async void WriteCollisionConfig_3_Button_Click(object sender, RoutedEventArgs e) => await PushToCBPC();
        private async Task PushToCBPC()
        {
            if (DATA_Colliders == null) return;
            CBPC_panel.Focus();
            if (vm.writeSome)
            {
                NotifyRightCQ.Enqueue("Save");
                await Task.Run(() => new CBPC.Collision(vmm).Save(DATA_Colliders, Options));
            }
            else
            {
                NotifyRightCQ.Enqueue("Copy");
                if (vm.CBPCfullcopy) Clipboard.SetText(await Task.Run(() => new CBPC.Collision(vmm).MakeOrganized(DATA_Colliders, Options)));
                else Clipboard.SetText(await Task.Run(() => new CBPC.Collision(vmm).MakeMessy(DATA_Colliders, Options)));
            }
        }

        // check write checkbox
        private void CBPC_panel_All_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null || sender is not CheckBox) return;
            bool check(string name)
            {
                string[] vaginal = new[] { "Vagina", "Clit", "Pussy" };
                foreach (string v in vaginal)
                {
                    if (!vm.overrideVaginal && name.Contains(v)) return false;
                }
                return true;
            }
            void set(bool val)
            {
                foreach (var d in CBPC_panel.Children)
                {
                    if (d is CBPC_collider collider)
                    {
                        collider.ColliderObject.Write = val && check(collider.ColliderObject.Name);
                    }
                }
            }
            set(vm.writeAll);
            if (vm.writeAll) vm.writeSome = true;
            else if (IsEveryWriteCheckbox(false)) vm.writeSome = false;
        }
        private void CheckBox_Check_Changed(object sender, RoutedEventArgs e)
        {
            if (sender == null || sender is not CheckBox) return;
            bool check(string name)
            {
                string[] vaginal = new[] { "Vagina", "Clit", "Pussy" };
                foreach (string v in vaginal)
                {
                    if (!vm.overrideVaginal && name.Contains(v)) return false;
                }
                return true;
            }
            void set(bool val)
            {
                foreach (var d in CBPC_panel.Children)
                {
                    if (d is CBPC_collider collider)
                    {
                        collider.ColliderObject.Write = val && check(collider.ColliderObject.Name);
                    }
                }
            }
            set(vm.writeAll);
            if (vm.writeAll) vm.writeSome = true;
            else if (IsEveryWriteCheckbox(false)) vm.writeSome = false;
        }
        private bool IsEveryWriteCheckbox(bool tf)
        {
            if (tf)
            {
                bool checker = true;
                foreach (var d in CBPC_panel.Children)
                {
                    if (d is CBPC_collider collider) checker &= collider.ColliderObject.Write;
                }
                return checker;
            }
            else
            {
                bool checker = false;
                foreach (var d in CBPC_panel.Children)
                {
                    if (d is CBPC_collider collider) checker |= collider.ColliderObject.Write;
                }
                return !checker;
            }
        }
        #endregion
        #region Physics
        private void CommonCombobox_MouseEnter(object sender, MouseEventArgs e) => (sender as ComboBox)?.Focus();
        private async void PhysicsFile_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PhyFileCB.Items.Count == 0) return;
            string? buffer = PhyFileCB.Items.GetItemAt(PhyFileCB.SelectedIndex).ToString();
            if (buffer != null)
            {
                vm.fileCBPC_Physics = buffer;
                await ReadPhysics();
            }
        }
        CBPC.Icbpc_data? DATA_CBPC;
        private volatile bool PhysicsReading = false;
        private bool? bindlr = null;//, leftonly = false
        private int collective = 0;
        private async Task ReadPhysics()
        {
            if (PhysicsReading) return;
            try
            {
                PhysicsReading = true;
                vm.CBPC_Physics_running = true;

                await Task.Run(() =>
                {
                    //M.D(DATA_3BA?.L1.Data.collisionElastic.Value1);
                    try { DATA_CBPC = new CBPC.Physics(vm).GetPhysics(); }
                    catch (Exception e)
                    {
                        (int c, string msg) = EE.Parse(e);
                        MessageBox.Show(msg);
                        return;
                    }

                    switch (DATA_CBPC.DataType)
                    {
                        case "3ba":
                        case "leg":
                        case "vagina":
                            Dispatcher?.Invoke(() =>
                            {
                                if (cbpcPhyPanel.Children.Count == 1 && cbpcPhyPanel.Children[0] is CBPC_Physics_MultiBone w)
                                    w.Data = (CBPC.Icbpc_data_multibone)DATA_CBPC;
                                else
                                {
                                    cbpcPhyPanel.Children.Clear();
                                    var page = new CBPC_Physics_MultiBone(vmm, (CBPC.Icbpc_data_multibone)DATA_CBPC, bindlr: bindlr, collective: collective);
                                    //page.LeftOnlyUpdated += (val) => leftonly = val;
                                    page.BindLRUpdated += (val) => bindlr = val;
                                    page.CollectiveUpdated += (val) => collective = val;
                                    cbpcPhyPanel.Children.Add(page);
                                }
                            });
                            break;
                        case "bbp":
                        case "belly":
                        case "butt":
                        case "single":
                        case "mirrored":
                            Dispatcher?.Invoke(() =>
                            {
                                if (cbpcPhyPanel.Children.Count == 1 && cbpcPhyPanel.Children[0] is CBPC_Physics w)
                                    w.Data = DATA_CBPC;
                                else
                                {
                                    cbpcPhyPanel.Children.Clear();
                                    var page = new CBPC_Physics(vmm, DATA_CBPC, bindlr: bindlr, collective: collective);
                                    //page.LeftOnlyUpdated += (val) => leftonly = val;
                                    page.BindLRUpdated += (val) => bindlr = val;
                                    page.CollectiveUpdated += (val) => collective = val;
                                    cbpcPhyPanel.Children.Add(page);
                                }
                            });
                            break;
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally
            {
                vm.CBPC_Physics_running = false;
                PhysicsReading = false;
            }
        }

        private async void WritePhysics_Button_Click(object sender, RoutedEventArgs e) => await WritePhysics();
        private async Task WritePhysics()
        {
            if (DATA_CBPC == null) return;
            try
            {
                vm.CBPC_Physics_running = true;
                cbpcPhyPanel.Focus();
                await Task.Run(() =>
                {
                    if (vm.overwriteCBPC_Physics)
                    {
                        NotifyRightPQ.Enqueue("Save");
                        new CBPC.Physics(vm).Save(DATA_CBPC, vm.overwriteCBPC_Physics);
                    }
                    else
                    {
                        NotifyRightPQ.Enqueue("Copy");
                        Dispatcher?.Invoke(() => Clipboard.SetText(new CBPC.Physics(vm).MakeCBPConfig(DATA_CBPC)));
                    }
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { vm.CBPC_Physics_running = false; }
        }


        #endregion
        #region CBPC common
        // because it IS used
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void CBPCLocationLoader_Load(FolderLoader sender) => LoadPhysicsLocation(sender);
        private readonly string[] filter = new string[] { "3b", "BBP", "butt", "belly", "leg", "Vagina" };

        private async void LoadPhysicsLocation(FolderLoader? sender = null)
        {
            if (System.IO.Directory.Exists(vm.dirCBPC))
            {
                string getfilename(string path)
                {
                    string name = path;
                    if (name.Contains('/')) name = path.Split('/').Last();
                    if (name.Contains('\\')) name = path.Split('\\').Last();
                    if (name.Contains("\\\\")) name = path.Split("\\\\").Last();
                    return name;
                }
                string[] getfiles(string searchPattern, string[]? filter)
                {
                    string[] files = System.IO.Directory.GetFiles(vm.dirCBPC, searchPattern).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    List<string> output = new();
                    foreach (string file in files)
                    {
                        if (filter == null) output.Add(getfilename(file));
                        else
                        {
                            foreach (string filterItem in filter)
                            {
                                if (file.Contains(filterItem)) output.Add(getfilename(file));
                            }
                        }
                    }
                    return output.ToArray();
                }

                vm.fileCBPC_Collisions.Clear();
                getfiles("CBPCollisionConfig*", null).ForEach(x => vm.fileCBPC_Collisions.Add(x));
                if (vm.fileCBPC_Collisions.Contains(vm.fileCBPC_Collision)) vm.fileCBPC_Collision_Index = vm.fileCBPC_Collisions.IndexOf(vm.fileCBPC_Collision);
                if (vm.fileCBPC_Collision_Index == -1) vm.fileCBPC_Collision_Index = vm.fileCBPC_Collisions.IndexOf("CBPCollisionConfig_Female.txt");

                vm.fileCBPC_Physicss.Clear();
                getfiles("CBPConfig*", filter).ForEach(x => vm.fileCBPC_Physicss.Add(x));
            }
            else
            {
                if (sender != null) await sender.Flash();
                MessageBox.Show(EE.List[12001]);
            }
        }

        #endregion



        private async void CBPCPhysics_UpdateFromText_Button_Click(object sender, RoutedEventArgs e)
        {
            var textdialog = new TextInputDialog();
            bool? result = textdialog.ShowDialog();
            if (result == true)
            {
                new CBPC.Physics(vm).Replace(textdialog.Text);
                await ReadPhysics();
            }
        }
        private async void CBPCPhysics_UpdateFromFile_Button_Click(object sender, RoutedEventArgs e)
        {
            var filedialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Text Documents|*.txt"
            };
            if (System.IO.Directory.Exists(vm.cbpcUpdateFromFile_DefaultLocation)) filedialog.InitialDirectory = vm.cbpcUpdateFromFile_DefaultLocation;
            bool? result = filedialog.ShowDialog();
            if (result == true)
            {
                new CBPC.Physics(vm).ReplaceFile(filedialog.FileName);
                await ReadPhysics();
            }
        }


    }

    internal class BackgroundTask
    {
        public BackgroundTask() { }
        public BackgroundTask(Action a) => A = a;
        protected System.Threading.CancellationTokenSource cts = new();

        public void Start(System.Windows.Threading.DispatcherOperation? d, int wait = 0)
            => Start(async () => { if (d != null) await d; }, wait);
        public void Start(Action? a = null, int wait = 0)
        {
            Working = true;
            cts = new();
            if (a != null) A = a;
            _ = _Task(wait);
        }
        public void Stop() => cts.Cancel();

        public bool Working { get; private set; } = false;
        public int Delay { get; set; } = 25;
        public Action? A { get; set; }


        //private Task? Task { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        private async Task _Task(int wait = 0)
        {
            if (A == null) { Working = false; return; }
            if (wait > 0) Delay = wait;
            try
            {
                while (true)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    await Task.Run(A);
                    cts.Token.ThrowIfCancellationRequested();
                    if (Delay > 0) await Task.Delay(Delay);
                }
            }
            catch (OperationCanceledException) { Working = false; }
        }
    }

}
