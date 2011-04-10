using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WPFCommon.Input;
using System.Windows;

namespace WPFCommon.Tools
{
    public class ToolService : IToolService
    {
        #region Fields

        private IServiceProvider hostProvider;
        private Dictionary<Guid, ITool> tools;

        #endregion



        #region ctor

        public ToolService(IServiceProvider host, IDrawingSurface drawingSurface) {
            if (host == null)
                throw new ArgumentNullException("host");
            if (drawingSurface == null)
                throw new ArgumentNullException("drawingSurface");

            hostProvider = host;
            DrawingSurface = drawingSurface;
            InitializeService();
        }

        #endregion

        #region Initialization

        private void InitializeService() {
            tools = new Dictionary<Guid, ITool>();
            DrawingSurface.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DrawingSurface_MouseDown);
            DrawingSurface.MouseMove += new MouseEventHandler(DrawingSurface_MouseMove);
            DrawingSurface.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(DrawingSurface_MouseUp);
        }
        #endregion

        #region Input events binding



        void DrawingSurface_MouseDown(object sender, MouseButtonEventArgs e) {
            foreach (ITool tool in tools.Values) {
                if (tool is IMouseListener) {
                    ((IMouseListener)tool).MouseDown(e);
                    if (e.Handled) return;
                }
            }
        }


        void DrawingSurface_MouseMove(object sender, MouseEventArgs e) {
            foreach (ITool tool in tools.Values) {
                if (tool is IMouseListener) {
                    ((IMouseListener)tool).MouseMove(e);
                    if (e.Handled) return;
                }
            }
        }

        void DrawingSurface_MouseUp(object sender, MouseButtonEventArgs e) {
            foreach (ITool tool in tools.Values) {
                if (tool is IMouseListener) {
                    ((IMouseListener)tool).MouseUp(e);
                    if (e.Handled) return;
                }
            }
        }

        #endregion

        #region IToolService Members

        public IDrawingSurface DrawingSurface { get; private set; }

        public void RegisterTool(ITool tool) {
            if (tool == null) return;

            if (!tools.ContainsKey(tool.ToolId)) {
                tools.Add(tool.ToolId, tool);
                tool.ToolService = this;
            }
        }

        public void UnregisterTool(ITool tool) {
            if (tool == null) return;

            if (tools.ContainsKey(tool.ToolId))
                tools.Remove(tool.ToolId);
        }

        public void UnsuspendAll() {
            foreach (ITool tool in tools.Values)
                tool.IsSuspended = false;
        }

        public void SuspendAll() {
            foreach (ITool tool in tools.Values)
                tool.IsSuspended = true;
        }

        public void SuspendAll(ITool exclude) {
            foreach (ITool tool in tools.Values) {
                if (tool.ToolId != exclude.ToolId)
                    tool.IsSuspended = true;
            }
        }

        public ITool GetTool(string name) {
            foreach (ITool tool in tools.Values) {
                if (tool.Name == name)
                    return tool;
            }
            return null;
        }

        public ITool GetTool(Guid toolId) {
            if (tools.ContainsKey(toolId))
                return tools[toolId];
            else
                return null;
        }

        public bool ActivateTool(Guid toolId) {
            ITool tool = GetTool(toolId);
            return ActivateTool(tool);
        }

        public bool ActivateTool(ITool tool) {
            if (tool != null && tool.CanActivate)
                return tool.Activate();
            else
                return false;
        }

        public bool DeactivateTool(ITool tool) {
            if (tool != null && tool.Enabled && tool.IsActive)
                return tool.Deactivate();
            else
                return false;
        }

        public void DeactivateAll() {
            foreach (ITool tool in tools.Values)
                tool.Deactivate();
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType) {
            return hostProvider.GetService(serviceType);
        }

        #endregion
    }
}
