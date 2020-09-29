﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lockpicking;
using UnityEngine;

public class SphereII_Locks
{
    public static GameObject LockPickAsset;
    public static GameObject lockPick;
    // transforms 
    List<String> transforms = new List<string>();

    public void Init()
    {
        if (lockPick == null)
        {
            String LockPrefab = Configuration.GetPropertyValue("AdvancedLockpicking", "LockPrefab");
            LockPickAsset = DataLoader.LoadAsset<GameObject>(LockPrefab );
            lockPick = UnityEngine.Object.Instantiate<GameObject>(LockPickAsset);
            Disable();
        }

        // Marked transforms
        transforms = new List<string>() { "Baseplate1", "Baseplate2", "ButtonInner", "ButtonInner", "ButtonOuter", "Padlock1_low" };
        transforms.AddRange(new List<String>() { "Padlock1_Latch_low", "Lock1Outer", "Lock2Outer", "Lock3Outer", "Lock1Inner", "Lock2Inner", "Lock3Inner" });

        if (lockPick.GetComponent<Keyhole>() == null)
        {
            // Populate the Keyhole
            Keyhole keyhole = lockPick.AddComponent<Keyhole>();
            keyhole.keyhole = FindTransform("Keyhole (Turnable)").gameObject;

            LockControls lockControl;
            if (lockPick.transform.parent != null)
            {
                lockControl = lockPick.transform.parent.gameObject.AddComponent<LockControls>();
              //  lockPick.transform.parent.gameObject.AddComponent<LockObjectRotation>();
            }
            else
            {
                lockControl = lockPick.transform.gameObject.AddComponent<LockControls>();
                
            }

            lockControl.lockpick = keyhole;

            // Lock Pick configuration
            keyhole.lockpickObject = lockPick.transform.FindInChilds("LockpickB (Turnable)").gameObject;
            keyhole.lockpickAnimator = FindTransform("LockpickB").GetComponent<Animator>();
            keyhole.lockpickAnimator.gameObject.SetActive(true);

            Camera cam = FindTransform("Cam2").GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.rect = new Rect(0.25f, 0.25f, 0.5f, 0.5f);
                LockObjectRotation lockObjectRotation = keyhole.lockpickObject.transform.gameObject.AddComponent<LockObjectRotation>();
                lockObjectRotation.uiCam = cam;
            }

            Transform padlock = FindTransform("Padlock1");
            if (padlock != null)
            {
                keyhole.padlock1 = padlock.gameObject;
                keyhole.audioPadlockJiggle = FindTransform("Audio Padlock Jiggle").gameObject.AddComponent<LocksetAudio>();
                keyhole.audioPadlockOpen = FindTransform("Audio Padlock Open").gameObject.AddComponent<LocksetAudio>();
            }
            // audio configuration
            keyhole.audioTurnClick = FindTransform("Audio Turn Click").gameObject.AddComponent<LocksetAudio>();
            keyhole.audioSqueek = FindTransform("Audio Squeek").gameObject.AddComponent<LocksetAudio>();
            keyhole.audioOpen = FindTransform("Audio Open").gameObject.AddComponent<LocksetAudio>();
            keyhole.audioJiggle = FindTransform("Audio Jiggle A").gameObject.AddComponent<LocksetAudio>();
            keyhole.audioJiggle2 = FindTransform("Audio Jiggle B").gameObject.AddComponent<LocksetAudio>();
            keyhole.audioJiggle3 = FindTransform("Audio Jiggle C").gameObject.AddComponent<LocksetAudio>();

            keyhole.audioLockpickBreak = FindTransform("Audio Lockpick Break").gameObject.AddComponent<LocksetAudio>();
            keyhole.audioLockpickEnter = FindTransform("Audio Lockpick Enter").gameObject.AddComponent<LocksetAudio>();
            keyhole.audioLockpickClick = FindTransform("Audio Lockpick Click").gameObject.AddComponent<LocksetAudio>();

            LockEmissive lockEmissive = lockPick.AddComponent<LockEmissive>();

            List<Renderer> lstRenders = new List<Renderer>();
            Renderer[] tempRender = new Renderer[12];
            Debug.Log("temp Render");


            foreach( String transform in transforms)
            {
                Transform temp = FindTransform(transform);
                if (temp)
                    lstRenders.Add(FindTransform(transform).GetComponent<MeshRenderer>());
            }
            Debug.Log("Setting Renders");
            lockEmissive.SetRenders( lstRenders.ToArray() );

            foreach (Transform child in lockPick.transform)
            {
                Debug.Log("\t Transform: " + child.transform.ToString() + " Active? " + child.gameObject.activeInHierarchy);
            }

        }
    }

    public void ToggleLock(bool padlock)
    {
        foreach (String transform in transforms)
        {
            Transform temp = FindTransform(transform);
            if (temp)
            {
                if (temp.name.Contains("Padlock"))
                    temp.GetComponent<MeshRenderer>().enabled = padlock;
                else
                    temp.GetComponent<MeshRenderer>().enabled = !padlock;
            }
        }

        //FindTransform("Padlock1_low").GetComponent<MeshRenderer>().enabled = padlock;
        //FindTransform("Padlock1_Latch_low").GetComponent<MeshRenderer>().enabled = padlock;

        //FindTransform("Baseplate1").GetComponent<MeshRenderer>().enabled = !padlock;
        //FindTransform("Baseplate2").GetComponent<MeshRenderer>().enabled = !padlock;
        //FindTransform("ButtonInner").GetComponent<MeshRenderer>().enabled = false;
        //FindTransform("ButtonOuter").GetComponent<MeshRenderer>().enabled = false;

        //FindTransform("Lock1Outer").GetComponent<MeshRenderer>().enabled = !padlock;
        //FindTransform("Lock2Outer").GetComponent<MeshRenderer>().enabled = !padlock;

        //FindTransform("Lock3Outer").GetComponent<MeshRenderer>().enabled = !padlock;
        //FindTransform("Lock1Inner").GetComponent<MeshRenderer>().enabled = !padlock;
        //FindTransform("Lock2Inner").GetComponent<MeshRenderer>().enabled = !padlock;
        //FindTransform("Lock3Inner").GetComponent<MeshRenderer>().enabled = !padlock;
    }
    public bool IsLockOpened()
    {
        if (lockPick != null)
            return lockPick.GetComponent<Keyhole>().LockIsOpen;
        return false;
    }

    public void SetPlayer(EntityPlayerLocal player)
    {
        if (lockPick != null)
        {
            lockPick.GetComponent<Keyhole>().player = player;

        }
    }
    public Transform FindTransform(String target)
    {
        return lockPick.transform.FindInChilds(target, false);
    }

    //private void AttachScript(String Script, String Transform)
    //{
    //    Type type = Type.GetType(Script + ", Mods");
    //    if (type == null)
    //        return;

    //    Transform temp = FindTransform(Transform);
    //    if (temp != null)
    //    {
    //        temp.gameObject.AddComponent(type);
    //    }
    //}

    public void Enable()
    {
        if (lockPick != null)
        {
            lockPick.SetActive(true);

            //GameRandom random = new GameRandom();
            //if (random.RandomRange(0, 2) <= 1f)
            //    ToggleLock(true);
            //else
            //    ToggleLock(false);
        }
    }
    public void Disable()
    {
        if (lockPick != null)
        {
            lockPick.SetActive(false);
        }
    }
}
