using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

using Windows.UI.ViewManagement;

using System.Collections.ObjectModel;

using Windows.UI.Xaml.Media;

namespace SJCAM_Zone
{
    public class CustomFrame : Frame
    {
        public CustomFrame()
        {
            this.DefaultStyleKey = typeof(CustomFrame);
        }

        private Button HamburgerButton;
        private Button HamburgerButton2;
        private SplitView MySplitView;
        public Grid OverlayGrid;

        /// <summary>
        /// Вызывается при построении макета.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.HamburgerButton = GetTemplateChild("HamburgerButton") as Button;
            this.HamburgerButton2 = GetTemplateChild("HamburgerButton2") as Button;
            this.MySplitView = GetTemplateChild("MySplitView") as SplitView;
            this.OverlayGrid = GetTemplateChild("Overlay") as Grid;

            this.HamburgerButton2.Click += HamburgerButton_Click;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, this.MySplitView.IsPaneOpen ? "Small" : "Large", true);
        }
    }
}
