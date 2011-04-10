using System.Windows;

namespace WPFMapUserControls.Behaviours.DragNDrop
{
    public interface IDropTargetAdvisor
    {
		UIElement TargetUI
		{
			get;
			set;
		}
		bool IsValidDataObject(IDataObject obj);
		void OnDropCompleted(IDataObject obj, Point dropPoint);
		UIElement GetVisualFeedback(IDataObject obj);
        Point GetOffsetPoint(IDataObject obj);
	}
}
