using System;

namespace LighterEdit.Utility
{
    public static class OrbitTools
    {
        /// <summary>
        /// Modifies the orbit of the given object, so that it orbits the its 
        /// reference body in a perfectly circular orbit
        /// </summary>
        public static void MakeSimpleOrbit(OrbitDriver driver, CelestialBody reference, Double altitude)
        {
            MakeOrbit(driver, reference, 0, 0, altitude, 0, 0, 0);
        }

        /// <summary>
        /// Makes a more complex orbit using multiple parameters
        /// </summary>
        public static void MakeOrbit(OrbitDriver driver, CelestialBody reference, Double inclination, Double eccentricy,
            Double altitude, Double LAN, Double argPe, Double mEp)
        {
            CelestialBody oldBody = driver.referenceBody;
            FlightGlobals.overrideOrbit = true;
            FlightGlobals.fetch.Invoke("disableOverride", 2f);
            if (driver.vessel != null)
            {
                driver.vessel.Landed = false;
                driver.vessel.Splashed = false;
                driver.vessel.SetLandedAt("");
                driver.vessel.KillPermanentGroundContact();
                driver.vessel.ResetGroundContact();
                FlightGlobals.currentMainBody = reference;
                OrbitPhysicsManager.SetDominantBody(reference);
            }

            // Pack vessels
            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                if (!vessel.packed)
                {
                    vessel.GoOnRails();
                }
            }

            // Disable inverse rotation
            foreach (CelestialBody body in PSystemManager.Instance.localBodies)
            {
                body.inverseRotation = false;
            }
            altitude = Math.Min(FlightGlobals.currentMainBody.sphereOfInfluence * 0.99, altitude);
            driver.orbit.SetOrbit(inclination, eccentricy, altitude, LAN, argPe, mEp, Planetarium.GetUniversalTime(),
                reference);
            driver.updateFromParameters();

            // Finalize Vessel Movement
            if (driver.vessel != null)
            {
                CollisionEnhancer.bypass = true;
                FloatingOrigin.SetOffset(driver.vessel.transform.position);
                OrbitPhysicsManager.CheckReferenceFrame();
                OrbitPhysicsManager.HoldVesselUnpack(10);
                if (reference != oldBody)
                {
                    GameEvents.onVesselSOIChanged.Fire(
                        new GameEvents.HostedFromToAction<Vessel, CelestialBody>(driver.vessel, oldBody,
                            reference));
                }
                driver.vessel.IgnoreGForces(20);
            }
        }

        public static Boolean IsValidSimpleOrbit(CelestialBody reference, Double altitude)
        {
            if (reference == null)
                return false;
            if (reference.atmosphere)
                return altitude > reference.atmosphereDepth;
            if (reference.pqsController != null)
                return altitude > reference.pqsController.radiusMax - reference.Radius;
            // What is this for a body
            return altitude > 0;
        }

        public static Double? GetSafeAltitude(CelestialBody body)
        {
            if (body.atmosphere)
            {
                return body.atmosphereDepth + 100;
            }
            if (body.pqsController != null)
            {
                return body.pqsController.radiusMax - body.Radius;
            }
            return null;
        }
    }
}