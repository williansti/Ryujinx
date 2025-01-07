using Avalonia.Controls;

namespace Ryujinx.Ava.Utilities.Compat
{
    public partial class CompatibilityList : UserControl
    {
        public CompatibilityList()
        {
            InitializeComponent();
        }

        private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (DataContext is not CompatibilityViewModel cvm)
                return;

            if (sender is not TextBox searchBox)
                return;
        
            cvm.Search(searchBox.Text);
        }
    }
}
