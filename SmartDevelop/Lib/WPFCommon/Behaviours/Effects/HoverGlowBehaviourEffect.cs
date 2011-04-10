using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WPFCommon.GraphicsResources;
using System.Windows.Media.Effects;

namespace WPFCommon.Behaviours.Effects
{
    public class HoverGlowBehaviourEffect : ControlBehaviour
    {
        Effect hovereffect;
        Effect backup = null;

        #region Constructor

        public HoverGlowBehaviourEffect(FrameworkElement e, Effect effect)
            : base(e) {
                hovereffect = effect;
        }

        public HoverGlowBehaviourEffect(FrameworkElement e)
            : this(e, GlobalEffects.RedGlowEffect) { }

        #endregion

        #region  Mouse Hover

        private void RIEDOHomeLogo_MouseEnter(object sender, MouseEventArgs e) {
            backup = AttachedElement.Effect;
            AttachedElement.Effect = hovereffect;
        }

        private void RIEDOHomeLogo_MouseLeave(object sender, MouseEventArgs e) {
            AttachedElement.Effect = backup;
        }

        #endregion

        protected override void Attach(FrameworkElement e) {
            e.MouseEnter += new MouseEventHandler(RIEDOHomeLogo_MouseEnter);
            e.MouseLeave += new MouseEventHandler(RIEDOHomeLogo_MouseLeave);
        }
    }
}
