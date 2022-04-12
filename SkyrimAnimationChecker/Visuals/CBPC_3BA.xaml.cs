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
    public class VM_V3BA : Notify.NotifyPropertyChanged
    {
        public VM_V3BA() => Default_V3BA();
        protected void Default_V3BA() { VM3BA_BoneAll = true; VM3BA_BoneSelect = 1; VM3BA_ShowLeftOnly = true; }
        public void New_V3BA() { VM3BA_BoneSelect = 1; }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool VM3BA_BoneAll { get => Get<bool>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public int VM3BA_BoneSelect { get => Get<int>(); set { if (value > -1) Set(value); } }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool VM3BA_ShowLeftOnly { get => Get<bool>(); set => Set(value); }



        public string VM3BA_MirrorFilter { get => Get<string>(); set => Set(value); }



        [System.Text.Json.Serialization.JsonIgnore]
        public string VM3BA_Name { get => Get<string>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public string VM3BA_Side { get => Get<string>(); set => Set(value); }
        [System.Text.Json.Serialization.JsonIgnore]
        public int VM3BA_Number { get => Get<int>(); set => Set(value); }

    }
}
namespace SkyrimAnimationChecker
{
    /// <summary>
    /// Interaction logic for CBPC_3BA.xaml
    /// </summary>
    public partial class CBPC_3BA : UserControl
    {
        public CBPC_3BA(VM vmm, breast_3ba_object o)
        {
            InitializeComponent();
            vm = vmm.VM_V3BA;
            DataContext = vm;
            Data = o;
            if (CheckMirrorFilter(vm.VM3BA_MirrorFilter?.Split(','))) vm.VM3BA_MirrorFilter = string.Join(',', o.Mirrorable);
            Make();
        }
        VM_V3BA vm;


        #region Properties
        public breast_3ba_object Data
        {
            get => (breast_3ba_object)GetValue(PhysicsProperty);
            set => SetValue(PhysicsProperty, value);
        }
        public static readonly DependencyProperty PhysicsProperty
            = DependencyProperty.Register(
                  "Data", typeof(breast_3ba_object), typeof(CBPC_3BA),
                  new PropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not CBPC_3BA) return;
                      ((CBPC_3BA)d).Make();
                  }))
              );
        #endregion
        #region Events
        public delegate void DataUpdateEventHandler(physics_object o);
        public event DataUpdateEventHandler? DataUpdated;
        #endregion


        //private string[] MirrorFilter
        //{
        //    get => GetMirrorFilter(vm.VM3BA_MirrorFilter.Split(','));
        //    set { Data.Mirrorable = value; vm.VM3BA_MirrorFilter = string.Join(',', value); }
        //}
        private void MirrorFilter_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Enter || e.Key == Key.Return)
                Data.Mirrorable = GetMirrorFilter(vm.VM3BA_MirrorFilter.Split(','));
        }
        private string[] GetMirrorFilter(string[] filter) => CheckMirrorFilter(filter) ? Data.Mirrorable : filter;
        private bool CheckMirrorFilter(string[]? filter) => filter?.All(x => string.IsNullOrWhiteSpace(x)) ?? true;

        private void AddColumn(string key, object[] data)
        {
            ColumnDefinition cd = new();
            if (data is string[]) { cd.MinWidth = 120; cd.MaxWidth = 200; }
            panel.ColumnDefinitions.Add(cd);

            CBPC_Physics_Column c = new();
            c.Header = key;
            c.Data = data;
            //c.DataUpdated += Column_DataUpdated;
            c.SetValue(Grid.ColumnProperty, panel.ColumnDefinitions.Count - 1);
            panel.Children.Add(c);
        }
        //private void Column_DataUpdated(physics_object o) => Data.SetTo(o.Name, o.Key, o.Values, true);

        private void Make()
        {
            vm.VM3BA_Name = Data.L1.NameShort;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();
            if (vm.VM3BA_BoneAll)
            {
                AddColumn(string.Empty, Data.L1.Data.Keys);
                Data.Keys.ForEach(key =>
                {
                    var br = Data.GetPropertyHandleValue<CBPC_Breast>(key);
                    if (br.Side != CBPC_Breast.Sides.R || !vm.VM3BA_ShowLeftOnly) AddColumn($"{br.Number}{br.Side}", br.Data.Values);
                });
            }
            else
            {
                AddColumn(string.Empty, Data.L1.Data.Keys);
                switch (vm.VM3BA_BoneSelect)
                {
                    case 1:
                        AddColumn("1L", Data.L1.Data.Values);
                        if (!vm.VM3BA_ShowLeftOnly) AddColumn("1R", Data.R1.Data.Values);
                        break;
                    case 2:
                        AddColumn("2L", Data.L2.Data.Values);
                        if (!vm.VM3BA_ShowLeftOnly) AddColumn("2R", Data.R2.Data.Values);
                        break;
                    case 3:
                        AddColumn("3L", Data.L3.Data.Values);
                        if (!vm.VM3BA_ShowLeftOnly) AddColumn("3R", Data.R3.Data.Values);
                        break;
                }
            }
        }

        private void _3BA_RadioButton_Click(object sender, RoutedEventArgs e) => Make();

    }
}
