using HarmonyLib;
using OldMoatGames;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SphereII_TileEntitySign_Gif
{
 //   [HarmonyPatch(typeof(TileEntity))]
    //[HarmonyPatch("OnDestroy")]
    //public class SphereII_TileEntity_OnDestroy
    //{
    //    public static void Postfix(TileEntity __instance)
    //    {
    //        if (__instance is TileEntitySign)
    //        {
    //            Vector3i blockPos = __instance.ToWorldPos();
    //            ChunkCluster chunkCluster = GameManager.Instance.World.ChunkClusters[0];
    //            if (chunkCluster == null)
    //            {
    //                return;
    //            }
    //            Chunk chunk = (Chunk)chunkCluster.GetChunkFromWorldPos(blockPos);
    //            if (chunk == null)
    //            {
    //                return;
    //            }
    //            BlockEntityData _ebcd = chunk.GetBlockEntity(blockPos);
    //            if (_ebcd != null)
    //            {
    //                ImageWrapper player = _ebcd.transform.GetComponent<ImageWrapper>();
    //                if (player != null)
    //                {
                       
    //                    player.Pause();
    //                  //  player.OnDestroy();
    //                }


    //                URL_texture uRL_Texture = _ebcd.transform.GetComponent<URL_texture>();
    //                if (uRL_Texture != null)
    //                {
    //                    uRL_Texture.OnDestroy();
    //                }
    //            }

    //        }
    //    }
    //}


    [HarmonyPatch(typeof(TileEntitySign))]
    //[HarmonyPatch("UpdateTick")]
    [HarmonyPatch("SetText")]
    public class SphereII_TileEntity_SetBlockEntityData
    {
        public static bool Prefix(TileEntitySign __instance, SmartTextMesh ___smartTextMesh, string _text)
        {
            if (GameManager.IsDedicatedServer)
                return true;

            if (___smartTextMesh == null)
            {
                return true;
            }
            if (_text.StartsWith("http"))
            {
                ImageWrapper wrapper = ___smartTextMesh.transform.parent.transform.GetComponent<ImageWrapper>();
                if (wrapper == null)
                    wrapper = ___smartTextMesh.transform.parent.transform.gameObject.AddComponent<ImageWrapper>();

                if (wrapper.IsNewURL(_text))
                {
                    wrapper.Pause();
                    wrapper.Init(_text);

                    __instance.SetModified();
                }
                ___smartTextMesh.gameObject.SetActive(false);
            }
            else
            {
                ImageWrapper wrapper = ___smartTextMesh.transform.parent.transform.GetComponent<ImageWrapper>();
                if (wrapper != null)
                {
                   
                //    wrapper.Reset();
                }
                    ___smartTextMesh.gameObject.SetActive(true);
            }

            return true;
        
        }
    }
}
