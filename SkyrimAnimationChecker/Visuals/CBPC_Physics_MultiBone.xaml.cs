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
    public class VM_Vmultibone : Notify.NotifyPropertyChanged, IVM_Visual
    {
        public VM_Vmultibone() => Default_Vmultibone();
        protected void Default_Vmultibone() { VMbreast_BoneAll = true; VMbreast_BoneSelect = 1; VMbreast_ShowLeftOnly = false; VMmultibone_MirrorFilter = string.Empty; VMmultibone_MirrorPair = string.Empty; }
        public void Reset() => Default_Vmultibone();
        public void New_Vmultibone() { VMbreast_BoneSelect = 1; }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool VMbreast_BoneAll { get => Get<bool>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public int VMbreast_BoneSelect { get => Get<int>(); set { if (value > -1) Set(value); } }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool VMbreast_ShowLeftOnly { get => Get<bool>(); set => Set(value); }



        public string VMmultibone_MirrorFilter { get => Get<string>(); set => Set(value); }
        public string VMmultibone_MirrorPair { get => Get<string>(); set => Set(value); }



        [System.Text.Json.Serialization.JsonIgnore]
        public string VMbreast_Name { get => Get<string>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public string VMbreast_Side { get => Get<string>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public int VMbreast_Number { get => Get<int>(); set => Set(value); }


        [System.Text.Json.Serialization.JsonIgnore]
        public string VM_V_collective { get => Get<string>(); set => Set(value); }
    }
}
namespace SkyrimAnimationChecker
{
    /// <summary>
    /// Interaction logic for CBPC_3BA.xaml
    /// </summary>
    public partial class CBPC_Physics_MultiBone : UserControl
    {
        public CBPC_Physics_MultiBone(VM vmm, Icbpc_data_multibone o, bool? leftonly = null, int? collective = 0)
        {
            InitializeComponent();
            vm = vmm.Vmultibone;
            vm.PropertyChanged += (sender, e) => { if (e.PropertyName == "VMbreast_ShowLeftOnly") LeftOnlyUpdated?.Invoke(vm.VMbreast_ShowLeftOnly); };
            DataContext = vm;
            Data = o;
            if (CheckMirrorFilter(vm.VMmultibone_MirrorFilter?.Split(','))) vm.VMmultibone_MirrorFilter = string.Join(',', Data.MirrorKeys);
            if (CheckMirrorPair(vm.VMmultibone_MirrorPair, out MirrorPair[]? p)) vm.VMmultibone_MirrorPair = string.Join('|', Data.MirrorPairs.ForEach(x => x.ToString()));
            if (leftonly != null) vm.VMbreast_ShowLeftOnly = (bool)leftonly;
            if (CollectiveCB.SelectedIndex == -1) CollectiveCB.SelectedIndex = collective ?? 0;
            Make();
        }
        VM_Vmultibone vm;

        #region Properties
        public Icbpc_data_multibone Data
        {
            get => (Icbpc_data_multibone)GetValue(PhysicsProperty);
            set => SetValue(PhysicsProperty, value);
        }
        public static readonly DependencyProperty PhysicsProperty
            = DependencyProperty.Register(
                  "Data", typeof(Icbpc_data_multibone), typeof(CBPC_Physics_MultiBone),
                  new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not CBPC_Physics_MultiBone) return;
                      ((CBPC_Physics_MultiBone)d).Make();
                  }))
              );
        #endregion
        #region Events
        public delegate void LeftOnlyUpdateEventHandler(bool value);
        public event LeftOnlyUpdateEventHandler? LeftOnlyUpdated;
        public delegate void CollectiveUpdateEventHandler(int value);
        public event CollectiveUpdateEventHandler? CollectiveUpdated;
        #endregion

        #region Mirror
        //private string[] MirrorFilter
        //{
        //    get => GetMirrorFilter(vm.VMbreast_MirrorFilter.Split(','));
        //    set { Data.Mirrorable = value; vm.VMbreast_MirrorFilter = string.Join(',', value); }
        //}
        private void MirrorFilter_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Enter || e.Key == Key.Return)
                Data.MirrorKeys = GetMirrorFilter(vm.VMmultibone_MirrorFilter.Split(','));
        }
        private string[] GetMirrorFilter(string[] filter) => CheckMirrorFilter(filter) ? Data.MirrorKeys : filter;
        private bool CheckMirrorFilter(string[]? filter) => filter?.All(x => string.IsNullOrWhiteSpace(x)) ?? true;

        private void MirrorPair_Textbox_KeyDown(object sender, KeyEventArgs e)
        {
            Data.MirrorPairs = GetMirrorPair(vm.VMmultibone_MirrorPair);
        }
        private MirrorPair[] GetMirrorPair(string? sPair) => CheckMirrorPair(sPair, out MirrorPair[]? pairs) || pairs == null ? Data.MirrorPairs : pairs;
        private bool CheckMirrorPair(string? s, out MirrorPair[]? pairs)
        {
            if (s == null) { pairs = null; return true; }
            string[] sPairs = s.Split('|');
            List<MirrorPair> result = new();
            foreach(string sPair in sPairs)
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
            c.SetValue(Grid.ColumnProperty, panel.ColumnDefinitions.Count - 1);
            panel.Children.Add(c);
        }


        private string lasttype = string.Empty;
        private void Make()
        {
            if (Data.DataType == "3ba") Make_3ba_new();
            else if (Data.DataType == "bbp") Make_bbp();
            else if (Data.DataType == "leg") Make_leg();
            else if (Data.DataType == "vagina") Make_vagina();
            lasttype = Data.DataType;
        }
        private void Make_bbp()
        {
            cbpc_breast d = (cbpc_breast)Data.GetData(0);
            vm.VMbreast_Name = d.NameShort;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();
            allboneCB.IsEnabled = false;
            vm.VMbreast_BoneAll = false;
            lonlyCB.IsEnabled = true;

            AddColumn(string.Empty, actualKeys(d.Left.Values), d.Left.Values);

            AddColumn("L", d.Left.Values);
            if (!vm.VMbreast_ShowLeftOnly) AddColumn("R", d.Right.Values);

        }
        private bool lastallbone = false;
        private void Make_3ba_new()
        {
            cbpc_breast[] bs = new cbpc_breast[3];
            for (int i = 0; i < bs.Length; i++) { bs[i] = (cbpc_breast)Data.GetData(i + 1); }
            vm.VMbreast_Name = bs[0].NameShort;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();
            allboneCB.IsEnabled = true;
            if (lasttype != "3ba") vm.VMbreast_BoneAll = true;
            lonlyCB.IsEnabled = true;

            if (vm.VMbreast_BoneAll)
            {
                if (lastallbone != vm.VMbreast_BoneAll) vm.VMbreast_ShowLeftOnly = true;
                AddColumn(string.Empty, Data.UsingKeys);
                Data.Keys.ForEach(key =>
                {
                    var br = Data.PropertyHandleGetValue<cbpc_breast>(key);
                    AddColumn($"{br.Number}{br.Left}", br.Left.Values, Data.UsingKeys);
                });
                if (!vm.VMbreast_ShowLeftOnly) Data.Keys.ForEach(key =>
                {
                    var br = Data.PropertyHandleGetValue<cbpc_breast>(key);
                    AddColumn($"{br.Number}{br.Right}", br.Right.Values, Data.UsingKeys);
                });
            }
            else
            {
                if (lastallbone != vm.VMbreast_BoneAll) vm.VMbreast_ShowLeftOnly = false;
                AddColumn(string.Empty, Data.UsingKeys);

                AddColumn($"{vm.VMbreast_BoneSelect}L", bs[vm.VMbreast_BoneSelect].Left.Values, Data.UsingKeys);
                if (!vm.VMbreast_ShowLeftOnly) AddColumn($"{vm.VMbreast_BoneSelect}R", bs[vm.VMbreast_BoneSelect].Right.Values, Data.UsingKeys);
            }
            lastallbone = vm.VMbreast_BoneAll;
        }
        private void Make_3ba()
        {
            cbpc_breast[] bs = new cbpc_breast[3];
            for (int i = 0; i < bs.Length; i++) { bs[i] = (cbpc_breast)Data.GetData(i + 1); }
            vm.VMbreast_Name = bs[0].NameShort;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();
            allboneCB.IsEnabled = true;
            if (lasttype != "3ba") vm.VMbreast_BoneAll = true;
            lonlyCB.IsEnabled = true;

            if (vm.VMbreast_BoneAll)
            {
                if (lastallbone != vm.VMbreast_BoneAll) vm.VMbreast_ShowLeftOnly = true;
                AddColumn(string.Empty, Data.UsingKeys, bs[0].Left.Values);
                Data.Keys.ForEach(key =>
                {
                    var br = Data.PropertyHandleGetValue<cbpc_breast>(key);
                    AddColumn($"{br.Number}{br.Left}", br.Left.Values);
                });
                if (!vm.VMbreast_ShowLeftOnly) Data.Keys.ForEach(key =>
                {
                    var br = Data.PropertyHandleGetValue<cbpc_breast>(key);
                    AddColumn($"{br.Number}{br.Right}", br.Right.Values);
                });
            }
            else
            {
                if (lastallbone != vm.VMbreast_BoneAll) vm.VMbreast_ShowLeftOnly = false;
                AddColumn(string.Empty, actualKeys(bs[0].Left.Values), bs[0].Left.Values);
                switch (vm.VMbreast_BoneSelect)
                {
                    case 1:
                        AddColumn("1L", bs[0].Left.Values);
                        if (!vm.VMbreast_ShowLeftOnly) AddColumn("1R", bs[0].Right.Values);
                        break;
                    case 2:
                        AddColumn("2L", bs[1].Left.Values);
                        if (!vm.VMbreast_ShowLeftOnly) AddColumn("2R", bs[1].Right.Values);
                        break;
                    case 3:
                        AddColumn("3L", bs[2].Left.Values);
                        if (!vm.VMbreast_ShowLeftOnly) AddColumn("3R", bs[2].Right.Values);
                        break;
                }
            }
            lastallbone = vm.VMbreast_BoneAll;
        }
        private void Make_leg()
        {
            cbpc_leg d = (cbpc_leg)Data;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();
            allboneCB.IsEnabled = false;
            vm.VMbreast_BoneAll = false;
            lonlyCB.IsEnabled = true;
            if (lasttype != "leg") vm.VMbreast_ShowLeftOnly = false;

            AddColumn(string.Empty, d.UsingKeys);
            switch (vm.VMbreast_BoneSelect)
            {
                case 1:
                    vm.VMbreast_Name = d.FrontThigh.Name;
                    //AddColumn(string.Empty, actualKeys(d.FrontThigh.Left.Values), d.FrontThigh.Left.Values);
                    AddColumn(d.FrontThigh.Left.Name, d.FrontThigh.Left.Values, d.UsingKeys);
                    if (!vm.VMbreast_ShowLeftOnly) AddColumn(d.FrontThigh.Right.Name, d.FrontThigh.Right.Values, d.UsingKeys);
                    break;
                case 2:
                    vm.VMbreast_Name = d.RearThigh.Name;
                    //AddColumn(string.Empty, actualKeys(d.RearThigh.Left.Values), d.RearThigh.Left.Values);
                    AddColumn(d.RearThigh.Left.Name, d.RearThigh.Left.Values, d.UsingKeys);
                    if (!vm.VMbreast_ShowLeftOnly) AddColumn(d.RearThigh.Right.Name, d.RearThigh.Right.Values, d.UsingKeys);
                    break;
                case 3:
                    vm.VMbreast_Name = d.RearCalf.Name;
                    //AddColumn(string.Empty, actualKeys(d.RearCalf.Left.Values), d.RearCalf.Left.Values);
                    AddColumn(d.RearCalf.Left.Name, d.RearCalf.Left.Values, d.UsingKeys);
                    if (!vm.VMbreast_ShowLeftOnly) AddColumn(d.RearCalf.Right.Name, d.RearCalf.Right.Values, d.UsingKeys);
                    break;
            }

        }
        private void Make_vagina()
        {
            cbpc_vagina d = (cbpc_vagina)Data;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();
            allboneCB.IsEnabled = false;
            vm.VMbreast_BoneAll = false;
            if (lasttype != "vagina") vm.VMbreast_ShowLeftOnly = false;

            AddColumn(string.Empty, d.UsingKeys);
            switch (vm.VMbreast_BoneSelect)
            {
                case 1:
                    vm.VMbreast_Name = d.Vagina.Name;
                    lonlyCB.IsEnabled = false;
                    //AddColumn(string.Empty, actualKeys(d.Vagina.Data.Values), d.Vagina.Data.Values);
                    AddColumn(d.Vagina.Data.Name, d.Vagina.Data.Values, d.UsingKeys);
                    break;
                case 2:
                    vm.VMbreast_Name = d.Clit.Name;
                    lonlyCB.IsEnabled = false;
                    //AddColumn(string.Empty, actualKeys(d.Clit.Data.Values), d.Clit.Data.Values);
                    AddColumn(d.Clit.Data.Name, d.Clit.Data.Values, d.UsingKeys);
                    break;
                case 3:
                    vm.VMbreast_Name = d.Labia.Name;
                    lonlyCB.IsEnabled = true;
                    //AddColumn(string.Empty, actualKeys(d.Labia.Left.Values), d.Labia.Left.Values);
                    AddColumn(d.Labia.Left.Name, d.Labia.Left.Values, d.UsingKeys);
                    if (!vm.VMbreast_ShowLeftOnly) AddColumn(d.Labia.Right.Name, d.Labia.Right.Values, d.UsingKeys);
                    break;
            }

        }
        private string[] actualKeys(physics_object[] vals) => vals.ForEach(x => x.Key);

        private void _3BA_RadioButton_Click(object sender, RoutedEventArgs e) => Make();

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


    }
}
