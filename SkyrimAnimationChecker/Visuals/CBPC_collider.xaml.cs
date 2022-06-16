using SkyrimAnimationChecker.Common;
using System.Windows;
using System.Windows.Controls;

namespace SkyrimAnimationChecker
{
    /// <summary>
    /// Interaction logic for CBPC_colider.xaml
    /// </summary>
    public partial class CBPC_collider : UserControl
    {
        public CBPC_collider() { InitializeComponent(); DataContext = ColliderObject; }
        public CBPC_collider(collider_object o) { InitializeComponent(); ColliderObject = o; DataContext = ColliderObject; }


        public collider_object ColliderObject
        {
            get => (collider_object)GetValue(ColliderObjectProperty);
            set => SetValue(ColliderObjectProperty, value);
        }

        public static readonly DependencyProperty ColliderObjectProperty
            = DependencyProperty.Register(
                  "ColliderObject",
                  typeof(collider_object),
                  typeof(CBPC_collider),
                  new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnColliderObjectChanged))
              );
        private static void OnColliderObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CBPC_collider collider) collider.OnColliderObjectChanged(e);
        }
        private void OnColliderObjectChanged(DependencyPropertyChangedEventArgs e) => DataContext = e.NewValue;



        public delegate void ColliderUpdateEventHandler(collider_object collider);

        public event ColliderUpdateEventHandler? ColliderUpdateEvent;

        private void Write_CheckBox_Click(object sender, RoutedEventArgs e) => ColliderUpdateEvent?.Invoke(ColliderObject);
    }
}
