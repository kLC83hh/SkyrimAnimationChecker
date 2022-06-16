using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;

namespace SkyrimAnimationChecker.Common
{
    public interface IVM_Visual
    {
        public string VM_V_collective { get; set; }
    }
}
namespace SkyrimAnimationChecker
{
    /// <summary>
    /// Interaction logic for CBPC_Physics_Column.xaml
    /// </summary>
    public partial class CBPC_Physics_Column : UserControl
    {
        public CBPC_Physics_Column(Common.IVM_Visual vm, string col = "") { InitializeComponent(); this.vm = vm; if (!string.IsNullOrWhiteSpace(col)) { Collective = col; } }

        private readonly Common.IVM_Visual vm;

        #region DependencyProperty
        public string Collective
        {
            get => (string)GetValue(CollectiveProperty);
            set => SetValue(CollectiveProperty, value);
        }
        public static readonly DependencyProperty CollectiveProperty
            = DependencyProperty.Register(
                  "Collective", typeof(string), typeof(CBPC_Physics_Column),
                  new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not CBPC_Physics_Column) return;
                      //M.D($"Collective Property Callback {e.NewValue}");
                      //((CBPC_Physics_Column)d).vm.VM_V_collective = (string)e.NewValue;
                      //if (!string.IsNullOrWhiteSpace((string)e.NewValue))
                      //{
                      //    //M.D("Collective Property Callback check passed");
                      //    //((CBPC_Physics_Column)d).Collective = (string)e.NewValue;
                      //    //M.D($"Collective Property Callback {((CBPC_Physics_Column)d).Collective}");
                      //}
                  })
              ));
        public double StackHeight
        {
            get => (double)GetValue(StackHeightProperty);
            set => SetValue(StackHeightProperty, value);
        }
        public static readonly DependencyProperty StackHeightProperty
            = DependencyProperty.Register(
                  "StackHeight", typeof(double), typeof(CBPC_Physics_Column),
                  new FrameworkPropertyMetadata(18.0, new PropertyChangedCallback((d, e) =>
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
                  new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) =>
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
                  new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not CBPC_Physics_Column) return;
                      ((CBPC_Physics_Column)d).Make();
                  }))
              );
        public object[]? Option
        {
            get => (object[])GetValue(OptionProperty);
            set => SetValue(OptionProperty, value);
        }
        public static readonly DependencyProperty OptionProperty
            = DependencyProperty.Register(
                  "Option", typeof(object[]), typeof(CBPC_Physics_Column),
                  new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not CBPC_Physics_Column) return;
                      ((CBPC_Physics_Column)d).Make();
                  }))
              );
        #endregion
        #region events
        public delegate void CopyEventHandler(string way);
        public event CopyEventHandler? Copy;
        #endregion

        // do loop to make column
        private void Make()
        {
            if (Data == null) return;
            //if (Option != null && Data.Length != Option.Length) return;
            //M.D($"{Data.Length} {Option?.Length}");

            if (Data is string[])
            {
                H0.Visibility = Visibility.Hidden; H1.Visibility = Visibility.Hidden; HL.Visibility = Visibility.Hidden; HR.Visibility = Visibility.Hidden;
                B01.Visibility = Visibility.Visible; B10.Visibility = Visibility.Visible;
            }
            panel.Children.Clear();

            if (Data is string[] s)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (TryMakeOne(out UIElement? o, s[i])) continue;
                    else panel.Children.Add(o);
                }
            }
            else if (Data is Common.physics_object[] p)
            {
                if (Option is string[] k)
                {
                    for (int i = 0; i < k.Length; i++)
                    {
                        if (TryMakeOne(out UIElement? o, p.First(x => x.Key == k[i]), k[i])) continue;
                        else panel.Children.Add(o);
                    }
                }
            }
            //for (int i = 0; i < Data.Length; i++)
            //{
            //    if (TryMakeOne(out UIElement? o, Option?[i], Data[i])) continue;
            //    else panel.Children.Add(o);
            //}
        }

        // select what to make
        //private bool TryMakeOne(out UIElement? o, object? op = null, params object[] d)
        //{
        //    if (d.Length == 1 && d[0] is Common.physics_object dpo)
        //    {
        //        //M.D($"{dpo.Key} {op}");
        //        if (op == null) return TryMakeOne(out o, dpo);
        //        else return TryMakeOne(out o, dpo, (string)op);
        //    }
        //    else if (d.Length == 1 && d[0] is string ds)
        //    {
        //        if (op == null) return TryMakeOne(out o, ds);
        //        else return TryMakeOne(out o, ds, (Common.physics_object)op);
        //    }
        //    else if (d.Length == 1 && d[0] is double[] dd) return TryMakeOne(out o, dd);

        //    o = null;
        //    return true;
        //}

        // make PhysicsBox
        private bool TryMakeOne(out UIElement? o, Common.physics_object d)
        {
            if (!d.Use) { o = null; return true; }

            o = new Visuals.PhysicsBox() { Physics = d };

            string colpara = $"all,{GetCollective(d.Key)}";
            BindingOperations.SetBinding(o, Control.VisibilityProperty, new Binding("Collective") { Source = this, Converter = new CollectiveConverter(), ConverterParameter = colpara });
            BindingOperations.SetBinding(o, Control.HeightProperty, new Binding() { Source = StackHeight });

            return false;
        }
        private bool TryMakeOne(out UIElement? o, Common.physics_object d, string? op)
        {
            if (d.Key != op) { o = null; return true; }

            if (!d.Use) o = new Rectangle();
            else o = new Visuals.PhysicsBox() { Physics = d };

            string colpara = $"all,{GetCollective(d.Key)}";
            BindingOperations.SetBinding(o, Control.VisibilityProperty, new Binding("Collective") { Source = this, Converter = new CollectiveConverter(), ConverterParameter = colpara });
            BindingOperations.SetBinding(o, Control.HeightProperty, new Binding() { Source = StackHeight });

            return false;
        }
        // make header string
        private bool TryMakeOne(out UIElement? o, string d)
        {
            o = new TextBlock() { Text = d };
            string colpara = $"all,{GetCollective(d)}";
            BindingOperations.SetBinding(o, Control.VisibilityProperty, new Binding("Collective") { Source = this, Converter = new CollectiveConverter(), ConverterParameter = colpara });
            BindingOperations.SetBinding(o, Control.HeightProperty, new Binding() { Source = StackHeight });
            return false;
        }
        private bool TryMakeOne(out UIElement? o, string d, Common.physics_object? op)
        {
            if (op != null && !op.Use) { o = null; return true; }
            o = new TextBlock() { Text = d };
            string colpara = $"all,{GetCollective(d)}";
            BindingOperations.SetBinding(o, Control.VisibilityProperty, new Binding("Collective") { Source = this, Converter = new CollectiveConverter(), ConverterParameter = colpara });
            BindingOperations.SetBinding(o, Control.HeightProperty, new Binding() { Source = StackHeight });
            return false;
        }
        // old make value boxes
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

        private static string GetCollective(string key)
        {
            if (key.StartsWith("collision", StringComparison.CurrentCultureIgnoreCase)) return "collision";
            else if (key.EndsWith("Rot", StringComparison.CurrentCultureIgnoreCase) || key.StartsWith("rotation", StringComparison.CurrentCultureIgnoreCase)) return "rotation";
            else return "straight";
        }

        private void Copy_Button_Click(object sender, RoutedEventArgs e) => Copy?.Invoke((string)((sender as Button)?.Content ?? string.Empty));
    }

}
