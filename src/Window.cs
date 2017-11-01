using System;
using KSP.UI.TooltipTypes;
using LighterEdit.Utility;
using Smooth.Delegates;
using UnityEngine;
using UnityEngine.UI;

namespace LighterEdit
{
    /// <summary>
    /// Base class for the UI namespace
    /// </summary>
    public abstract class Window<T> where T : class, new()
    {
        /// <summary>
        /// The internal instance variable
        /// </summary>
        private static T _instance;

        /// <summary>
        /// The currently active Instance of this window
        /// </summary>
        public static T Instance => _instance ?? (_instance = new T());
        
        /// <summary>
        /// Whether the window is created
        /// </summary>
        public Boolean IsOpen { get; protected set; }
        
        /// <summary>
        /// Whether the Window is visible
        /// </summary>
        public Boolean IsVisible { get; protected set; }

        /// <summary>
        /// The template for the window
        /// </summary>
        protected MultiOptionDialog Template;

        /// <summary>
        /// The dialog element that gets created
        /// </summary>
        protected PopupDialog Dialog;

        /// <summary>
        /// Returns the title of the window
        /// </summary>
        public abstract String GetTitle();

        /// <summary>
        /// Returns the dialog elements that assemble the window
        /// </summary>
        protected abstract DialogGUIBase[] GetDialogElements();

        /// <summary>
        /// Returns the width of the window
        /// </summary>
        public abstract Single GetWidth();

        /// <summary>
        /// A callback that is invoked when the window is closed
        /// </summary>
        protected Action OnClose;

        /// <summary>
        /// The current position of the window on the screen
        /// </summary>
        private Vector2 _position = new Vector2(100f, 100f);

        /// <summary>
        /// The base for building tooltip objects
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Tooltip_Text Prefab = AssetBase.GetPrefab<Tooltip_Text>("Tooltip_Text");

        /// <summary>
        /// Opens a new window
        /// </summary>
        public void Open()
        {
            if (IsOpen)
                return;
            IsOpen = IsVisible = true;
            Template = new MultiOptionDialog(GetTitle(), "", GetTitle(), HighLogic.UISkin,
                GetRect(_position.x, _position.y),
                GetDialogElements());
            Dialog = PopupDialog.SpawnPopupDialog(new Vector2(0f, 1f), new Vector2(0f, 1f), Template, true,
                HighLogic.UISkin, isModal: false);
            Dialog.SetDraggable(true);
        }

        public Rect GetRect(Single x, Single y)
        {
            x /= Screen.width;
            y = (Screen.height - y) / Screen.height;
            return new Rect(x, y, GetWidth(), -1f);
        }

        /// <summary>
        /// Toggles the visibility of the window
        /// </summary>
        public void ToggleVisibility()
        {
            if (IsVisible)
                Hide();
            else
                Show();
        }

        /// <summary>
        /// Hides the window
        /// </summary>
        public void Hide()
        {
            if (IsVisible)
                Dialog.gameObject.SetActive(IsVisible = false);
        }

        /// <summary>
        /// Shows the window
        /// </summary>
        public void Show()
        {
            if (!IsVisible)
                Dialog.gameObject.SetActive(IsVisible = true);
        }

        public void Close()
        {
            if (IsOpen)
            {
                Vector2 pos = Dialog.RTrf.anchoredPosition;
                _position = new Vector2(pos.x, -pos.y);
                Dialog.Dismiss();
                UnityEngine.Object.DestroyImmediate(Dialog.popupWindow);
                IsOpen = IsVisible = false;
                OnClose?.Invoke();
            }
        }

        /// <summary>
        /// Updates the dialog
        /// </summary>
        public void Redraw()
        {
            if (!IsOpen)
                return;
            Vector2 pos = Dialog.RTrf.anchoredPosition;
            Debug.Log(pos);
            Template = new MultiOptionDialog(GetTitle(), "", GetTitle(), HighLogic.UISkin,
                GetRect(pos.x, -pos.y),
                GetDialogElements());
            Dialog.Dismiss();
            UnityEngine.Object.Destroy(Dialog.popupWindow);
            Dialog = PopupDialog.SpawnPopupDialog(new Vector2(0f, 1f), new Vector2(0f, 1f), Template, true,
                HighLogic.UISkin, isModal: false);
            Dialog.SetDraggable(true);
            Dialog.gameObject.SetActive(IsVisible);
        }

        /// <summary>
        /// Adds a tooltip to the UI element
        /// </summary>
        protected DialogGUIBase Tooltip(DialogGUIBase dialog, String tip)
        {
            dialog.tooltipText = tip;
            if (dialog.tooltipText != "")
            {
                dialog.OnUpdate += () =>
                {
                    if (dialog.uiItem == null) return;
                    TooltipController_Text tt = dialog.uiItem.AddOrGetComponent<TooltipController_Text>();
                    if (tt == null) return;
                    tt.textString = tip;
                    tt.prefab = Prefab;
                };
            }
            return dialog;
        }

        /// <summary>
        /// Fixes the buttons inside of a scroll view.
        /// </summary>
        protected DialogGUIButton FixScrollButton(DialogGUIButton button)
        {
            button.OnUpdate += () =>
            {
                if (button.uiItem == null) return;
                if (button.uiItem.GetComponent<FixScrollRect>() != null) return;
                button.uiItem.AddComponent<FixScrollRect>().MainScroll =
                    Part.GetComponentUpwards<ScrollRect>(button.uiItem);
            };
            return button;
        }

        protected DialogGUIBase OnUpdate(DialogGUIBase dialog, Action<DialogGUIBase> callback)
        {
            dialog.OnUpdate += () => callback(dialog);
            return dialog;
        }
    }
}