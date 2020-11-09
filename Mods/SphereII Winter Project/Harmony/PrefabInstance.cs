using DMT;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/*
   public class PrefabInstance
   {
	   public PrefabInstance(int _id, string _filename, Vector3i _position, byte _rotation, Prefab _bad, int _standaloneBlockSize)
       {
       }
   }
*/
public class SphereII_WinterProject
{
    public class SphereII_WinterProject_Init : IHarmony
    {
        public void Start()
        {
            var harmony = new Harmony(GetType().ToString());

            // Navezgane only - Since it's pregenerated, it uses a different prefabs loading, with preset locations. This will adjust the prefabs for only navezgane.
            var original = typeof(PrefabInstance).GetConstructor(new Type[] { typeof(int), typeof(PathAbstractions.AbstractedLocation), typeof(Vector3i), typeof(byte), typeof(Prefab), typeof(int) });
            var postfix = typeof(SphereII_WinterProject_PrefabInstance).GetMethod("PrefabInstance_Prefix");
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));

            // Random Gen
            harmony.PatchAll();
        }
    }

    // Navezgane only - Since it's pregenerated, it uses a different prefabs loading, with preset locations. This will adjust the prefabs for only navezgane.
    public class SphereII_WinterProject_PrefabInstance
    {
        public static void PrefabInstance_Prefix(ref PrefabInstance __instance, ref Prefab _bad, Vector3i _position)
        {
  
            return;
        }
    }

    [HarmonyPatch(typeof(DynamicPrefabDecorator))]
    [HarmonyPatch("AddPrefab")]
    public class SphereII_DynamicPrefabDecorator
    {
        public static bool Prefix(DynamicPrefabDecorator __instance, PrefabInstance _pi)
        {


            if (_pi.prefab.size.y < 11)
                return false;

            // Prefabs with too great of an offset should be removed.
            // Example: Size y size 30 with an offset of -25 would only be 5 above terrain; not visible.
            if (_pi.prefab.size.y - Math.Abs(_pi.prefab.yOffset) < 11)
                return false;

            if (_pi.prefab.PrefabName.Contains("trader_hugh"))
                return true;

            // Check if the current thread has a name. the GenerateWorlds for RWG has a named thread; the others do not.
            if (Thread.CurrentThread.Name != null)
                return true;

            // Sink the prefab into the ground
            // This also sinks the SleeperVolumes, so they work as expected in clear quests.
            _pi.boundingBoxPosition.y -= 8;
            return true;
        }

    }
    [HarmonyPatch(typeof(Prefab))]
    [HarmonyPatch("readBlockData")]
    public class SphereII_WinterProject_readBlockData
    {
        public static bool Postfix(bool __result, ref Prefab __instance, ref List<string> ___allowedZones)
        {
            if (__result)
            {
                if (!__instance.PrefabName.Contains("trader_hugh"))
                {
                    __instance.bTraderArea = false;
                    __instance.bExcludeDistantPOIMesh = true;
                    __instance.bCopyAirBlocks = true;
                }
            }
            return __result;
        }

    }

    [HarmonyPatch(typeof(Prefab))]
    [HarmonyPatch("CopyIntoLocal")]
    public class SphereII_WinterProject_Prefab_Prefix
    {

        public static void Postfix(Prefab __instance, Vector3i _destinationPos, ChunkCluster _cluster, QuestTags _questTags)
        {
            if (__instance.Tags.Test_AllSet(POITags.Parse("SKIP_HARMONY_COPY_INTO_LOCAL")))
                return;

            if (!__instance.PrefabName.Contains("trader_hugh"))
                WinterModPrefab.SetSnowPrefab(__instance, _cluster, _destinationPos, _questTags);
        }

    }

    [HarmonyPatch(typeof(PrefabInstance))]
    [HarmonyPatch("CopyIntoChunk")]
    public class SphereII_WinterProject_PrefabInstance_CopyIntoChunk
    {
        public static void Postfix(PrefabInstance __instance, Chunk _chunk)
        {
            if (!__instance.prefab.PrefabName.Contains("trader_hugh"))
                WinterModPrefab.SetSnowChunk(_chunk, __instance.boundingBoxPosition, __instance.boundingBoxSize);
        }
    }


}
