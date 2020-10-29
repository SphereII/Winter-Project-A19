using DMT;
using HarmonyLib;
using System;
using System.Collections.Generic;
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
            harmony.Patch(original, postfix:new HarmonyMethod(postfix));

            // Random Gen
            harmony.PatchAll();
        }
    }

     // Navezgane only - Since it's pregenerated, it uses a different prefabs loading, with preset locations. This will adjust the prefabs for only navezgane.
    public class SphereII_WinterProject_PrefabInstance
    {
        public static void PrefabInstance_Prefix(ref PrefabInstance __instance, ref Prefab _bad, Vector3i _position)
        {
            //Debug.Log(" Before Bounding Box: " + __instance.boundingBoxPosition);
            //if (!_bad.PrefabName.Contains("trader_hugh"))
            //    __instance.boundingBoxPosition.y -= 8;
            //Debug.Log(" After Bounding Box: " + __instance.boundingBoxPosition);
            //// No longer needed, but left as an example
            return;
        }
    }

    [HarmonyPatch(typeof(DynamicPrefabDecorator))]
    [HarmonyPatch("AddPrefab")]
    public class SphereII_DynamicPrefabDecorator
    {
        public static bool Prefix( DynamicPrefabDecorator __instance, PrefabInstance _pi)
        {
            if (_pi.prefab.size.y < 10)
                return false;

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
                //if (__instance.size.y < 10)
                //{
                //    ___allowedZones.Clear();
                //    ___allowedZones.Add("DevOnly");
                //    //Debug.Log("\n**************");
                //    //Debug.Log("Winter Project Prefab Filter : " + __instance.PrefabName + " yOffset: " + __instance.yOffset + " Size: " + __instance.size.ToString());
                //    //Debug.Log("Disabling POI that is too short. Expect the next line to be a WRN about it. Ignore it. ");
                //    //return false;
                //}
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



    //[HarmonyPatch(typeof(Prefab))]
    //[HarmonyPatch("CopyBlocksIntoChunkNoEntities")]
    //public class SphereII_WinterProject_Prefab_Prefix_CopyBlocksIntoChunkNoEntities
    //{
    //    public static bool Prefix(ref Prefab __instance, ref Vector3i _prefabTargetPos)
    //    {
    //        // If they are pre-generated Winter Project worlds, don't apply this. They'd have already been applied.
    //        if (GamePrefs.GetString(EnumGamePrefs.GameWorld).ToLower().Contains("winter project"))
    //            return true;

    //        if (!__instance.PrefabName.Contains("trader_hugh"))
    //        {
    //            __instance.bTraderArea = false;
    //            __instance.bExcludeDistantPOIMesh = true;
    //            __instance.bCopyAirBlocks = true;
    //        }
    //        return true;

    //    }

    //}

    [HarmonyPatch(typeof(Prefab))]
    [HarmonyPatch("CopyIntoLocal")]
    public class SphereII_WinterProject_Prefab_Prefix
    {
        //public static bool Prefix(Prefab __instance, ref Vector3i _destinationPos, ChunkCluster _cluster, QuestTags _questTags)
        //{
        //    // If they are pre-generated Winter Project worlds, don't apply this. They'd have already been applied.
        //    if (GamePrefs.GetString(EnumGamePrefs.GameWorld).ToLower().Contains("winter project"))
        //        return true;

        //    if (__instance.Tags.Test_AllSet(POITags.Parse("SKIP_HARMONY_COPY_INTO_LOCAL")))
        //        return true;

        //  //  __instance.bExcludeDistantPOIMesh = true;

        //    return true;
        //}

        public static void Postfix(Prefab __instance, Vector3i _destinationPos, ChunkCluster _cluster, QuestTags _questTags)
        {
            if (__instance.Tags.Test_AllSet(POITags.Parse("SKIP_HARMONY_COPY_INTO_LOCAL")))
                return;

          //  __instance.bExcludeDistantPOIMesh = true;

            if (!__instance.PrefabName.Contains("trader_hugh"))
                WinterModPrefab.SetSnowPrefab(__instance, _cluster, _destinationPos, _questTags);
        }

    }

    //[HarmonyPatch(typeof(Prefab))]
    //[HarmonyPatch("LoadXMLData")]
    //public class SphereII_WinterProject_Prefab_LoadXMLData
    //{
    //    public static void Postfix(bool __result, ref Prefab __instance)
    //    {
    //        if (__result)
    //        {
    //            if (!__instance.PrefabName.Contains("trader_hugh"))
    //            {
    //                __instance.bExcludeDistantPOIMesh = true;
    //            }
    //        }
    //    }

    //}

    [HarmonyPatch(typeof(PrefabInstance))]
    [HarmonyPatch("CopyIntoChunk")]
    public class SphereII_WinterProject_PrefabInstance_CopyIntoChunk
    {
        public static void Postfix(PrefabInstance __instance, Chunk _chunk)
        {
         //   __instance.prefab.bExcludeDistantPOIMesh = true;

            if (!__instance.prefab.PrefabName.Contains("trader_hugh"))
                WinterModPrefab.SetSnowChunk(_chunk, __instance.boundingBoxPosition, __instance.boundingBoxSize);
        }
    }


}
