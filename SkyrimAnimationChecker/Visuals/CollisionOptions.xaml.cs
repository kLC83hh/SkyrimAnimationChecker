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
            DataContext = Options;
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
                new PropertyMetadata(null, new PropertyChangedCallback((d, e) =>
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
                new PropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                {
                    if (d is not CollisionOptions) return;
                    //((CollisionOptions)d).DataContext = e.NewValue;
                }))
            );
        #endregion

    }
}
