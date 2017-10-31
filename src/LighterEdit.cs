using LighterEdit.UI;
using UnityEngine;

namespace LighterEdit
{
    /// <summary>
    /// The main MonoBehaviour. Here we create 
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class LighterEdit : MonoBehaviour
    {
        void Start()
        {
            // Stop the Garbage Collector
            DontDestroyOnLoad(this);
        }

        void Update()
        {
            // Check if the user wants to open the UI
            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.L))
            {
                // Open or close the main window
                if (!Menu.Instance.IsOpen)
                    Menu.Instance.Open();
                else
                    Menu.Instance.ToggleVisibility();
            }
        }
    }
}