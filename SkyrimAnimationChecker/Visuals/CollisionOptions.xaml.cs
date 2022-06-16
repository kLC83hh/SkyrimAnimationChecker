using SkyrimAnimationChecker.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SkyrimAnimationChecker
{
    /// <summary>
    /// Interaction logic for CollisionOptions.xaml
    /// </summary>
    public partial class CollisionOptions : UserControl
    {
        public CollisionOptions() => InitializeComponent();
        public CollisionOptions((cc_options_object, cc_extraoptions_object) op)
        {
            InitializeComponent();
            Options = op.Item1;
            ExtraOptions = op.Item2;
            //DataContext = Options;
        }

        #region Properties
        public cc_options_object? Options
        {
            get => (cc_options_object)GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }
        public static readonly DependencyProperty OptionsProperty
            = DependencyProperty.Register(
                "Options", typeof(cc_options_object), typeof(CollisionOptions),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                {
                    if (d is not CollisionOptions) return;
                }))
            );
        public cc_extraoptions_object? ExtraOptions
        {
            get => (cc_extraoptions_object)GetValue(ExtraOptionsProperty);
            set => SetValue(ExtraOptionsProperty, value);
        }
        public static readonly DependencyProperty ExtraOptionsProperty
            = DependencyProperty.Register(
                "ExtraOptions", typeof(cc_extraoptions_object), typeof(CollisionOptions),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                {
                    if (d is not CollisionOptions) return;
                    ((CollisionOptions)d).Make();
                }))
            );
        #endregion

        private void Make()
        {
            if (ExtraOptions == null) return;
            extrapanel.Children.Clear();
            for (int i = 0; i < ExtraOptions?.Keys.Length; i++)
            {
                if (i % 2 == 0)
                {
                    var g = new Grid();
                    string key = ExtraOptions.Keys[i];
                    //M.D(ExtraOptions.Find(key));

                    var vb = new Viewbox();
                    vb.SetValue(Grid.ColumnProperty, 0);
                    var lb = new TextBlock();
                    lb.SetBinding(TextBlock.TextProperty, new Binding() { Source = key });
                    vb.Child = lb;
                    g.Children.Add(vb);

                    var tb = new TextBox();
                    tb.SetBinding(TextBox.TextProperty, new Binding() { Path = new PropertyPath(key), Source = ExtraOptions });
                    tb.SetValue(Grid.ColumnProperty, 1);
                    g.Children.Add(tb);

                    extrapanel.Children.Add(g);
                }
                else
                {
                    var g = (Grid)extrapanel.Children[^1];
                    string key = ExtraOptions.Keys[i];

                    var vb = new Viewbox();
                    vb.SetValue(Grid.ColumnProperty, 2);
                    var lb = new TextBlock();
                    lb.SetBinding(TextBlock.TextProperty, new Binding() { Source = key });
                    vb.Child = lb;
                    g.Children.Add(vb);

                    var tb = new TextBox();
                    tb.SetBinding(TextBox.TextProperty, new Binding() { Path = new PropertyPath(key), Source = ExtraOptions });
                    tb.SetValue(Grid.ColumnProperty, 3);
                    g.Children.Add(tb);
                }
            }
        }



    }
}
