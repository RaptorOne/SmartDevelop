using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace WPFMapUserControls.Behaviours.DragNDrop
{
	public static class DragDropManager
	{
		private static UIElement _draggedElt;
		private static bool _isMouseDown = false;
		private static Point _dragStartPoint;
        private static Point _offsetPoint;
		private static DropPreviewAdorner _overlayElt;

		#region Dependency Properties
		public static readonly DependencyProperty DragSourceAdvisorProperty =
			DependencyProperty.RegisterAttached("DragSourceAdvisor", typeof(IDragSourceAdvisor), typeof(DragDropManager),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDragSourceAdvisorChanged)));

		public static readonly DependencyProperty DropTargetAdvisorProperty =
			DependencyProperty.RegisterAttached("DropTargetAdvisor", typeof(IDropTargetAdvisor), typeof(DragDropManager),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDropTargetAdvisorChanged)));

		public static void SetDragSourceAdvisor(DependencyObject depObj, bool isSet)
		{
			depObj.SetValue(DragSourceAdvisorProperty, isSet);
		}

		public static void SetDropTargetAdvisor(DependencyObject depObj, bool isSet)
		{
			depObj.SetValue(DropTargetAdvisorProperty, isSet);
		}

		public static IDragSourceAdvisor GetDragSourceAdvisor(DependencyObject depObj)
		{
			return depObj.GetValue(DragSourceAdvisorProperty) as IDragSourceAdvisor;
		}

		public static IDropTargetAdvisor GetDropTargetAdvisor(DependencyObject depObj)
		{
			return depObj.GetValue(DropTargetAdvisorProperty) as IDropTargetAdvisor;
		}

		#endregion

		#region Property Change handlers
		private static void OnDragSourceAdvisorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
		{
			UIElement sourceElt = depObj as UIElement;
			if (args.NewValue != null && args.OldValue == null)
			{
				sourceElt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
				sourceElt.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
				sourceElt.PreviewMouseUp += new MouseButtonEventHandler(DragSource_PreviewMouseUp);

				// Set the Drag source UI
				IDragSourceAdvisor advisor = args.NewValue as IDragSourceAdvisor;
				advisor.SourceUI = sourceElt;
			}
			else if (args.NewValue == null && args.OldValue != null)
			{
				sourceElt.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
				sourceElt.PreviewMouseMove -= DragSource_PreviewMouseMove;
				sourceElt.PreviewMouseUp -= DragSource_PreviewMouseUp;
			}
		}

		static void DragSource_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			_isMouseDown = false;
			Mouse.Capture(null);
		}

		private static void OnDropTargetAdvisorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
		{
			UIElement targetElt = depObj as UIElement;
			if (args.NewValue != null && args.OldValue == null)
			{
				targetElt.PreviewDragEnter += new DragEventHandler(DropTarget_PreviewDragEnter);
				targetElt.PreviewDragOver += new DragEventHandler(DropTarget_PreviewDragOver);
				targetElt.PreviewDragLeave += new DragEventHandler(DropTarget_PreviewDragLeave);
				targetElt.PreviewDrop += new DragEventHandler(DropTarget_PreviewDrop);
				targetElt.AllowDrop = true;

				// Set the Drag source UI
				IDropTargetAdvisor advisor = args.NewValue as IDropTargetAdvisor;
				advisor.TargetUI = targetElt;
			}
			else if (args.NewValue == null && args.OldValue != null)
			{
				targetElt.PreviewDragEnter -= DropTarget_PreviewDragEnter;
				targetElt.PreviewDragOver -= DropTarget_PreviewDragOver;
				targetElt.PreviewDragLeave -= DropTarget_PreviewDragLeave;
				targetElt.PreviewDrop -= DropTarget_PreviewDrop;
				targetElt.AllowDrop = false;
			}
		}

        static void targetElt_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }
		#endregion

		/* ____________________________________________________________________
		 *		Drop Target events 
		 * ____________________________________________________________________
		 */
		static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
		{
			if (UpdateEffects(sender, e) == false) return;

			IDropTargetAdvisor advisor = GetDropTargetAdvisor(sender as DependencyObject);
			Point dropPoint = e.GetPosition(sender as UIElement);

			// Calculate displacement for (Left, Top)
			Point offset = e.GetPosition(_overlayElt);
			dropPoint.X = dropPoint.X - offset.X;
			dropPoint.Y = dropPoint.Y - offset.Y;

			advisor.OnDropCompleted(e.Data, dropPoint);
			RemovePreviewAdorner();
            _offsetPoint = new Point(0, 0);

		}

		static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
		{
			if (UpdateEffects(sender, e) == false) return;

			RemovePreviewAdorner();
			e.Handled = true;
		}

		static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
		{
			if (UpdateEffects(sender, e) == false) return;
			// Update position of the preview Adorner
			Point position = e.GetPosition(sender as UIElement);
			_overlayElt.Left = position.X - _offsetPoint.X;
			_overlayElt.Top = position.Y - _offsetPoint.Y;
			
			e.Handled = true;
		}

		static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
		{

			if (UpdateEffects(sender, e) == false) return;
            
			// Setup the preview Adorner
			UIElement feedbackUI = GetDropTargetAdvisor(sender as DependencyObject).GetVisualFeedback(e.Data);
            _offsetPoint = GetDropTargetAdvisor(sender as DependencyObject).GetOffsetPoint(e.Data);
			CreatePreviewAdorner(sender as UIElement, feedbackUI);

			e.Handled = true;
		}

		static bool UpdateEffects(object uiObject, DragEventArgs e)
		{
			IDropTargetAdvisor advisor = GetDropTargetAdvisor(uiObject as DependencyObject);
			if (advisor.IsValidDataObject(e.Data) == false) return false;

			if ((e.AllowedEffects & DragDropEffects.Move) == 0 &&
				(e.AllowedEffects & DragDropEffects.Copy) == 0)
			{
				e.Effects = DragDropEffects.None;
				return true;
			}

			if ((e.AllowedEffects & DragDropEffects.Move) != 0 &&
				(e.AllowedEffects & DragDropEffects.Copy) != 0)
			{
                if ((e.KeyStates & DragDropKeyStates.ControlKey) != 0)
                {
                }
				e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0) ?
					DragDropEffects.Copy : DragDropEffects.Move;
			}

			return true;
		}

		/* ____________________________________________________________________
		 *		Drag Source events 
		 * ____________________________________________________________________
		 */
		static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// Make this the new drag source
			IDragSourceAdvisor advisor = GetDragSourceAdvisor(sender as DependencyObject);

			if (advisor.IsDraggable(e.Source as UIElement) == false) return;

			_draggedElt = e.Source as UIElement;
			_dragStartPoint = e.GetPosition(GetTopContainer());
            _offsetPoint = e.GetPosition(_draggedElt);
			_isMouseDown = true;

		}

		static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (_isMouseDown && IsDragGesture(e.GetPosition(GetTopContainer())))
			{
				DragStarted(sender as UIElement);
			}
		}

		static void DragStarted(UIElement uiElt)
		{
			_isMouseDown = false;
			Mouse.Capture(uiElt);

			IDragSourceAdvisor advisor = GetDragSourceAdvisor(uiElt as DependencyObject);
			DataObject data = advisor.GetDataObject(_draggedElt, _offsetPoint);
			DragDropEffects supportedEffects = advisor.SupportedEffects;

			// Perform DragDrop

			DragDropEffects effects = System.Windows.DragDrop.DoDragDrop(_draggedElt, data, supportedEffects);
			advisor.FinishDrag(_draggedElt, effects);

			// Clean up
			RemovePreviewAdorner();
			Mouse.Capture(null);
			_draggedElt = null;
		}

		static bool IsDragGesture(Point point)
		{
			bool hGesture = Math.Abs(point.X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance;
			bool vGesture = Math.Abs(point.Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance;

			return (hGesture | vGesture);
		}

		/* ____________________________________________________________________
		 *		Utility functions
		 * ____________________________________________________________________
		 */
		static UIElement GetTopContainer()
		{
			return Application.Current.MainWindow.Content as UIElement;
		}

		private static void CreatePreviewAdorner(UIElement adornedElt, UIElement feedbackUI)
		{
			// Clear if there is an existing preview adorner
			RemovePreviewAdorner();

			AdornerLayer layer = AdornerLayer.GetAdornerLayer(GetTopContainer());
			_overlayElt = new DropPreviewAdorner(feedbackUI, adornedElt);
			layer.Add(_overlayElt);
		}

		private static void RemovePreviewAdorner()
		{
			if (_overlayElt != null)
			{
				AdornerLayer.GetAdornerLayer(GetTopContainer()).Remove(_overlayElt);
				_overlayElt = null;
			}
		}

	}
}
