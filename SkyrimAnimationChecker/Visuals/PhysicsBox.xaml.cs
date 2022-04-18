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

namespace SkyrimAnimationChecker.Visuals
{
    /// <summary>
    /// Interaction logic for PhysicsBox.xaml
    /// </summary>
    public partial class PhysicsBox : UserControl
    {
        public PhysicsBox() => InitializeComponent();


        #region Properties
        public Common.physics_object Physics
        {
            get => (Common.physics_object)GetValue(PhysicsProperty);
            set => SetValue(PhysicsProperty, value);
        }
        public static readonly DependencyProperty PhysicsProperty
            = DependencyProperty.Register(
                  "Physics", typeof(Common.physics_object), typeof(PhysicsBox),
                  new PropertyMetadata(null, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not PhysicsBox) return;
                      ((PhysicsBox)d).DataContext = e.NewValue;
                  }))
              );
        #endregion

        #region Events
        //public delegate void PhysicsUpdateEventHandler(Common.physics_object o);
        //public event PhysicsUpdateEventHandler? PhysicsUpdated;
        #endregion


        //private void Value_TB_KeyDown(object sender, KeyEventArgs e) => PhysicsUpdated?.Invoke(Physics);

        private void ValueTB_GotFocus(object sender, RoutedEventArgs e) => (sender as TextBox)?.Dispatcher?.BeginInvoke(() => (sender as TextBox)?.SelectAll());
        //private void ValueTB_MouseDown(object sender, MouseButtonEventArgs e) { e.Handled = true; (sender as TextBox)?.SelectAll(); }



    }
}
