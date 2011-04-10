using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFCommon.Tools
{
    public interface IToolService
    {
        IDrawingSurface DrawingSurface { get; }

        void RegisterTool(ITool tool);
        void UnregisterTool(ITool tool);

        void UnsuspendAll();
        void SuspendAll();
        void SuspendAll(ITool exclude);

        ITool GetTool(Guid toolId);
        ITool GetTool(string name);

        bool ActivateTool(Guid toolId);
        bool ActivateTool(ITool tool);

        bool DeactivateTool(ITool tool);
        void DeactivateAll();
    }
}
