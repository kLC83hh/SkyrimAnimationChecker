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
    /// Interaction logic for FolderLoader.xaml
    /// </summary>
    public partial class FolderLoader : UserControl
    {
        public FolderLoader()
        {
            InitializeComponent();
        }

        private List<int> flashing = new();
        private async Task FlashUI(Control c)
        {
            if (flashing.Contains(c.GetHashCode())) return;
            flashing.Add(c.GetHashCode());
            int delay = 100;
            if (c is CheckBox || c is Label)
            {
                c.FontWeight = FontWeights.Bold;
                await Task.Delay(delay);
                c.FontWeight = FontWeights.Normal;
                await Task.Delay(delay);
                c.FontWeight = FontWeights.Bold;
                await Task.Delay(delay);
                c.FontWeight = FontWeights.Normal;
            }
            flashing.RemoveAll(item => item == c.GetHashCode());
        }

        public async Task Flash() => await FlashUI(nameLable);



        #region Properties
        public Brush HeaderColor
        {
            get => (Brush)GetValue(HeaderColorProperty);
            set => SetValue(HeaderColorProperty, value);
        }
        public static readonly DependencyProperty HeaderColorProperty
            = DependencyProperty.Register(
                  "HeaderColor", typeof(Brush), typeof(FolderLoader),
                  new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not FolderLoader) return;
                      //((FolderLoader)d).Make();
                  }))
              );
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public static readonly DependencyProperty HeaderProperty
            = DependencyProperty.Register(
                  "Header", typeof(string), typeof(FolderLoader),
                  new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not FolderLoader) return;
                      //((FolderLoader)d).Make();
                  }))
              );
        public string Location
        {
            get => (string)GetValue(LocationProperty);
            set => SetValue(LocationProperty, value);
        }
        public static readonly DependencyProperty LocationProperty
            = DependencyProperty.Register(
                  "Location", typeof(string), typeof(FolderLoader),
                  new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not FolderLoader) return;
                      //((FolderLoader)d).Make();
                  }))
              );
        public string Location2
        {
            get => (string)GetValue(Location2Property);
            set => SetValue(Location2Property, value);
        }
        public static readonly DependencyProperty Location2Property
            = DependencyProperty.Register(
                  "Location2", typeof(string), typeof(FolderLoader),
                  new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not FolderLoader) return;
                      //((FolderLoader)d).Make();
                  }))
              );
        #endregion
        #region Events
        public delegate void LoadEventHandler(FolderLoader sender);
        public event LoadEventHandler? Load;
        #endregion

        private void PhysicsLocation_TextBox_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter || e.Key == Key.Return) Load?.Invoke(this); }
        private void ReloadPhysics_Button_Click(object sender, RoutedEventArgs e) => Load?.Invoke(this);


    }
}
