using System.Windows;

namespace SkyrimAnimationChecker
{
    /// <summary>
    /// Interaction logic for TextInputDialog.xaml
    /// </summary>
    public partial class TextInputDialog : Window
    {
        public TextInputDialog() { InitializeComponent(); InputBox.Focus(); }
        public TextInputDialog(string title) { InitializeComponent(); this.Title = title; InputBox.Focus(); }
        public TextInputDialog(string title, string defaultText) { InitializeComponent(); this.Title = title; this.Text = defaultText; InputBox.Focus(); InputBox.SelectAll(); }

        #region Properties
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty
            = DependencyProperty.Register(
                  "Text", typeof(string), typeof(TextInputDialog),
                  new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback((d, e) =>
                  {
                      if (d is not TextInputDialog) return;
                      //((CBPC_Physics)d).Make();
                  }))
              );
        #endregion


        private void OK_Button_Click(object sender, RoutedEventArgs e) { this.DialogResult = true; Close(); }


    }
}
