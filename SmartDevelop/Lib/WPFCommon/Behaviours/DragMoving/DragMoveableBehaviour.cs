using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;

namespace WPFCommon.Behaviours.DragMoving
{   
    /// <summary>
    /// Makes the Control Moveable in the Parent Container.
    /// You have to subclass and implement the abstract SetPosition(e, x, y) Method to place the Control in the Parent
    /// </summary>
    public abstract class DragMoveableBehaviour : ControlBehaviour
    {
        #region Private Data

        //private bool firstClick = true;
        public bool isMoving = false;
        private Point relative;
        //private Point startPoint;
        private TranslateTransform moveTransform;

        #endregion

        public DragMoveableBehaviour(FrameworkElement e)
            : base(e) { }

        #region Attach / Deatach Behaviour

        protected override void Attach(FrameworkElement e) {
            e.RenderTransform = new TranslateTransform();
            moveTransform = e.RenderTransform as TranslateTransform;

            e.MouseMove += new MouseEventHandler(MoveMe_MouseMove);
            e.MouseDown += new MouseButtonEventHandler(MoveMe_MouseDown);
            e.MouseUp += new MouseButtonEventHandler(MoveMe_MouseUp);
        }

        protected override void DeAttach() {
            if (IsAtached) {
                // remove event handler
                this.AttachedElement.MouseMove -= new MouseEventHandler(MoveMe_MouseMove);
                this.AttachedElement.MouseDown += new MouseButtonEventHandler(MoveMe_MouseDown);
                this.AttachedElement.MouseUp += new MouseButtonEventHandler(MoveMe_MouseUp);

                // clean up private data
                //firstClick = true;
                IsMoving = false;
                relative = new Point();
                //startPoint = new Point();
                moveTransform = null;

                base.DeAttach();
            }
        }

        #endregion

        private bool IsMoving {
            get { return isMoving; }

            set {
                if (value) {
                    // ToDo: bring to front
                }

                isMoving = value;
            }
        }


        #region Mouse Events

        private void MoveMe_MouseUp(object sender, MouseEventArgs e) {
            IsMoving = false;
        }

        private void MoveMe_MouseDown(object sender, MouseEventArgs e) {
            var c = sender as FrameworkElement;

            relative = Mouse.GetPosition(c);
            IsMoving = true;
        }

        private void MoveMe_MouseMove(object sender, MouseEventArgs e) {
            var c = sender as FrameworkElement; // Attached Element

            //Get the position of the mouse relative to the controls parent              
            var MousePoint = Mouse.GetPosition(c.Parent as IInputElement);

            if (Mouse.LeftButton == MouseButtonState.Released)
                IsMoving = false;

            if (IsMoving) {
                var move = new Vector()
                {
                    X = MousePoint.X - relative.X,
                    Y = MousePoint.Y - relative.Y
                };
                SetPosition(move.X, move.Y);
            }
        }

        #endregion

        /// <summary>
        /// Set the Position of the attached Element
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected abstract void SetPosition(double x, double y);

    }
}
