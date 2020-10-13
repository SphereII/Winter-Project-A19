using HarmonyLib;
using OldMoatGames;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SphereII_TileEntitySign_Gif
{


    [HarmonyPatch(typeof(TileEntitySign))]
    [HarmonyPatch("SetText")]
    public class SphereII_TileEntitySign_SetText
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
