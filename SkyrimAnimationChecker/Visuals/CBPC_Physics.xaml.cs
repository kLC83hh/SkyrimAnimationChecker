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
    public class VM_VPhysics : Notify.NotifyPropertyChanged
    {
        public VM_VPhysics() => Default_VPhysics();
        protected void Default_VPhysics() { VMPhysics_BoneAll = true; VMPhysics_BoneSelect = 1; VMPhysics_ShowLeftOnly = false; }
        public void New_VPhysics() { VMPhysics_BoneSelect = 1; }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool VMPhysics_BoneAll { get => Get<bool>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public int VMPhysics_BoneSelect { get => Get<int>(); set { if (value > -1) Set(value); } }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool VMPhysics_ShowLeftOnly { get => Get<bool>(); set => Set(value); }



        public string VMPhysics_MirrorFilter { get => Get<string>(); set => Set(value); }



        [System.Text.Json.Serialization.JsonIgnore]
        public string VMPhysics_Name { get => Get<string>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public string VMPhysics_Side { get => Get<string>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public int VMPhysics_Number { get => Get<int>(); set => Set(value); }

    }
}
namespace SkyrimAnimationChecker
{
    /// <summary>
    /// Interaction logic for CBPC_Physics.xaml
    /// </summary>
    public partial class CBPC_Physics : UserControl
    {
        public CBPC_Physics(VM vmm, Icbpc_data o, bool? leftonly = null)
        {
            InitializeComponent();
            vm = vmm.Vphysics;
            vm.PropertyChanged += (sender, e) => { if (e.PropertyName == "VMPhysics_ShowLeftOnly") LeftOnlyUpdated?.Invoke(vm.VMPhysics_ShowLeftOnly); };
            DataContext = vm;
            Data = o;
            if (CheckMirrorFilter(vm.VMPhysics_MirrorFilter?.Split(',')))
            {
                if (o is Icbpc_data_mirrored m) vm.VMPhysics_MirrorFilter = string.Join(',', m.MirrorKeys);
            }
            if (leftonly != null) vm.VMPhysics_ShowLeftOnly = (bool)leftonly;
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
        #endregion

        #region Mirror
        private void MirrorFilter_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Enter || e.Key == Key.Return)
            if (Data is Icbpc_data_mirrored m) m.MirrorKeys = GetMirrorFilter(vm.VMPhysics_MirrorFilter.Split(','));
        }
        private string[] GetMirrorFilter(string[] filter)
        {
            if (Data is Icbpc_data_mirrored m) return CheckMirrorFilter(filter) ? m.MirrorKeys : filter;
            else return filter;
        }
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

            AddColumn(string.Empty, d.Data.Keys, d.Data.Values);

            AddColumn("", d.Data.Values);
        }
        private void Make_Mirrored()
        {
            cbpc_data_mirrored d = (cbpc_data_mirrored)Data;
            vm.VMPhysics_Name = d.NameShort;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();

            AddColumn(string.Empty, d.Left.Keys, d.Left.Values);

            AddColumn("L", d.Left.Values);
            if (!vm.VMPhysics_ShowLeftOnly) AddColumn("R", d.Right.Values);
        }

        private void When_Click_toMake(object sender, RoutedEventArgs e) => Make();
    }
}
