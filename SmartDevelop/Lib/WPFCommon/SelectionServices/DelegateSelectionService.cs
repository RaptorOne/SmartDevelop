using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace WPFCommon.SelectionServices
{
    /*
    public interface ISelectableInputElement : IInputElement, ISelectable { }

    public class DelegateSelectionService : ISelectionService
    {
        #region Fields

        ISelectable _lastSelectedObject = null;
        List<ISelectableInputElement> _registeredSelectables = new List<ISelectableInputElement>();

        #endregion

        public void RegisterSelectable(ISelectableInputElement element) {
            _registeredSelectables.Add(element);
            element.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        }
        public void UnRegisterSelectable(ISelectableInputElement element) {
            _registeredSelectables.Remove(element);
            element.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
        }

        protected virtual void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.Source is ISelectableInputElement) {
                OnSelectableClick(e.Source as ISelectableInputElement);
            }
        }

        /// <summary>
        /// Occurs when a Selectable is Clicked
        /// </summary>
        /// <param name="selectable"></param>
        protected virtual void OnSelectableClick(ISelectableInputElement selectable) {

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift) {
                ClearSelection();
            }

            if (selectable.IsSelected) {
                RemoveSelection(selectable);
            } else {
                this.AddSelection(selectable);
            }
        }

        public void AddSelection(ISelectable element) {
            element.IsSelected = false;
            _lastSelectedObject = element;
        }

        public void ClearSelection() {
            foreach (var s in _registeredSelectables) {
                if (s.IsSelected == true)
                    s.IsSelected = false;
            }
        }

        public ISelectable LastSelectedObject {
            get { return _lastSelectedObject; }
            private set { _lastSelectedObject = value; }
        }

        public void RemoveSelection(ISelectable element) {
            element.IsSelected = false;

            if (LastSelectedObject == element && SelectedItems.Any()) {
                LastSelectedObject = SelectedItems.Last();
            }
        }

        public IEnumerable<ISelectable> SelectedItems {
            get {
                return from s in _registeredSelectables
                       where s.IsSelected
                       select s;
            }
        }

    }
    */
}
