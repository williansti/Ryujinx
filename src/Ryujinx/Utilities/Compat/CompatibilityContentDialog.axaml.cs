using FluentAvalonia.UI.Controls;
using System;

namespace Ryujinx.Ava.Utilities.Compat
{
    public partial class CompatibilityContentDialog : ContentDialog
    {
        protected override Type StyleKeyOverride => typeof(ContentDialog);
        
        public CompatibilityContentDialog() => InitializeComponent();
    }
}

