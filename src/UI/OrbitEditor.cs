using System;
using System.Collections.Generic;
using LighterEdit.Utility;
using TMPro;
using UnityEngine;

namespace LighterEdit.UI
{
    /// <summary>
    /// Renders a window for editing the orbit of a vessel or a body
    /// </summary>
    public class OrbitEditor : Window<OrbitEditor>
    {
        public enum Mode
        {
            Simple,
            Complex,
            Graphical
        }
        
        /// <summary>
        /// The orbit we are currently editing
        /// </summary>
        public OrbitDriver Current;

        /// <summary>
        /// The mode of the editor interface that gets rendered
        /// </summary>
        public Mode DisplayMode;
        
        /// <summary>
        /// Returns the title of the window
        /// </summary>
        public override String GetTitle()
        {
            return "Orbit Editor";
        }

        /// <summary>
        /// Returns the width of the window
        /// </summary>
        public override Single GetWidth()
        {
            return 375f;
        }

        /// <summary>
        /// Initialize the Editor
        /// </summary>
        public OrbitEditor()
        {
            DisplayMode = Mode.Simple;
            OnClose += () => DisplayMode = Mode.Simple;
        }

        /// <summary>
        /// Returns the dialog elements that assemble the window
        /// </summary>
        protected override DialogGUIBase[] GetDialogElements()
        {
            // Orbit Selection
            DialogGUIVerticalLayout layout = new DialogGUIVerticalLayout(GetWidth(), -1f,
                new DialogGUIButton(() => "Currently editing: " + GetOrbitDriverName(Current, false), () =>
                    Selector<OrbitDriver>.Instance.ShowSelector(GetOrbitDrivers(true), c =>
                    {
                        Current = c;
                        Redraw();
                    }, s => GetOrbitDriverName(s)), () => true, -1, -1, false)
            ) { stretchWidth = true };
            
            // Mode selector
            if (Current != null)
            {
                layout.AddChild(new DialogGUIHorizontalLayout(true, true, new DialogGUIToggleGroup(
                    new DialogGUIToggleButton(() => DisplayMode == Mode.Simple, "Simple", (b) =>
                    {
                        if (!b) return;
                        DisplayMode = Mode.Simple;
                        Redraw();
                    }, h: 30f),
                    new DialogGUIToggleButton(() => DisplayMode == Mode.Complex, "Complex", (b) =>
                    {
                        if (!b) return;
                        DisplayMode = Mode.Complex;
                        Redraw();
                    }, h: 30f),
                    new DialogGUIToggleButton(() => DisplayMode == Mode.Graphical, "Graphical", (b) =>
                    {
                        if (!b) return;
                        DisplayMode = Mode.Graphical;
                        Redraw();
                    }, h: 30f)
                )));
            }
            else
            {
                return new DialogGUIBase[] {layout};
            }
            
            // Reference body selector
            CelestialBody reference = Current.referenceBody;
            DialogGUIButton referenceBodySelector = new DialogGUIButton(
                () => "Reference body: " + reference.bodyDisplayName.Replace("^N", ""),
                () => Selector<OrbitDriver>.Instance.ShowSelector(GetOrbitDrivers(false),
                    c => reference = c.celestialBody, s => GetOrbitDriverName(s)), () => true, -1, -1, false);
            
            // Simple Orbit Editor, only altitude and reference body selection
            if (DisplayMode == Mode.Simple)
            {
                // Variables
                Double altitude = 0;
                Double? altitudeSetter = null;

                layout.AddChildren(new DialogGUIBase[]
                {
                    new DialogGUILabel(" Altitude:", true, true),
                    OnUpdate(
                        Tooltip(new DialogGUITextInput("", "Please enter a number", false, 20,
                            // ReSharper disable once ConditionalTernaryEqualBranch
                            s => !Double.TryParse(s, out altitude) ? s : s, 25f), "Altitude of circular orbit"),
                        dialog =>
                        {
                            if (altitudeSetter.HasValue)
                            {
                                dialog.uiItem.GetComponent<TMP_InputField>().text = altitudeSetter.Value.ToString();
                                altitude = altitudeSetter.Value;
                            }
                            altitudeSetter = null;
                        }
                    ),
                    referenceBodySelector,
                    Tooltip(new DialogGUIButton("Set to current orbit",
                            () => altitudeSetter = Current.orbit.semiMajorAxis - Current.orbit.referenceBody.Radius,
                            () => Current.orbit == null && (Current.vessel == null || Current.vessel.Landed), false),
                        "Sets all the fields of the editor to reflect the orbit of the currently selected vessel"),
                    Tooltip(new DialogGUIButton("Set to safe orbit",
                            () => altitudeSetter = OrbitTools.GetSafeAltitude(reference),
                            () => reference.atmosphere || reference.pqsController != null, false),
                        "Sets the altitude to the lowest value that is considered safe."),
                    Tooltip(new DialogGUIButton("Apply",
                        () => OrbitTools.MakeSimpleOrbit(Current, reference, altitude + reference.Radius),
                        () => OrbitTools.IsValidSimpleOrbit(reference, altitude), false), "Sets the orbit"),
                });
            }
            return new DialogGUIBase[] {layout};
        }

        /// <summary>
        /// Returns the name of the object the orbit driver is attached to
        /// </summary>
        private String GetOrbitDriverName(OrbitDriver driver, Boolean color = true)
        {
            if (driver == null)
                return "Nothing";
            if (driver.celestialBody != null)
            {
                String name = driver.celestialBody.bodyDisplayName.Replace("^N", "");
                if (!color) return name;
                if (FlightGlobals.currentMainBody == driver.celestialBody ||
                    PlanetariumCamera.fetch?.target.celestialBody == driver.celestialBody)
                {
                    name += " [<color=" + XKCDColors.HexFormat.AquaBlue + ">+</color>]";
                }
                if (driver == Current)
                {
                    name += " [<color=" + XKCDColors.HexFormat.OrangeYellow + ">+</color>]";
                }
                return name;
            }
            if (driver.vessel != null)
            {
                String name = driver.vessel.vesselName;
                if (!color) return name;
                if (FlightGlobals.ActiveVessel?.id == driver.vessel.id ||
                    PlanetariumCamera.fetch?.target.vessel?.id == driver.vessel.id)
                {
                    name += " [<color=" + XKCDColors.HexFormat.AcidGreen + ">+</color>]";
                }
                if (driver == Current)
                {
                    name += " [<color=" + XKCDColors.HexFormat.OrangeYellow + ">+</color>]";
                }
                return name;
            }
            return driver.name;
        }

        /// <summary>
        /// Creates a list of all orbit drivers that are currently active
        /// </summary>
        private static List<OrbitDriver> GetOrbitDrivers(Boolean includeVessels)
        {
            List<OrbitDriver> drivers = new List<OrbitDriver>();
            if (includeVessels)
            {
                for (Int32 i = 0; i < FlightGlobals.Vessels.Count; i++)
                {
                    drivers.Add(FlightGlobals.Vessels[i].orbitDriver);
                }
            }
            for (Int32 i = 1; i < PSystemManager.Instance.localBodies.Count; i++)
            {
                drivers.Add(PSystemManager.Instance.localBodies[i].orbitDriver);
            }
            return drivers;
        }
    }
}