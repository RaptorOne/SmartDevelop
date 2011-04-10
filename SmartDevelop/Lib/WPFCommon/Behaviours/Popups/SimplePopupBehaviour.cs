using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace WPFCommon.Behaviours.Popups
{
    /// <summary>
    /// Shows a custom Popup on MouseHover
    /// </summary>
    public class SimplePopupBehaviour : ControlBehaviour
    {
        protected Popup popup;

        public SimplePopupBehaviour(FrameworkElement e, UIElement popupUI) 
            : base(e) {
                popup = new Popup()
                {
                    AllowsTransparency = true,
                    PlacementTarget = e,
                    Placement = PlacementMode.Mouse,
                    PopupAnimation = PopupAnimation.Scroll
                };
                popup.Child = popupUI;
        }

        protected override void Attach(System.Windows.FrameworkElement e) {
           e.MouseEnter += new System.Windows.Input.MouseEventHandler(Element_MouseEnter);
           e.MouseLeave += new System.Windows.Input.MouseEventHandler(Element_MouseLeave);
        }
        
        protected virtual void Element_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
            if (popup.IsOpen) {
                popup.IsOpen = false;
            }
        }

        protected virtual void Element_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            if (!popup.IsOpen) {
                popup.IsOpen = true;
            }
        }
    }
}
