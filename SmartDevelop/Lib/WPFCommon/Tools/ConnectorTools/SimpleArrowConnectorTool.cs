using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFCommon.Input;
using System.Windows.Input;
using WPFCommon.Connectors;
using System.Windows;
using WPFCommon.Connectors.Converters;

namespace WPFCommon.Tools.ConnectorTools
{
    public class SimpleArrowConnectorTool : Tool, IMouseListener
    {
        #region ctor

        public SimpleArrowConnectorTool(string name, Type contentCreator)
            : base(name, contentCreator) {
            this.ToolId = Guid.NewGuid();
        }

        #endregion

        #region Connector Tool implementation

        public override Guid ToolId { get; protected set; }

        public override void OnExecute(object sender, ExecutedRoutedEventArgs e) {
            Activate();
        }

        public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = !IsSuspended;
        }
        #endregion

        IConnectable sourceObject;
        private IConnector link;
        private bool isLinkStarted;

        #region IMouseListener Members

        public void MouseDown(MouseEventArgs e) {
            if (IsSuspended) return;

            if (IsActive) {
                if (e.Source is IConnectable && ((IConnectable)e.Source).CanConnect) {
                    if (link == null || link.EndPoint != link.StartPoint) {
                        Point position = e.GetPosition(this.ToolService.DrawingSurface);
                        link = new ArrowLineConnector();
                        //link = System.Activator.CreateInstance<ArrowLine>();
                        link.StartPoint = link.EndPoint = position;

                        this.ToolService.DrawingSurface.Children.Insert(0, (ObjectConnector)link);
                        isLinkStarted = true;
                        sourceObject = e.Source as IConnectable;
                        e.Handled = true;
                    }
                } else
                    Deactivate();
            }
        }

        public void MouseMove(MouseEventArgs e) {
            if (IsActive && isLinkStarted) {
                // Set the new link end point to current mouse position
                link.EndPoint = e.GetPosition(this.ToolService.DrawingSurface);
                e.Handled = true;
            }
        }

        public void MouseUp(MouseEventArgs e) {
            if (IsActive) {
                // We released the button on MyThumb object
                if (e.Source is IConnectable && ((IConnectable)e.Source).CanConnect) {
                    var targetThumb = e.Source as IConnectable;
                    // if any line was drawn (avoid just clicking on the thumb) establish connection
                    if (e.GetPosition(this.ToolService.DrawingSurface) != link.StartPoint && sourceObject != targetThumb)
                        AddConnection(ConnectorType.Arrowhead, sourceObject, targetThumb);
                }

                Deactivate();
                // exit link drawing mode
                //e.Handled = true;        
            }

            isLinkStarted = false;

            if (link != null) {
                // remove line from the canvas
                this.ToolService.DrawingSurface.Children.Remove((ObjectConnector)link);
                // clear the link variable
                link = null;
            }
        }

        public void AddConnection(ConnectorType type, IConnectable source, IConnectable target) {
            switch (type) {
                case ConnectorType.Plain: {
                    PlainLineConnector conn = new PlainLineConnector();
                    conn.SetBinding(ObjectConnector.StartPointProperty, BindingHelper.CreateCenteredBinding(source));
                    conn.SetBinding(ObjectConnector.EndPointProperty, BindingHelper.CreateCenteredBinding(target));
                    this.ToolService.DrawingSurface.Children.Insert(0, conn);
                    break;
                }
                case ConnectorType.Arrowhead: {
                    var conn = new ArrowLineConnector();
                    conn.SetBinding(ObjectConnector.StartPointProperty, BindingHelper.CreateAngledBinding(source, target));
                    conn.SetBinding(ObjectConnector.EndPointProperty, BindingHelper.CreateAngledBinding(target, source));
                    this.ToolService.DrawingSurface.Children.Insert(0, conn);
                }
                break;
            }
        }

        #endregion
    }
}
