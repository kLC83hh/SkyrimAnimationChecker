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
            vmm = new VM();
            if (!vmm.Load()) M.D("vmFile load failed");
            this.vm = vmm.GENERAL;
            this.DataContext = vm;
            this.Closing += MainWindow_Closing;
            this.ContentRendered += MainWindow_ContentRendered;
        }

        private VM vmm;
        private VM_GENERAL vm;
        private void VMreset_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Reset settings to default", "Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK) vm.Reset();
        }
        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            vmm.Save();
        }
        private void MainWindow_ContentRendered(object? sender, EventArgs e)
        {
            LoadPhysicsLocation();
        }

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


        #region DAR
        private async void RunDAR() => T.Text = await Task.Run(new DAR.DAR(vmm).Run);
        private void DARDuplicateCheck_Button_Click(object sender, RoutedEventArgs e) => RunDAR();
        #endregion
        #region Collision
        private void MakeIntermediumNIFs_1_Button_Click(object sender, RoutedEventArgs e) => RunMakeInterNIF();
        private async void RunMakeInterNIF()
        {
            int res = await Task.Run(() => new NIF.CollisionNIFs(vm).Make());
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


        private void SphereSize_2_Button_Click(object sender, RoutedEventArgs e) => RunNIF_CBPC();
        collider_object[]? DATA_Colliders;
        private async void RunNIF_CBPC()
        {
            try
            {
                var result = await Task.Run(() => new NIF.Colliders(vm).Get(out DATA_Colliders));
                if (result.code > 0) MessageBox.Show(result.msg);
                if (DATA_Colliders != null && DATA_Colliders.Length > 0) Clist(DATA_Colliders);
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
            if (vm.writeSome) await Task.Run(() => new CBPC.Colliders(vmm).Save(DATA_Colliders));
            else
            {
                if (vm.CBPCfullcopy) Clipboard.SetText(await Task.Run(() => new CBPC.Colliders(vmm).MakeOrganized(DATA_Colliders)));
                else Clipboard.SetText(await Task.Run(() => new CBPC.Colliders(vmm).MakeMessy(DATA_Colliders)));
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
        private void ReloadPhysics_Button_Click(object sender, RoutedEventArgs e) => LoadPhysicsLocation();
        private void PhysicsLocation_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                LoadPhysicsLocation();
            }
        }
        private async void LoadPhysicsLocation()
        {
            if (System.IO.Directory.Exists(vm.locationCBPC_Physics))
            {
                PhyFileCB.Items.Clear();
                string[] files = System.IO.Directory.GetFiles(vm.locationCBPC_Physics, "CBPConfig*").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                string[] filter = new string[] { "3b" };
                foreach (string file in files)
                {
                    foreach (string filterItem in filter)
                    {
                        if (file.Contains(filterItem))
                        {
                            string name = file;
                            if (name.Contains('/')) name = file.Split('/').Last();
                            if (name.Contains('\\')) name = file.Split('\\').Last();
                            if (name.Contains("\\\\")) name = file.Split("\\\\").Last();
                            PhyFileCB.Items.Add(name);
                        }
                    }
                }
                //if (PhyFileCB.Items.Contains(vm.fileCBPC_Physics)) PhyFileCB.SelectedIndex = PhyFileCB.Items.IndexOf(vm.fileCBPC_Physics);
                //else PhyFileCB.SelectedIndex = 0;
            }
            else { await FlashUI(PhyLocTB); MessageBox.Show(EE.List[12001]); }
        }

        private void PhysicsFile_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? buffer = PhyFileCB.Items.GetItemAt(PhyFileCB.SelectedIndex).ToString();
            if (buffer != null)
            {
                vm.fileCBPC_Physics = buffer;
                ReadPhysics();
            }
        }
        CBPC.breast_3ba_object? DATA_3BA;
        private volatile bool PhysicsReading = false;
        private async void ReadPhysics()
        {
            if (PhysicsReading) return;
            PhysicsReading = true;
            vm.CBPC_Physics_running = true;
            await Task.Run(() => {
                //M.D(DATA_3BA?.L1.Data.collisionElastic.Value1);
                try { DATA_3BA = new CBPC.FPhysics(vm).GetPhysics(); }
                catch (Exception e)
                {
                    (int c, string msg) = EE.Parse(e);
                    MessageBox.Show(msg);
                    vm.CBPC_Physics_running = false;
                    PhysicsReading = false;
                    return;
                }
                Dispatcher?.Invoke(() => {
                    if (cbpcPhyPanel.Children.Count == 1 && cbpcPhyPanel.Children[0] is CBPC_3BA)
                        (cbpcPhyPanel.Children[0] as CBPC_3BA).Data = DATA_3BA;
                    else
                    {
                        cbpcPhyPanel.Children.Clear();
                        cbpcPhyPanel.Children.Add(new CBPC_3BA(vmm, DATA_3BA));
                    }
                });
            });
            vm.CBPC_Physics_running = false;
            PhysicsReading = false;
        }

        private void WritePhysics_Button_Click(object sender, RoutedEventArgs e) => WritePhysics();
        private async void WritePhysics()
        {
            if (DATA_3BA == null) return;
            vm.CBPC_Physics_running = true;
            await Task.Run(() =>
            {
                if (vm.overwriteCBPC_Physics) new CBPC.FPhysics(vm).Save(DATA_3BA, vm.overwriteCBPC_Physics);
                else Dispatcher?.Invoke(() => Clipboard.SetText(new CBPC.FPhysics(vm).Make3BA(DATA_3BA)));
            });
            vm.CBPC_Physics_running = false;
        }


        #endregion

    }

}
