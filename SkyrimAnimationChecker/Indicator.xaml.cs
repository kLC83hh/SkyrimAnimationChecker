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
    /// Interaction logic for Indicator.xaml
    /// </summary>
    public partial class Indicator : UserControl
    {
        public Indicator() => InitializeComponent();

        public bool Trigger
        {
            get => (bool)GetValue(IndicateProperty);
            set => SetValue(IndicateProperty, value);
        }
        public static readonly DependencyProperty IndicateProperty
            = DependencyProperty.Register("Trigger", typeof(bool), typeof(Indicator),
                new PropertyMetadata(false));
    }
}
