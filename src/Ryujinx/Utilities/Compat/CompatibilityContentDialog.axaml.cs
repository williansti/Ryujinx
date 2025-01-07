using Avalonia.Styling;
using FluentAvalonia.UI.Controls;
using Ryujinx.Ava.UI.Helpers;
using System;
using System.Threading.Tasks;

namespace Ryujinx.Ava.Utilities.Compat
{
    public partial class CompatibilityContentDialog : ContentDialog
    {
        protected override Type StyleKeyOverride => typeof(ContentDialog);

        public static async Task Show()
        {
            await CompatibilityHelper.InitAsync();

            CompatibilityContentDialog contentDialog = new()
            {
                Content = new CompatibilityList { DataContext = new CompatibilityViewModel(RyujinxApp.MainWindow.ViewModel.ApplicationLibrary) }
            };

            Style closeButton = new(x => x.Name("CloseButton"));
            closeButton.Setters.Add(new Setter(WidthProperty, 80d));
            
            Style closeButtonParent = new(x => x.Name("CommandSpace"));
            closeButtonParent.Setters.Add(new Setter(HorizontalAlignmentProperty, Avalonia.Layout.HorizontalAlignment.Right));

            contentDialog.Styles.Add(closeButton);
            contentDialog.Styles.Add(closeButtonParent);

            await ContentDialogHelper.ShowAsync(contentDialog);
        }
        
        public CompatibilityContentDialog() => InitializeComponent();
    }
}

