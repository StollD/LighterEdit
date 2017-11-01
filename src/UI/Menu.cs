using System;
using System.Linq;
using System.Reflection;
using KSP.UI.Screens.DebugToolbar;

namespace LighterEdit.UI
{
    /// <summary>
    /// This class handles the menu window of LighterEdit
    /// </summary>
    public class Menu : Window<Menu>
    {
        /// <summary>
        /// Returns the title of the window
        /// </summary>
        public override String GetTitle()
        {
            return "LighterEdit";
        }

        /// <summary>
        /// Returns the width of the window
        /// </summary>
        public override Single GetWidth()
        {
            return 150f;
        }

        /// <summary>
        /// Returns the dialog elements that assemble the window
        /// </summary>
        protected override DialogGUIBase[] GetDialogElements()
        {
            return new DialogGUIBase[]
            {
                new DialogGUIVerticalLayout(GetWidth(), -1f,
                    Tooltip(new DialogGUIButton("Close all", OnCloseAll, false), "Closes all windows"),
                    Tooltip(new DialogGUIButton("Orbit Editor", OnOrbitEditor, false), "Opens the Orbit Editor window"),
                    Tooltip(new DialogGUIButton("Planet Editor", OnPlanetEditor, false),
                        "Opens the Planet Editor window"),
                    Tooltip(new DialogGUIButton("Ship Lander", OnShipLander, false), "Opens the Ship Lander window"),
                    Tooltip(new DialogGUIButton("Misc Tools", OnMiscTools, false), "Opens the Misc Tools window"),
                    Tooltip(new DialogGUIButton("KSP Debug Menu", OnDebugMenu, false),
                        "Opens the KSP Debug Toolbar (also available with Mod+F12)")
                ) {stretchWidth = true}
            };
        }

        /// <summary>
        /// Gets called when the "Close all" button is clicked
        /// Closes all windows that are currently open
        /// </summary>
        private void OnCloseAll()
        {
            Menu.Instance.Close();
            OrbitEditor.Instance.Close();
        }

        private void OnOrbitEditor()
        {
            // Open or close the orbit editor
            if (!OrbitEditor.Instance.IsOpen)
                OrbitEditor.Instance.Open();
            else
                OrbitEditor.Instance.ToggleVisibility();
        }

        private void OnPlanetEditor()
        {
            
        }
        
        private void OnShipLander()
        {
            
        }

        private void OnMiscTools()
        {
            
        }
        
        /// <summary>
        /// Gets called when the "KSP Debug Menu" button is clicked
        /// and opens the cheat toolbar
        /// </summary>
        private void OnDebugMenu()
        {
            DebugScreenSpawner spawner = UnityEngine.Object.FindObjectOfType<DebugScreenSpawner>();
            DebugScreen screen =
                typeof(DebugScreenSpawner).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .First(f => f.FieldType == typeof(DebugScreen))
                    .GetValue(spawner) as DebugScreen;
            screen?.Show();
        }
    }
}