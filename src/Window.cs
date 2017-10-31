using System;
using UnityEngine;

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
        /// Opens a new window
        /// </summary>
        public void Open()
        {
            if (IsOpen)
                return;
            IsOpen = true;
            IsVisible = true;
            if (Template == null)
            {
                Template = new MultiOptionDialog(GetTitle(), "", GetTitle(), UISkinManager.GetSkin("MainMenuSkin"),
                    GetWidth(),
                    GetDialogElements());
            }
            Dialog = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Template, true,
                UISkinManager.GetSkin("MainMenuSkin"), isModal: false);
            UnityEngine.Object.DontDestroyOnLoad(Dialog);
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
                Dialog.Dismiss();
        }
    }
}