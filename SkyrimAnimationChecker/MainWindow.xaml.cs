using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SkyrimAnimationChecker.Common;

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
            SaveCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            
            // title manipulation (version)
            foreach (System.Reflection.Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.GetName().Name == "SkyrimAnimationChecker")
                {
                    string v = a.GetName().Version?.ToString() ?? string.Empty;
                    while (v.EndsWith(".0")) { v = v.Remove(v.Length - 2); }
                    this.Title += $" {v}";
                }
            }

            // general background tasks
            _ = Notifying_RightPanel();
        }

        #region ui/ux operation
        private VM vmm;
        private VM_GENERAL vm;
        /* general events */
        private void VMreset_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Reset settings to default", "Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK) vmm.Reset();
        }
        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            vmm.Save();
            cts.Cancel();
        }
        private void MainWindow_ContentRendered(object? sender, EventArgs e)
        {
            if (CheckMO2()) vm.mo2Detected = true;
            LoadPhysicsLocation();
        }
        private bool CheckMO2()
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
        public static RoutedCommand SaveCommand = new();
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            switch (vm.panelNumber)
            {
                case 0:
                    RunCBPC();
                    break;
                case 1:
                    WritePhysics();
                    break;
            }
        }

        /* text notifying */
        private System.Threading.CancellationTokenSource cts = new();
        Queue<string> NotifyRightCQ = new(), NotifyRightPQ = new();
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
        private List<int> flashing = new();
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
            if (sres.EndsWith('\n')) sres = sres.Substring(0, sres.Length - 1);
            sres.Trim();
            darBox.Text = sres;
        }
        private void DARDuplicateCheck_Button_Click(object sender, RoutedEventArgs e) => RunDAR();
        #endregion

        #region Collision
        private void MakeIntermediumNIFs_1_Button_Click(object sender, RoutedEventArgs e) => RunMakeInterNIF();
        private async void RunMakeInterNIF()
        {
            //MessageBox.Show(vm.dirNIF_bodyslide);
            int res = await Task.Run(() => new NIF.Collider(vm).Make());
            if (res != 1)
            {
                Action<string> msg = o => MessageBox.Show($"Error Code 01-{res}: {o}");
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

        private void CollisionFile_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string? buffer = ColFileCB.Items.GetItemAt(ColFileCB.SelectedIndex).ToString();
            //if (buffer != null)
            //{
            //vm.fileCBPC_Collision = buffer;
            //RunNIF_CBPC();
            //}
        }
        private void SphereSize_2_Button_Click(object sender, RoutedEventArgs e) => RunNIF_CBPC();
        collider_object[]? DATA_Colliders;
        (cc_options_object op, cc_extraoptions_object eop)? Options;
        private async void RunNIF_CBPC()
        {
            try
            {
                Options = await Task.Run(() => new CBPC.Collision(vm).Option());
                if (vm.readFromNIFs)
                {
                    var result = await Task.Run(() => new NIF.Collision(vm).Get(out DATA_Colliders));
                    if (result.code > 0) MessageBox.Show(result.msg);
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


        private void WriteCollisionConfig_3_Button_Click(object sender, RoutedEventArgs e) => RunCBPC();
        private async void RunCBPC()
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


        private void CBPC_panel_All_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null || sender is not CheckBox) return;
            Action<bool> set = (val) =>
            {
                foreach (var d in CBPC_panel.Children)
                {
                    if (d is CBPC_collider) ((CBPC_collider)d).ColliderObject.Write = val;
                }
            };
            set((sender as CheckBox)?.IsChecked == true);
            if ((sender as CheckBox)?.IsChecked == true) vm.writeSome = true;
            else if (IsEveryWriteCheckbox(false)) vm.writeSome = false;
        }
        private bool IsEveryWriteCheckbox(bool tf)
        {
            if (tf)
            {
                bool checker = true;
                foreach (var d in CBPC_panel.Children)
                {
                    if (d is CBPC_collider) checker &= ((CBPC_collider)d).ColliderObject.Write;
                }
                return checker;
            }
            else
            {
                bool checker = false;
                foreach (var d in CBPC_panel.Children)
                {
                    if (d is CBPC_collider) checker |= ((CBPC_collider)d).ColliderObject.Write;
                }
                return !checker;
            }
        }
        #endregion
        #region Physics
        private void CommonCombobox_MouseEnter(object sender, MouseEventArgs e) => (sender as ComboBox)?.Focus();
        private void PhysicsFile_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PhyFileCB.Items.Count == 0) return;
            string? buffer = PhyFileCB.Items.GetItemAt(PhyFileCB.SelectedIndex).ToString();
            if (buffer != null)
            {
                vm.fileCBPC_Physics = buffer;
                ReadPhysics();
            }
        }
        CBPC.Icbpc_data? DATA_CBPC;
        private volatile bool PhysicsReading = false;
        private bool? leftonly = false, bindlr = null;
        private int collective = 0;
        private async void ReadPhysics()
        {
            if (PhysicsReading) return;
            PhysicsReading = true;
            vm.CBPC_Physics_running = true;

            await Task.Run(() => {
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
                                page.LeftOnlyUpdated += (val) => leftonly = val;
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
                                page.LeftOnlyUpdated += (val) => leftonly = val;
                                page.BindLRUpdated += (val) => bindlr = val;
                                page.CollectiveUpdated += (val) => collective = val;
                                cbpcPhyPanel.Children.Add(page);
                            }
                        });
                        break;
                }
            });

            vm.CBPC_Physics_running = false;
            PhysicsReading = false;
        }

        private void WritePhysics_Button_Click(object sender, RoutedEventArgs e) => WritePhysics();
        private async void WritePhysics()
        {
            if (DATA_CBPC == null) return;
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
            vm.CBPC_Physics_running = false;
        }


        #endregion
        #region CBPC common
        private void CBPCLocationLoader_Load(FolderLoader sender) => LoadPhysicsLocation(sender);
        string[] filter = new string[] { "3b", "BBP", "butt", "belly", "leg", "Vagina" };

        private async void LoadPhysicsLocation(FolderLoader? sender = null)
        {
            //MessageBox.Show(vm.dirCBPC);
            if (System.IO.Directory.Exists(vm.dirCBPC))
            {
                Func<string, string> getfilename = (path) =>
                {
                    string name = path;
                    if (name.Contains('/')) name = path.Split('/').Last();
                    if (name.Contains('\\')) name = path.Split('\\').Last();
                    if (name.Contains("\\\\")) name = path.Split("\\\\").Last();
                    return name;
                };
                Func<string, string[]?, string[]> getfiles = (searchPattern, filter) => {
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
                };
                //ColFileCB.Items.Clear();
                //getfiles("CBPCollisionConfig*", null).ForEach(x => ColFileCB.Items.Add(x));
                vm.fileCBPC_Collisions.Clear();
                getfiles("CBPCollisionConfig*", null).ForEach(x => vm.fileCBPC_Collisions.Add(x));
                //PhyFileCB.Items.Clear();
                //getfiles("CBPConfig*", filter).ForEach(x => PhyFileCB.Items.Add(x));
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


        private void CBPCPhysics_UpdateFromText_Button_Click(object sender, RoutedEventArgs e)
        {
            var textdialog = new TextInputDialog();
            bool? result = textdialog.ShowDialog();
            if (result == true)
            {
                new CBPC.Physics(vm).Replace(textdialog.Text);
                ReadPhysics();
            }
        }
        private void CBPCPhysics_UpdateFromFile_Button_Click(object sender, RoutedEventArgs e)
        {
            var filedialog = new Microsoft.Win32.OpenFileDialog();
            filedialog.DefaultExt = ".txt";
            filedialog.Filter = "Text Documents|*.txt";
            if (System.IO.Directory.Exists(vm.cbpcUpdateFromFile_DefaultLocation)) filedialog.InitialDirectory = vm.cbpcUpdateFromFile_DefaultLocation;
            bool? result = filedialog.ShowDialog();
            if (result == true)
            {
                new CBPC.Physics(vm).ReplaceFile(filedialog.FileName);
                ReadPhysics();
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
