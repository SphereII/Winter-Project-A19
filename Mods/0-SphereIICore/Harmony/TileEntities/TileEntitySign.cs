using HarmonyLib;
using OldMoatGames;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SphereII_TileEntitySign_Gif
{

    [HarmonyPatch(typeof(TileEntitySecureLootContainerSigned))]
    [HarmonyPatch("SetText")]
    public class SphereII_TileEntitySecureLootContainerSigned_SetText
    {
        public static bool Prefix(TileEntitySign __instance, SmartTextMesh[] ___smartTextMesh, string _text)
        {
            if (GameManager.IsDedicatedServer)
                return true;

            if (___smartTextMesh == null)
            {
                return true;
            }
            if (_text.StartsWith("http"))
            {
                for (int i = 0; i < ___smartTextMesh.Length; i++)
                {
                    Debug.Log("Component: " + ___smartTextMesh[i].transform.name);
                    ImageWrapper wrapper = ___smartTextMesh[i].transform.parent.transform.GetComponent<ImageWrapper>();
                    if (wrapper == null)
                        wrapper = ___smartTextMesh[i].transform.parent.transform.gameObject.AddComponent<ImageWrapper>();

                    if (wrapper.IsNewURL(_text))
                    {
                        wrapper.Pause();
                        Debug.Log("Initializing ImageWrapper: " + _text + "  Transform: " + ___smartTextMesh[i].transform.name);
                        wrapper.Init(_text, ___smartTextMesh[i].transform.name);

                        __instance.SetModified();
                    }
                    ___smartTextMesh[i].gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < ___smartTextMesh.Length; i++)
                {
                    ImageWrapper wrapper = ___smartTextMesh[i].transform.parent.transform.GetComponent<ImageWrapper>();
                    if (wrapper != null)
                    {

                        //    wrapper.Reset();
                    }
                    ___smartTextMesh[i].gameObject.SetActive(true);
                }
            }

            return true;

        }
    }


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
