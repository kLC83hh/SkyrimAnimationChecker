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
    /// Interaction logic for CBPC_colider.xaml
    /// </summary>
    public partial class CBPC_collider : UserControl
    {
        public CBPC_collider() { InitializeComponent(); DataContext = ColliderObject; }
        public CBPC_collider(CBPC_collider_object o) { InitializeComponent(); ColliderObject = o; DataContext = ColliderObject; }


        public CBPC_collider_object ColliderObject
        {
            get => (CBPC_collider_object)GetValue(ColliderObjectProperty);
            set => SetValue(ColliderObjectProperty, value);
        }

        public static readonly DependencyProperty ColliderObjectProperty
            = DependencyProperty.Register(
                  "ColliderObject",
                  typeof(CBPC_collider_object),
                  typeof(CBPC_collider),
                  new PropertyMetadata(null, new PropertyChangedCallback(OnColliderObjectChanged))
              );
        private static void OnColliderObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CBPC_collider) ((CBPC_collider)d).OnColliderObjectChanged(e);
        }
        private void OnColliderObjectChanged(DependencyPropertyChangedEventArgs e) => DataContext = e.NewValue;



        public delegate void ColliderUpdateEventHandler(CBPC_collider_object collider);

        public event ColliderUpdateEventHandler? ColliderUpdateEvent;

        private void Write_CheckBox_Click(object sender, RoutedEventArgs e) => ColliderUpdateEvent?.Invoke(ColliderObject);
    }
}
