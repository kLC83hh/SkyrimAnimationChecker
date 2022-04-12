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
    /// Interaction logic for CBPC_Physics_Column.xaml
    /// </summary>
    public partial class CBPC_Physics_Column : UserControl
    {
        public CBPC_Physics_Column() => InitializeComponent();

        #region DependencyProperty
        public double StackHeight
        {
            get => (double)GetValue(StackHeightProperty);
            set => SetValue(StackHeightProperty, value);
        }
        public static readonly DependencyProperty StackHeightProperty
            = DependencyProperty.Register(
                  "StackHeight", typeof(double), typeof(CBPC_Physics_Column),
                  new PropertyMetadata(18.0, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not CBPC_Physics_Column) return;
                      //((CBPC_Physics_Column)d).Make();
                  }))
              );
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public static readonly DependencyProperty HeaderProperty
            = DependencyProperty.Register(
                  "Header", typeof(string), typeof(CBPC_Physics_Column),
                  new PropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not CBPC_Physics_Column) return;
                      //((CBPC_Physics_Column)d).Make();
                  }))
              );
        public object[]? Data
        {
            get => (object[])GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
        public static readonly DependencyProperty DataProperty
            = DependencyProperty.Register(
                  "Data", typeof(object[]), typeof(CBPC_Physics_Column),
                  new PropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not CBPC_Physics_Column) return;
                      ((CBPC_Physics_Column)d).Make();
                  }))
              );
        #endregion

        private void Make()
        {
            if (Data == null) return;
            if (Data is string[]) { H0.Visibility = Visibility.Hidden; H1.Visibility = Visibility.Hidden; HL.Visibility = Visibility.Hidden; HR.Visibility = Visibility.Hidden; }
            panel.Children.Clear();
            foreach (var item in Data)
            {
                if (TryMakeOne(out UIElement? o, item)) continue;
                else panel.Children.Add(o);
            }
        }
        private bool TryMakeOne(out UIElement? o, params object[] d)
        {
            if (d.Length == 1 && d[0] is Common.physics_object) return TryMakeOne(out o, (Common.physics_object)d[0]);
            else if (d.Length == 1 && d[0] is string) return TryMakeOne(out o, (string)d[0]);
            else if (d.Length == 1 && d[0] is double[]) return TryMakeOne(out o, (double[])d[0]);
            
            o = null;
            return true;
        }
        private bool TryMakeOne(out UIElement? o, Common.physics_object d)
        {
            o = new Visuals.PhysicsBox() { Physics = d };
            //if ((o as Visuals.PhysicsBox) != null) (o as Visuals.PhysicsBox).PhysicsUpdated += (o) => DataUpdated?.Invoke(o);
            BindingOperations.SetBinding(o, Control.HeightProperty, new Binding() { Source = StackHeight });
            return false;
        }
        private bool TryMakeOne(out UIElement? o, string d)
        {
            o = new TextBlock() { Text = d };
            BindingOperations.SetBinding(o, Control.HeightProperty, new Binding() { Source = StackHeight });
            return false;
        }
        private bool TryMakeOne(out UIElement? o, double[] d)
        {
            var g = new Grid();
            g.ColumnDefinitions.Clear();
            g.ColumnDefinitions.Add(new() { MinWidth = 30, MaxWidth = 70 });
            g.ColumnDefinitions.Add(new() { MinWidth = 30, MaxWidth = 70 });
            //M.D(d.Length);
            if (d.Length == 2)
            {
                var tb1 = new TextBox() { Text = d[0].ToString() };
                //tb1.SetBinding(TextBox.TextProperty, new Binding() { Source = d[0] });
                g.Children.Add(tb1);
                var tb2 = new TextBox() { Text = d[1].ToString() };
                tb2.SetValue(Grid.ColumnProperty, 1);
                g.Children.Add(tb2);
            }
            o = g;
            BindingOperations.SetBinding(o, Control.HeightProperty, new Binding() { Source = StackHeight });
            return false;
        }


        //public delegate void DataUpdateEventHandler(Common.physics_object o);
        //public event DataUpdateEventHandler? DataUpdated;



    }

}
