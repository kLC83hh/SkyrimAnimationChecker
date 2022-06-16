using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }
        public static readonly DependencyProperty FillProperty
            = DependencyProperty.Register("Fill", typeof(Brush), typeof(Indicator),
                new PropertyMetadata(Brushes.OrangeRed));

        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }
        public static readonly DependencyProperty StrokeProperty
            = DependencyProperty.Register("Stroke", typeof(Brush), typeof(Indicator),
                new PropertyMetadata(Brushes.Black));
    }
}
