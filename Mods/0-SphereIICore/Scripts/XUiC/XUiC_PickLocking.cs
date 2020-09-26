using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class XUiC_PickLocking : XUiController
{
    public static string ID = "";
    public static GameObject lockPick = null;
    private XUiC_TextInput txtPassword;

    public override void Init()
    {
        XUiC_PickLocking.ID = windowGroup.ID;
        base.Init();
        txtPassword = (XUiC_TextInput)base.GetChildById("txtPassword");

        ((XUiC_SimpleButton)base.GetChildById("btnCancel")).OnPressed += BtnCancel_OnPressed;
        ((XUiC_SimpleButton)base.GetChildById("btnOk")).OnPressed += BtnOk_OnPressed;
    }

    private void BtnOk_OnPressed(XUiController _sender, OnPressEventArgs _onPressEventArgs)
    {

        GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, "Closing Lock");
        base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
    }

    private void BtnCancel_OnPressed(XUiController _sender, OnPressEventArgs _e)
    {
        base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
    }

    public override void OnOpen()
    {
        EntityPlayer player = base.xui.playerUI.entityPlayer;
        base.OnOpen();
        base.xui.playerUI.entityPlayer.PlayOneShot("open_sign", false);
        if ( lockPick == null )
        {
            GameObject temp = DataLoader.LoadAsset<GameObject>("#@modfolder(0-SphereIICore):Resources/LockPick.unity3d?Test");
            if (temp != null)
            {
                Debug.Log("Loaded Asset");
                Vector3 pos = new Vector3(10, 10, 10);
                lockPick = UnityEngine.Object.Instantiate<GameObject>(temp, xui.transform);
                if (lockPick != null)
                {
                    lockPick.transform.parent = base.xui.playerUI.uiCamera.transform;

                    lockPick.transform.position = pos;
//                    lockPick.transform.SetParent(base.xui.playerUI.uiCamera.transform);
                    Debug.Log("Lock Pick: " + lockPick.ToString());
                    lockPick.SetActive(true);
                    
                    foreach( Transform child in lockPick.transform )
                    {
                        Debug.Log("\t Transform: " + child.transform.ToString());
                    }
                   
                    Debug.Log("Position: " + lockPick.transform.position);
                    // Utils.SetLayerRecursively(lockPick, 11);
                }

            }
        }

    }

    public override void OnClose()
    {
        base.OnClose();
        base.xui.playerUI.entityPlayer.PlayOneShot("close_sign", false);
    }




}
