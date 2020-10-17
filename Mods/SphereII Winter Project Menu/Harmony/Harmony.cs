//using DMT;
//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using UnityEngine;

//public class SphereII_MainMenu
//{
//    public class SphereII_MainMenuHooks : IHarmony
//    {
//        public void Start()
//        {
//            Debug.Log(" Loading Patch: " + this.GetType().ToString());
//            var harmony = new Harmony(GetType().ToString());
//            harmony.PatchAll(Assembly.GetExecutingAssembly());
//        }
//    }

//    //    [HarmonyPatch(typeof(XUiC_MainMenu))]
//    //    [HarmonyPatch("OnOpen")]
//    //    public class SphereII_MainMenu_Init
//    //    {


//    //        // Attach the script to the center button.
//    //        public static void Postfix()
//    //        {

//    //            //    GameObject.Find("btnConnectToServer").AddComponent<SphereII_MenuScript>();

//    //            foreach (GameObject temp in GameObject.FindObjectsOfType<GameObject>())
//    //            {
//    //                Debug.Log(temp.name);
//    //            }
//    //            GameObject myMenuObject = new GameObject();
//    //            ParticleSystem myParticleSystem;

//    //            String strMyAssetBundle = "#@modfolder(SphereII Winter Project Main Menu):Resources/MenuParticles.unity3d?SnowSoftNoGround";
//    //            Debug.Log(" Asset Bundle: " + strMyAssetBundle);
//    //            if (strMyAssetBundle.IndexOf('#') == 0 && strMyAssetBundle.IndexOf('?') > 0)
//    //            {
//    //                GameObject temp = DataLoader.LoadAsset<GameObject>(strMyAssetBundle);
//    //                if (temp != null)
//    //                {
//    //                    Debug.Log("Game Object loaded");
//    //                    // We need to instantiate the object to have it come into effect. We'll attach it to the button as a focus point.
//    //                    myMenuObject = UnityEngine.Object.Instantiate<GameObject>(temp, GameObject.Find("menuBackground").transform);
//    //                    // In order to display on the UI, the layer of the game object must be 12.
//    //                    myMenuObject.layer = 12;
//    //                    myMenuObject.SetActive(true);
//    //                    myParticleSystem = myMenuObject.GetComponent<ParticleSystem>();
//    //                    ParticleSystem.MainModule temp2 = myParticleSystem.main;
//    //                    temp2.maxParticles = 10000000;
//    //                    temp2.loop = true;
//    //                    // myParticleSystem.SetActive(true);
//    //                }

//    //                GameObject temp3 = DataLoader.LoadAsset<GameObject>("#@modfolder(Samara):Resources/SamaraCurse.unity3d?Cutscene00");
//    //                if (temp3 != null)
//    //                {
//    //                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(temp, GameObject.Find("Camera").transform);
//    //                    if (gameObject != null)
//    //                    {
//    //                        Vector3 local_offset = new Vector3(0f, 0f, 0f);


//    //                        Vector3 local_rotation = new Vector3(0f, 0f, 0f);
//    //                        Debug.Log("Samara Asset Loaded");
//    //                        gameObject.SetActive(true);
//    //                        Utils.SetLayerRecursively(gameObject, gameObject.transform.parent.gameObject.layer);
//    //                    }

//    //                }
//    //            }


//    //        }
//    //    }
//    //}


//    [HarmonyPatch(typeof(XUiC_MainMenu))]
//    [HarmonyPatch("OnOpen")]
//    public class SphereII_MainMenu_Init
//    {

//        // Attach the script to the center button.
//        public static void Postfix(XUiC_SimpleButton ___btnContinueGame)
//        {

//            Transform target = ___btnContinueGame.Button.UiTransform;
//            ImageWrapper wrapper = target.parent.transform.GetComponent<ImageWrapper>();
//            if (wrapper == null)
//            {
//                Debug.Log("Adding ImageWrapper to: " + target.name);
//                MeshRenderer render = target.transform.gameObject.AddComponent<MeshRenderer>();
//                render.name = "MyNewRenderer";
//                 target.transform.gameObject.AddComponent<Camera>();

//                wrapper = target.parent.transform.gameObject.AddComponent<ImageWrapper>();
//                wrapper.Pause();
//                wrapper.Init("https://i.imgur.com/RAHTcqP.jpg");

//            }
//            //foreach (var component in target.GetComponentsInChildren<Component>())
//            //{
//            //    Debug.Log("Component: " + component.ToString() + " " + component.gameObject.layer + " " + component.transform.position);
//            //}

//            ////    GameObject.Find("btnConnectToServer").AddComponent<SphereII_MenuScript>();

//            //foreach (Component temp in GameObject.FindObjectsOfType<Component>())
//            //{
//            //    foreach (var component in temp.GetComponentsInChildren<Renderer>())
//            //    {
//            //        Debug.Log("\tComponent: " + component.ToString() + " : " + temp.ToString() + " " + component.transform.position);
//            //    }
//            //}
//                //    //if (temp.name == "mainMenu")
//                //    //    break;
//                //}


//                //GameObject myMenuObject = new GameObject();
//                //ParticleSystem myParticleSystem;

//                //String strMyAssetBundle = "#@modfolder(0-SphereIICore):Resources/LockPick.unity3d?Button";
//                //Debug.Log(" Asset Bundle: " + strMyAssetBundle);
//                //if (strMyAssetBundle.IndexOf('#') == 0 && strMyAssetBundle.IndexOf('?') > 0)
//                //{
//                //    GameObject temp = DataLoader.LoadAsset<GameObject>(strMyAssetBundle);
//                //    if (temp != null)
//                //    {
//                //        Debug.Log("Game Object loaded");
//                //        // We need to instantiate the object to have it come into effect. We'll attach it to the button as a focus point.
//                //        myMenuObject = UnityEngine.Object.Instantiate<GameObject>(temp, GameObject.Find("btnConnectToServer").transform);
//                //        myMenuObject.transform.parent = GameObject.Find("btnConnectToServer").transform;

//                //        // In order to display on the UI, the layer of the game object must be 12.
//                //        myMenuObject.layer = 12;
//                //        myMenuObject.SetActive(true);
//                //        //myParticleSystem = myMenuObject.GetComponent<ParticleSystem>();
//                //        //ParticleSystem.MainModule temp2 = myParticleSystem.main;
//                //        //temp2.maxParticles = 10000000;
//                //        //temp2.loop = true;
//                //        // myParticleSystem.SetActive(true);
//                //    }

//                //    //GameObject temp3 = DataLoader.LoadAsset<GameObject>("#@modfolder(Samara):Resources/SamaraCurse.unity3d?Cutscene00");
//                //    //if (temp3 != null)
//                //    //{
//                //    //    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(temp, GameObject.Find("Camera").transform);
//                //    //    if (gameObject != null)
//                //    //    {
//                //    //        Vector3 local_offset = new Vector3(0f, 0f, 0f);


//                //    //        Vector3 local_rotation = new Vector3(0f, 0f, 0f);
//                //    //        Debug.Log("Samara Asset Loaded");
//                //    //        gameObject.SetActive(true);
//                //    //        Utils.SetLayerRecursively(gameObject, gameObject.transform.parent.gameObject.layer);
//                //    //    }

//                //    //}
//                //}


//            }
//    }
//}



