using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace WPFUserControls.Loaders
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CircularGlowLoader : UserControl
    {
        public CircularGlowLoader() {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) {
                Rotate();
            }
        }

        private void Rotate() {
            var animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = 360;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1.5));
            animation.RepeatBehavior = RepeatBehavior.Forever;

            var rt = new RotateTransform();

            this.RenderTransformOrigin = new Point(0.5, 0.5);
            this.RenderTransform = rt;
            rt.BeginAnimation(RotateTransform.AngleProperty, animation);
        }
    }
}
