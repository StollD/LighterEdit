using System;
using System.Collections.Generic;
using LighterEdit.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace LighterEdit.UI
{
    /// <summary>
    /// Shows a window to select an element from a list
    /// </summary>
    public class Selector<T> : Window<Selector<T>>
    {
        /// <summary>
        /// A list of objects that can be selected
        /// </summary>
        private List<T> _objects;

        /// <summary>
        /// A function that should format the objects for displaying their names
        /// </summary>
        private Func<T, String> _format;

        /// <summary>
        /// Applies the selection
        /// </summary>
        private Action<T> _callback;

        /// <summary>
        /// Displays the selection window
        /// </summary>
        public void ShowSelector(List<T> objects, Action<T> callback, Func<T, String> format = null)
        {
            // If the window is already opened, abort
            if (IsOpen)
                return;
            
            // Apply the new parameters
            _objects = objects;
            _format = format ?? (o => o.ToString());
            _callback = callback;
            
            // Open the Window
            Open();
        }

        /// <summary>
        /// Returns the title of the window
        /// </summary>
        public override String GetTitle()
        {
            return "Select";
        }

        /// <summary>
        /// Returns the width of the window
        /// </summary>
        public override Single GetWidth()
        {
            return 300f;
        }

        /// <summary>
        /// Returns the dialog elements that assemble the window
        /// </summary>
        protected override DialogGUIBase[] GetDialogElements()
        {
            DialogGUIVerticalLayout layout = new DialogGUIVerticalLayout(290, 500, 4, new RectOffset(5, 23, 5, 5), TextAnchor.UpperCenter, new DialogGUIBase[0]);
            layout.stretchWidth = true;
            layout.AddChild(new DialogGUIContentSizer(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize, true));
            foreach (T obj in _objects)
            {
                layout.AddChild(new DialogGUIHorizontalLayout(true, true, FixScrollButton(new DialogGUIButton(
                    _format(obj),
                    () =>
                    {
                        _callback(obj);
                        Close();
                    }, false))));
            }
            return new DialogGUIBase[]
            {
                new DialogGUIScrollList(new Vector2(290f, 500f), false, true, layout)
            };
        }
    }
}