using Lockpicking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class XUiC_PickLocking : XUiController
{
    public static string ID = "";
    SphereII_Locks Lock ;
    
    ILockable LockedItem;
    BlockValue currentBlock;
    Vector3i blockPos;
    public override void Init()
    {
        Lock = new SphereII_Locks();
        XUiC_PickLocking.ID = windowGroup.ID;
        base.Init();
        Lock.Init();
    }

    public override void Update(float _dt)
    {
        base.Update(_dt);
        if ( Lock.IsLockOpened() && this.LockedItem != null )
        {
            this.LockedItem.SetLocked(false);
            OnClose();
        }
    }

    // Set the container reference so we can unlock it.
    public static void Open(LocalPlayerUI _playerUi, ILockable _lockedItem, BlockValue _blockValue, Vector3i _blockPos)
    {
        _playerUi.xui.FindWindowGroupByName(XUiC_PickLocking.ID).GetChildByType<XUiC_PickLocking>().LockedItem = _lockedItem;
        _playerUi.xui.FindWindowGroupByName(XUiC_PickLocking.ID).GetChildByType<XUiC_PickLocking>().currentBlock = _blockValue;
        _playerUi.xui.FindWindowGroupByName(XUiC_PickLocking.ID).GetChildByType<XUiC_PickLocking>().blockPos = _blockPos;
        _playerUi.windowManager.Open(XUiC_PickLocking.ID, true, false, true);
    }

    // Set the player reference and display the lock.
    public override void OnOpen()
    {
        EntityPlayer player = base.xui.playerUI.entityPlayer;
        base.OnOpen();
        Lock.SetPlayer(player as EntityPlayerLocal);
        

        Lock.Enable();
        base.xui.playerUI.entityPlayer.PlayOneShot("open_sign", false);


    }

    public override void OnClose()
    {
        Lock.Disable();
        
        base.OnClose();
        if ( Lock.IsLockOpened() )
        {
            Block.list[currentBlock.type].OnBlockActivated(GameManager.Instance.World, 0, blockPos, currentBlock, base.xui.playerUI.entityPlayer as EntityAlive);
        }
                    this.LockedItem = null;
        base.xui.playerUI.windowManager.Close(XUiC_PickLocking.ID);
        base.xui.playerUI.entityPlayer.PlayOneShot("close_sign", false);

    }




}
