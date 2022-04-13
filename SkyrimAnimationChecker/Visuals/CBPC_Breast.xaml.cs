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
    public class VM_Vbreast : Notify.NotifyPropertyChanged
    {
        public VM_Vbreast() => Default_Vbreast();
        protected void Default_Vbreast() { VMbreast_BoneAll = true; VMbreast_BoneSelect = 1; VMbreast_ShowLeftOnly = true; }
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
    public partial class CBPC_Breast : UserControl
    {
        public CBPC_Breast(VM vmm, Icbpc_breast_data o)
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
        public Icbpc_breast_data Data
        {
            get => (Icbpc_breast_data)GetValue(PhysicsProperty);
            set => SetValue(PhysicsProperty, value);
        }
        public static readonly DependencyProperty PhysicsProperty
            = DependencyProperty.Register(
                  "Data", typeof(Icbpc_breast_data), typeof(CBPC_Breast),
                  new PropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not CBPC_Breast) return;
                      ((CBPC_Breast)d).Make();
                  }))
              );
        #endregion
        #region Events
        //public delegate void DataUpdateEventHandler(physics_object o);
        //public event DataUpdateEventHandler? DataUpdated;
        #endregion


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

        private void AddColumn(string key, object[] data, object[]? options = null)
        {
            ColumnDefinition cd = new();
            if (data is string[]) { cd.MinWidth = 120; cd.MaxWidth = 200; cd.Width = new GridLength(1.5, GridUnitType.Star); }
            panel.ColumnDefinitions.Add(cd);

            CBPC_Physics_Column c = new();
            c.Header = key;
            c.Data = data;
            if (options?.Length > 0) c.Option = options;
            //c.DataUpdated += Column_DataUpdated;
            c.SetValue(Grid.ColumnProperty, panel.ColumnDefinitions.Count - 1);
            panel.Children.Add(c);
        }
        //private void Column_DataUpdated(physics_object o) => Data.SetTo(o.Name, o.Key, o.Values, true);

        private void Make()
        {
            if (Data.DataType == "3ba") Make_3ba();
            else if (Data.DataType == "bbp") Make_bbp();
        }
        private void Make_bbp()
        {
            vm.VMbreast_Name = Data.GetData(0).NameShort;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();

            AddColumn(string.Empty, Data.GetData(0).Left.Keys, Data.GetData(0).Left.Values);

            AddColumn("L", Data.GetData(0).Left.Values);
            if (!vm.VMbreast_ShowLeftOnly) AddColumn("R", Data.GetData(0).Right.Values);

        }
        private void Make_3ba()
        {
            vm.VMbreast_Name = Data.GetData(1).NameShort;
            panel.Children.Clear();
            panel.ColumnDefinitions.Clear();
            if (vm.VMbreast_BoneAll)
            {
                AddColumn(string.Empty, Data.GetData(1).Left.Keys, Data.GetData(1).Left.Values);
                Data.Keys.ForEach(key =>
                {
                    var br = Data.GetPropertyHandleValue<cbpc_breast>(key);
                    AddColumn($"{br.Number}{br.Left}", br.Left.Values);
                });
                if (!vm.VMbreast_ShowLeftOnly) Data.Keys.ForEach(key =>
                {
                    var br = Data.GetPropertyHandleValue<cbpc_breast>(key);
                    AddColumn($"{br.Number}{br.Right}", br.Right.Values);
                });
            }
            else
            {
                AddColumn(string.Empty, Data.GetData(1).Left.Keys, Data.GetData(1).Left.Values);
                switch (vm.VMbreast_BoneSelect)
                {
                    case 1:
                        AddColumn("1L", Data.GetData(1).Left.Values);
                        if (!vm.VMbreast_ShowLeftOnly) AddColumn("1R", Data.GetData(1).Right.Values);
                        break;
                    case 2:
                        AddColumn("2L", Data.GetData(2).Left.Values);
                        if (!vm.VMbreast_ShowLeftOnly) AddColumn("2R", Data.GetData(2).Right.Values);
                        break;
                    case 3:
                        AddColumn("3L", Data.GetData(3).Left.Values);
                        if (!vm.VMbreast_ShowLeftOnly) AddColumn("3R", Data.GetData(2).Right.Values);
                        break;
                }
            }
        }

        private void _3BA_RadioButton_Click(object sender, RoutedEventArgs e) => Make();

    }
}
