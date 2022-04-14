﻿using System;
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
    public class VM_Vbreast : Notify.NotifyPropertyChanged
    {
        public VM_Vbreast() => Default_Vbreast();
        protected void Default_Vbreast() { VMbreast_BoneAll = true; VMbreast_BoneSelect = 1; VMbreast_ShowLeftOnly = false; }
        public void New_Vbreast() { VMbreast_BoneSelect = 1; }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool VMbreast_BoneAll { get => Get<bool>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public int VMbreast_BoneSelect { get => Get<int>(); set { if (value > -1) Set(value); } }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool VMbreast_ShowLeftOnly { get => Get<bool>(); set => Set(value); }



        public string VMbreast_MirrorFilter { get => Get<string>(); set => Set(value); }



        [System.Text.Json.Serialization.JsonIgnore]
        public string VMbreast_Name { get => Get<string>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public string VMbreast_Side { get => Get<string>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public int VMbreast_Number { get => Get<int>(); set => Set(value); }

    }
}
namespace SkyrimAnimationChecker
{
    /// <summary>
    /// Interaction logic for CBPC_3BA.xaml
    /// </summary>
    public partial class CBPC_Physics_MultiBone : UserControl
    {
        public CBPC_Physics_MultiBone(VM vmm, Icbpc_data_multibone o)
        {
            InitializeComponent();
            vm = vmm.Vbreast;
            DataContext = vm;
            Data = o;
            if (CheckMirrorFilter(vm.VMbreast_MirrorFilter?.Split(','))) vm.VMbreast_MirrorFilter = string.Join(',', o.MirrorKeys);
            Make();
        }
        VM_Vbreast vm;

        #region Properties
        public Icbpc_data_multibone Data
        {
            get => (Icbpc_data_multibone)GetValue(PhysicsProperty);
            set => SetValue(PhysicsProperty, value);
        }
        public static readonly DependencyProperty PhysicsProperty
            = DependencyProperty.Register(
                  "Data", typeof(Icbpc_data_multibone), typeof(CBPC_Physics_MultiBone),
                  new PropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not CBPC_Physics_MultiBone) return;
                      ((CBPC_Physics_MultiBone)d).Make();
                  }))
              );
        #endregion
        #region Events
        //public delegate void DataUpdateEventHandler(physics_object o);
        //public event DataUpdateEventHandler? DataUpdated;
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
                Data.MirrorKeys = GetMirrorFilter(vm.VMbreast_MirrorFilter.Split(','));
        }
        private string[] GetMirrorFilter(string[] filter) => CheckMirrorFilter(filter) ? Data.MirrorKeys : filter;
        private bool CheckMirrorFilter(string[]? filter) => filter?.All(x => string.IsNullOrWhiteSpace(x)) ?? true;
        #endregion

        private void AddColumn(string key, object[] data, object[]? options = null)
        {
            ColumnDefinition cd = new();
            if (data is string[]) { cd.MinWidth = 120; cd.MaxWidth = 200; cd.Width = new GridLength(1.5, GridUnitType.Star); }
            panel.ColumnDefinitions.Add(cd);

            CBPC_Physics_Column c = new();
            c.Header = key;
            c.Data = data;
            if (options?.Length > 0) c.Option = options;
            c.SetValue(Grid.ColumnProperty, panel.ColumnDefinitions.Count - 1);
            panel.Children.Add(c);
        }
        

        private void Make()
        {
            if (Data.DataType == "3ba") Make_3ba();
            else if (Data.DataType == "bbp") Make_bbp();
            else if (Data.DataType == "leg") Make_leg();
            else if (Data.DataType == "vagina") Make_vagina();
        }
        private void Make_bbp()
        {
            cbpc_breast d = (cbpc_breast)Data.GetData(0);
            vm.VMbreast_Name = d.NameShort;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();
            allboneCB.IsEnabled = false;
            vm.VMbreast_BoneAll = false;

            AddColumn(string.Empty, d.Left.Keys, d.Left.Values);

            AddColumn("L", d.Left.Values);
            if (!vm.VMbreast_ShowLeftOnly) AddColumn("R", d.Right.Values);

        }
        private void Make_3ba()
        {
            cbpc_breast[] bs = new cbpc_breast[3];
            for (int i = 0; i < bs.Length; i++) { bs[i] = (cbpc_breast)Data.GetData(i + 1); }
            vm.VMbreast_Name = bs[0].NameShort;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();
            allboneCB.IsEnabled = true;

            if (vm.VMbreast_BoneAll)
            {
                vm.VMbreast_ShowLeftOnly = true;
                AddColumn(string.Empty, bs[0].Left.Keys, bs[0].Left.Values);
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
                AddColumn(string.Empty, bs[0].Left.Keys, bs[0].Left.Values);
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
        }
        private void Make_leg()
        {
            cbpc_leg d = (cbpc_leg)Data;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();
            allboneCB.IsEnabled = false;
            vm.VMbreast_BoneAll = false;

            switch (vm.VMbreast_BoneSelect)
            {
                case 1:
                    vm.VMbreast_Name = d.FrontThigh.Name;
                    lonlyCB.IsEnabled = true;
                    AddColumn(string.Empty, d.FrontThigh.Left.Keys, d.FrontThigh.Left.Values);
                    AddColumn(d.FrontThigh.Left.Name, d.FrontThigh.Left.Values);
                    if (!vm.VMbreast_ShowLeftOnly) AddColumn(d.FrontThigh.Right.Name, d.FrontThigh.Right.Values);
                    break;
                case 2:
                    vm.VMbreast_Name = d.RearThigh.Name;
                    lonlyCB.IsEnabled = true;
                    AddColumn(string.Empty, d.RearThigh.Left.Keys, d.RearThigh.Left.Values);
                    AddColumn(d.RearThigh.Left.Name, d.RearThigh.Left.Values);
                    if (!vm.VMbreast_ShowLeftOnly) AddColumn(d.RearThigh.Right.Name, d.RearThigh.Right.Values);
                    break;
                case 3:
                    vm.VMbreast_Name = d.RearCalf.Name;
                    lonlyCB.IsEnabled = true;
                    AddColumn(string.Empty, d.RearCalf.Left.Keys, d.RearCalf.Left.Values);
                    AddColumn(d.RearCalf.Left.Name, d.RearCalf.Left.Values);
                    if (!vm.VMbreast_ShowLeftOnly) AddColumn(d.RearCalf.Right.Name, d.RearCalf.Right.Values);
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

            switch (vm.VMbreast_BoneSelect)
            {
                case 1:
                    vm.VMbreast_Name = d.Vagina.Name;
                    lonlyCB.IsEnabled = false;
                    AddColumn(string.Empty, d.Vagina.Data.Keys, d.Vagina.Data.Values);
                    AddColumn(d.Vagina.Data.Name, d.Vagina.Data.Values);
                    break;
                case 2:
                    vm.VMbreast_Name = d.Clit.Name;
                    lonlyCB.IsEnabled = false;
                    AddColumn(string.Empty, d.Clit.Data.Keys, d.Clit.Data.Values);
                    AddColumn(d.Clit.Data.Name, d.Clit.Data.Values);
                    break;
                case 3:
                    vm.VMbreast_Name = d.Labia.Name;
                    lonlyCB.IsEnabled = true;
                    AddColumn(string.Empty, d.Labia.Left.Keys, d.Labia.Left.Values);
                    AddColumn(d.Labia.Left.Name, d.Labia.Left.Values);
                    if (!vm.VMbreast_ShowLeftOnly) AddColumn(d.Labia.Right.Name, d.Labia.Right.Values);
                    break;
            }

        }

        private void _3BA_RadioButton_Click(object sender, RoutedEventArgs e) => Make();

    }
}