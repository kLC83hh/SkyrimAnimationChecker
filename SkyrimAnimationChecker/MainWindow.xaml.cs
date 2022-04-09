using System;
using System.Collections.Generic;
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
            vm = VM.Load();
            if (vm.LoadFailed) M.D("vmFile load failed");
            this.DataContext = vm;
            this.Closing += MainWindow_Closing;
        }

        private VM vm;
        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            vm.Save();
        }

        private async void RunDAR() => T.Text = await Task.Run(new DAR(vm).Run);

        private async void RunNIFE()
        {
            int res = await Task.Run(() => new NIF(vm).Run1(OutputWeight(), vm.overwriteInterNIFs));
            if (res != 1)
            {
                Action<string> msg = o => MessageBox.Show($"Error Code 01-{res}: {o}");
                switch (res)
                {
                    case 0: msg("Nothing is done"); break;
                    case 2: await ControlFlash(overwriteCheckBox); break;//msg("Output_0 file exists"); 
                    case 3: await ControlFlash(overwriteCheckBox); break;//msg("Output_1 file exists"); 
                    case 4: msg("Some of input_0 files are not exists"); break;
                    case 5: msg("Some of input_1 files are not exists"); break;
                    case 6: await ControlFlash(overwriteCheckBox); break;//msg("Output_0 and Output_1 files are exists"); 
                    case 10: msg("Output_0 file exists and some of input_1 files are not exists"); await ControlFlash(overwriteCheckBox); break;
                    case 12: msg("Output_1 file exists and some of input_0 files are not exists"); await ControlFlash(overwriteCheckBox); break;
                    case 20: msg("Some of input_0 and input_1 files are not exists"); break;
                }
            }
        }
        private List<int> flashing = new();
        private async Task ControlFlash(Control c)
        {
            if (flashing.Contains(c.GetHashCode())) return;
            flashing.Add(c.GetHashCode());
            int delay = 100;
            if (c is CheckBox)
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

        CBPC_collider_object[]? DATA;
        private async void RunNIF_CBPC()
        {
            DATA = await Task.Run(() => { return new NIF(vm).Run2(OutputWeight()); });
            if (DATA != null && DATA.Length > 0) Clist(DATA);
        }
        private int OutputWeight()
        {
            int val = -1;
            Dispatcher?.Invoke(() =>
            {
                foreach (RadioButton r in weightSelector.Children)
                {
                    if (r.IsChecked == true)
                    {
                        switch (r.Content.ToString())
                        {
                            case "Both": val = 2; break;
                            case "0": val = 0; break;
                            case "1":
                            default: val = 1; break;
                        }
                    }
                }
            });
            return val;
        }

        private async void RunCBPC()
        {
            if (DATA == null) return;
            //if (vm.writeSome) Clipboard.SetText(await Task.Run(() => new CBPC(vm).MakeOrganized(DATA)));
            if (vm.writeSome) await Task.Run(() => new CBPC(vm).Save(DATA));
            else
            {
                if (vm.CBPCfullcopy) Clipboard.SetText(await Task.Run(() => new CBPC(vm).MakeOrganized(DATA)));
                else Clipboard.SetText(await Task.Run(() => new CBPC(vm).MakeMessy(DATA)));
            }
        }

        private void DARDuplicateCheck_Button_Click(object sender, RoutedEventArgs e) => RunDAR();
        private void MakeIntermediumNIFs_1_Button_Click(object sender, RoutedEventArgs e) => RunNIFE();
        private void SphereSize_2_Button_Click(object sender, RoutedEventArgs e) => RunNIF_CBPC();
        private void WriteCollisionConfig_3_Button_Click(object sender, RoutedEventArgs e) => RunCBPC();



        private void Tlist(object[] o)
        {
            T.Text = string.Empty;
            foreach(var d in o)
            {
                T.Text += $"{d}\n";
            }
        }
        private void Nlist(object[] o)
        {
            N.Text = string.Empty;
            foreach (var d in o)
            {
                N.Text += $"{d}\n";
            }
        }
        private void Clist(CBPC_collider_object[] o)
        {
            //C.Text = string.Empty;
            CBPC_panel.Children.Clear();
            foreach (CBPC_collider_object d in o)
            {
                //C.Text += $"{d}\n";
                d.Write = vm.writeAll;
                var uc = new CBPC_collider() { ColliderObject = d };
                uc.ColliderUpdateEvent += Uc_ColliderUpdateEvent;
                CBPC_panel.Children.Add(uc);
            }
        }
        private void Uc_ColliderUpdateEvent(CBPC_collider_object collider)
        {
            //System.Diagnostics.Debug.WriteLine("Uc_ColliderUpdateEvent");
            if (collider.Write)
            {
                vm.writeSome = true;
                if (EveryWriteCheckboxIs(true)) vm.writeAll = true;
            }
            else
            {
                vm.writeAll = false;
                if (EveryWriteCheckboxIs(false)) vm.writeSome = false;
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
            else if (EveryWriteCheckboxIs(false)) vm.writeSome = false;
        }
        private bool EveryWriteCheckboxIs(bool tf)
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

        private void VMreset_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Reset settings to default", "Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK) vm = new VM();
        }




    }

}
