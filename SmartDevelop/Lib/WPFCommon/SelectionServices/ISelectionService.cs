using System;
using System.Collections.Generic;
namespace WPFCommon.SelectionServices
{
    public interface ISelectionService
    {
        /// <summary>
        /// Raised wherever the selection chagnes
        /// </summary>
        event EventHandler SelectionChanged;

        /// <summary>
        /// Adds the selectable element to to selected Items (select it)
        /// </summary>
        /// <param name="element"></param>
        void AddSelection(ISelectable element);

        /// <summary>
        /// Removes all selections
        /// </summary>
        void ClearSelection();

        /// <summary>
        /// Holds the last selected Object
        /// </summary>
        ISelectable LastSelectedObject { get; }

        /// <summary>
        /// Removes the selection  from the object.
        /// </summary>
        /// <param name="element"></param>
        void RemoveSelection(ISelectable element);

        /// <summary>
        /// Request to select the Object. Handles specail cases like pressed shift for multi selection and other.
        /// </summary>
        /// <param name="element"></param>
        void RequestSelection(ISelectable element);

        IEnumerable<ISelectable> GetSelectedElements();

        int Count { get; }

        bool IsEmpty { get; }

        IEnumerable<T> GetSelectedElements<T>() where T : class;
    }
}
