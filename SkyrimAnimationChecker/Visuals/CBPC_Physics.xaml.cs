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
using SkyrimAnimationChecker.Common;
using SkyrimAnimationChecker.CBPC;

namespace SkyrimAnimationChecker.Common
{
    public class VM_VPhysics : Notify.NotifyPropertyChanged, IVM_Visual
    {
        public VM_VPhysics() => Default_VPhysics();
        protected void Default_VPhysics()
        {
            VMPhysics_BoneAll = true;
            VMPhysics_BoneSelect = 1;
            VMPhysics_ShowLeftOnly = false;
            VMPhysics_MirrorFilter = "default";
            VMPhysics_MirrorPair = "default";
            VMphysics_BindLR = true;
        }
        public void Reset() => Default_VPhysics();
        public void New_VPhysics() { VMPhysics_BoneSelect = 1; }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool VMPhysics_BoneAll { get => Get<bool>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public int VMPhysics_BoneSelect { get => Get<int>(); set { if (value > -1) Set(value); } }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool VMPhysics_ShowLeftOnly { get => Get<bool>(); set => Set(value); }
        public bool VMphysics_BindLR { get => Get<bool>(); set => Set(value); }



        public string VMPhysics_MirrorFilter { get => Get<string>(); set => Set(value); }
        public string VMPhysics_MirrorPair { get => Get<string>(); set => Set(value); }



        [System.Text.Json.Serialization.JsonIgnore]
        public string VMPhysics_Name { get => Get<string>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public string VMPhysics_Side { get => Get<string>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public int VMPhysics_Number { get => Get<int>(); set => Set(value); }


        [System.Text.Json.Serialization.JsonIgnore]
        public string VM_V_collective { get => Get<string>(); set => Set(value); }
    }
}
namespace SkyrimAnimationChecker
{
    /// <summary>
    /// Interaction logic for CBPC_Physics.xaml
    /// </summary>
    public partial class CBPC_Physics : UserControl
    {
        public CBPC_Physics(VM vmm, Icbpc_data o, bool? leftonly = null, bool? bindlr = null, int? collective = 0)
        {
            InitializeComponent();
            vm = vmm.Vphysics;
            vm.PropertyChanged += (sender, e) => { if (e.PropertyName == "VMPhysics_ShowLeftOnly") LeftOnlyUpdated?.Invoke(vm.VMPhysics_ShowLeftOnly); };
            DataContext = vm;
            Data = o;
            if (CheckMirrorFilter(vm.VMPhysics_MirrorFilter?.Split(',')))
            {
                if (Data is Icbpc_data_mirrored m) vm.VMPhysics_MirrorFilter = string.Join(',', m.MirrorKeys);
            }
            if (CheckMirrorPair(vm.VMPhysics_MirrorPair, out MirrorPair[]? p))
            {
                if (Data is Icbpc_data_mirrored m)
                    vm.VMPhysics_MirrorPair = string.Join('|', m.MirrorPairs.ForEach(x => x.ToString()));
            }
            if (leftonly != null) vm.VMPhysics_ShowLeftOnly = (bool)leftonly;
            if (CollectiveCB.SelectedIndex == -1) CollectiveCB.SelectedIndex = collective ?? 0;
            if (bindlr != null) vm.VMphysics_BindLR = (bool)bindlr;
            if (!vm.VMphysics_BindLR) SetIsMirrored();
            Make();
        }
        VM_VPhysics vm;

        #region Properties
        public Icbpc_data Data
        {
            get => (Icbpc_data)GetValue(PhysicsProperty);
            set => SetValue(PhysicsProperty, value);
        }
        public static readonly DependencyProperty PhysicsProperty
            = DependencyProperty.Register(
                  "Data", typeof(Icbpc_data), typeof(CBPC_Physics),
                  new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not CBPC_Physics) return;
                      ((CBPC_Physics)d).Make();
                  }))
              );
        #endregion
        #region Events
        public delegate void LeftOnlyUpdateEventHandler(bool value);
        public event LeftOnlyUpdateEventHandler? LeftOnlyUpdated;
        public delegate void BindLRUpdateEventHandler(bool value);
        public event BindLRUpdateEventHandler? BindLRUpdated;
        public delegate void CollectiveUpdateEventHandler(int value);
        public event CollectiveUpdateEventHandler? CollectiveUpdated;
        #endregion

        #region Mirror
        private void MirrorFilter_TextBox_TextChanged(object sender, TextChangedEventArgs e) => SetMirrorFilter();
        private void SetMirrorFilter()
        {
            if (Data is Icbpc_data_mirrored m)
            {
                if (CheckMirrorFilter(vm.VMPhysics_MirrorFilter.Split(','))) vm.VMPhysics_MirrorFilter = string.Join(',', m.DefaultMirrorKeys);
                m.MirrorKeys = GetMirrorFilter(vm.VMPhysics_MirrorFilter.Split(','));
            }
        }
        private string[] GetMirrorFilter(string[] filter)
        {
            if (Data is Icbpc_data_mirrored m) return CheckMirrorFilter(filter) ? m.DefaultMirrorKeys : filter;
            else return filter;
        }
        private bool CheckMirrorFilter(string[]? filter) => filter?.Length < 1 || filter?.Length == 1 && filter[0] == "default";


        private void MirrorPair_TextBox_TextChanged(object sender, TextChangedEventArgs e) => SetMirrorPair();
        private void SetMirrorPair()
        {
            if (Data is Icbpc_data_mirrored m)
            {
                if (CheckMirrorPair(vm.VMPhysics_MirrorPair, out MirrorPair[]? p)) vm.VMPhysics_MirrorPair = string.Join('|', m.DefaultMirrorPairs.ForEach(x => x.ToString()));
                m.MirrorPairs = GetMirrorPair(vm.VMPhysics_MirrorPair);
            }
        }
        private MirrorPair[] GetMirrorPair(string? sPair)
        {
            if (Data is Icbpc_data_mirrored m)
                return CheckMirrorPair(sPair, out MirrorPair[]? pairs) || pairs == null ? m.DefaultMirrorPairs : pairs;
            else return Array.Empty<MirrorPair>();
        }
        private bool CheckMirrorPair(string? s, out MirrorPair[]? pairs)
        {
            if (s == null) { pairs = null; return true; }
            string[] sPairs = s.Split('|');
            List<MirrorPair> result = new();
            foreach (string sPair in sPairs)
            {
                if (MirrorPair.TryParse(sPair, out MirrorPair? p) || p == null) { pairs = null; return true; }
                result.Add(p);
            }
            pairs = result.ToArray();
            return false;
        }
        #endregion

        private void AddColumn(string key, object[] data, object[]? options = null)
        {
            ColumnDefinition cd = new();
            if (data is string[]) { cd.MinWidth = 120; cd.MaxWidth = 200; cd.Width = new GridLength(1.5, GridUnitType.Star); }
            panel.ColumnDefinitions.Add(cd);

            CBPC_Physics_Column c = new(vm, (string)CollectiveCB.SelectedItem);
            c.Header = key;
            c.Data = data;
            if (options?.Length > 0) c.Option = options;
            if (data is string[]) c.Copy += (way) => CopyValues(way);
            c.SetValue(Grid.ColumnProperty, panel.ColumnDefinitions.Count - 1);
            panel.Children.Add(c);
        }
        private void CopyValues(string way)
        {
            if (MessageBox.Show($"This operation can not be reverted{Environment.NewLine}Proceed?", "Warning", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;
            if (way == "0->1")
            {
                Data.Values.ForEach(o =>
                {
                    if (o is cbpc_data_mirrored m)
                    {
                        m.Left.Values.ForEach(x => x.Value1 = x.Value0);
                        m.Right.Values.ForEach(x => x.Value1 = x.Value0);
                    }
                    else if (o is cbpc_data d)
                    {
                        d.Data.Values.ForEach(x => x.Value1 = x.Value0);
                    }
                });
            }
            else if (way == "1->0")
            {
                Data.Values.ForEach(o =>
                {
                    if (o is cbpc_data_mirrored m)
                    {
                        m.Left.Values.ForEach(x => x.Value0 = x.Value1);
                        m.Right.Values.ForEach(x => x.Value0 = x.Value1);
                    }
                    else if (o is cbpc_data d)
                    {
                        d.Data.Values.ForEach(x => x.Value0 = x.Value1);
                    }
                });
            }
        }

        private void Make()
        {
            if (Data.DataType == "single") Make_single();
            else if (Data.DataType == "belly") Make_single();
            else if (Data.DataType == "mirrored") Make_Mirrored();
            else if (Data.DataType == "bbp") Make_Mirrored();
            else if ( Data.DataType == "buut") Make_Mirrored();
        }
        private void Make_single()
        {
            cbpc_data d = (cbpc_data)Data;
            vm.VMPhysics_Name = d.NameShort;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();

            AddColumn(string.Empty, d.UsingKeys);
            //AddColumn(string.Empty, d.Data.Keys, d.Data.Values);

            AddColumn("", d.Data.Values, d.UsingKeys);
        }
        private void Make_Mirrored()
        {
            cbpc_data_mirrored d = (cbpc_data_mirrored)Data;
            vm.VMPhysics_Name = d.NameShort;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();

            AddColumn(string.Empty, d.UsingKeys);
            //AddColumn(string.Empty, d.Left.Keys, d.Left.Values);

            AddColumn("L", d.Left.Values, d.UsingKeys);
            if (!vm.VMPhysics_ShowLeftOnly) AddColumn("R", d.Right.Values, d.UsingKeys);
        }

        private void When_Click_toMake(object sender, RoutedEventArgs e) => Make();


        private void Collective_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectiveUpdated?.Invoke((sender as ComboBox)?.SelectedIndex ?? 0);
            foreach (var child in panel.Children)
            {
                if (child is CBPC_Physics_Column c)
                {
                    c.Collective = (string)((sender as ComboBox)?.SelectedItem ?? "all");
                }
            }
        }
        private void CollectiveCB_MouseEnter(object sender, MouseEventArgs e) => (sender as ComboBox)?.Focus();


        private void SetIsMirrored() { if (Data is cbpc_data_mirrored m) m.IsMirrored = vm.VMphysics_BindLR; }
        private void BindLR_CheckBox_Changed(object sender, RoutedEventArgs e) { SetIsMirrored(); BindLRUpdated?.Invoke(vm.VMphysics_BindLR); }


    }
}
