using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace WPFCommon.SelectionServices
{
    /// <summary>
    /// SelectionService which handles SelectionLogic.
    /// Can handle multiple Selections. To specify custom Keys overwrite PreviewMouseLeftButtonDown Method.
    /// </summary>
    public class SelectionService : ISelectionService
    {
        #region Fields

        protected IInputElement _container;
        List<ISelectable> _selectedItems = new List<ISelectable>();
        ISelectable _lastSelectedObject = null;
 
        #endregion

        #region Events

        /// <summary>
        /// Raised when the selection has changed
        /// </summary>
        public event EventHandler SelectionChanged;

        #endregion

        #region Constructors

        public SelectionService() { }

        public SelectionService(IInputElement ucontainer) {
            _container = ucontainer;
            _container.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PreviewMouseLeftButtonDown);
        }

        #endregion

        protected virtual void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.Source is ISelectable) {
                RequestSelection(e.Source as ISelectable);
            }
        }

        #region Public Methods


        /// <summary>
        /// Occurs when a Selectable is Clicked
        /// </summary>
        /// <param name="selectable"></param>
        public void RequestSelection(ISelectable selectable) {

            var state = selectable.IsSelected;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift) {
                ClearSelection();
            }
            if (state) {
                RemoveSelection(selectable);
            } else {
                AddSelection(selectable);
            }
        }


        public virtual void AddSelection(ISelectable element) {
            if (element.CanSelect) {
                _selectedItems.Add(element);
                element.IsSelected = true;
                LastSelectedObject = element;
                if (SelectionChanged != null)
                    SelectionChanged(this, null);
            }
        }

        public virtual void RemoveSelection(ISelectable element) {
            _selectedItems.Remove(element);
            element.IsSelected = false;
            if (SelectionChanged != null)
                SelectionChanged(this, null);
            if (LastSelectedObject == element && _selectedItems.Any()) {
                LastSelectedObject = _selectedItems.Last();
            }
        }

        public int Count {
            get { return _selectedItems.Count; }
        }

        public virtual IEnumerable<ISelectable> GetSelectedElements() {
            return new List<ISelectable>(_selectedItems);
        }
        public virtual IEnumerable<T> GetSelectedElements<T>() where T : class {
            return from s in _selectedItems
                   where s is T
                   select s as T;
        }

        public virtual void ClearSelection() {
            foreach (var item in _selectedItems) {
                item.IsSelected = false;
            }
            LastSelectedObject = null;
            _selectedItems.Clear();
            if (SelectionChanged != null)
                SelectionChanged(this, null);
        }

        #endregion

        public bool IsEmpty {
            get { return this.Count == 0; }
        }

        public ISelectable LastSelectedObject {
            get { return _lastSelectedObject; }
            protected set { _lastSelectedObject = value; }
        }

    }
}
