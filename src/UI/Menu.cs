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
                    new DialogGUIButton("Close all", OnCloseAll, false),
                    new DialogGUIButton("Orbit Editor", OnOrbitEditor, false),
                    new DialogGUIButton("Planet Editor", OnPlanetEditor, false),
                    new DialogGUIButton("Ship Lander", OnShipLander, false),
                    new DialogGUIButton("Misc Tools", OnMiscTools, false),
                    new DialogGUIButton("KSP Debug Menu", OnDebugMenu, false)
                ) { stretchWidth = true }
            };
        }

        /// <summary>
        /// Gets called when the "Close all" button is clicked
        /// Closes all windows that are currently open
        /// </summary>
        private void OnCloseAll()
        {
            Menu.Instance.Close();
        }

        private void OnOrbitEditor()
        {
            
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