using DMT;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SphereII_Spook
{
    private static readonly string AdvFeatureClass = "Theme";
    private static readonly string Feature = "Spook";


    // Constant Blood Moon
    [HarmonyPatch(typeof(SkyManager))]
    [HarmonyPatch("BloodMoon")]
    public class SphereII_CaveProject_SkyManager_BloodMoon
    {
        public static bool Prefix(ref bool __result)
        {
            if (!Configuration.CheckFeatureStatus(AdvFeatureClass, Feature))
                return true;

            SkyManager.SetSunIntensity(0.3f);
            __result = true;
            return false;
        }
    }


}