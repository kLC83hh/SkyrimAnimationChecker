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

namespace SkyrimAnimationChecker
{
    /// <summary>
    /// Interaction logic for CBPC_Physics.xaml
    /// </summary>
    public partial class CBPC_Physics : UserControl
    {


        public CBPC_Physics() { InitializeComponent(); DataContext = PhysicsObject; }
        public CBPC_Physics(physics_object_set o) { InitializeComponent(); PhysicsObject = o; DataContext = PhysicsObject; }


        public physics_object_set PhysicsObject
        {
            get => (physics_object_set)GetValue(PhysicsObjectProperty);
            set => SetValue(PhysicsObjectProperty, value);
        }

        public static readonly DependencyProperty PhysicsObjectProperty
            = DependencyProperty.Register(
                  "PhysicsObject",
                  typeof(physics_object_set),
                  typeof(CBPC_Physics),
                  new PropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not CBPC_Physics_Column) return;
                      ((CBPC_Physics_Column)d).DataContext = e.NewValue;
                  }))
              );



        public delegate void PhysicsUpdateEventHandler(physics_object_set obj);

        public event PhysicsUpdateEventHandler? PhysicsUpdateEvent;

        //private void Write_CheckBox_Click(object sender, RoutedEventArgs e) => ColliderUpdateEvent?.Invoke(PhysicsObject);



    }
}
