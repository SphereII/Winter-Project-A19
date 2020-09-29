using HarmonyLib;
using System;

//OnBlockActivated(int _indexInBlockActivationCommands, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _player)

public class SphereII_Blocks_BlockSecureLoot
{

    private static readonly string AdvFeatureClass = "AdvancedLockpicking";
    private static readonly string Feature = "AdvancedLocks";

    [HarmonyPatch(typeof(BlockSecureLoot))]
    [HarmonyPatch("OnBlockActivated")]
    [HarmonyPatch(new Type[] { typeof(int), typeof(WorldBase), typeof(int), typeof(Vector3i), typeof(BlockValue), typeof(EntityPlayer) })]

    public class SphereII_Block_Init
    {
        public static bool Prefix(ref Block __instance, int _indexInBlockActivationCommands, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _player
            , string ___lockPickItem)
        {
            // Check if this feature is enabled.
            if (!Configuration.CheckFeatureStatus(AdvFeatureClass, Feature))
                return true;
            if (_blockValue.ischild)
            {
                return true;
            }

            TileEntitySecureLootContainer tileEntitySecureLootContainer = _world.GetTileEntity(_cIdx, _blockPos) as TileEntitySecureLootContainer;
            if (tileEntitySecureLootContainer == null)
            {
                return false;
            }

            if ( _indexInBlockActivationCommands < 4)
            {
                // Check if the player has lock picks.
                LocalPlayerUI playerUI = (_player as EntityPlayerLocal).PlayerUI;
                ItemValue item = ItemClass.GetItem(___lockPickItem, false);
                if (playerUI.xui.PlayerInventory.GetItemCount(item) == 0)
                {
                    playerUI.xui.CollectedItemList.AddItemStack(new ItemStack(item, 0), true);
                    GameManager.ShowTooltip(_player as EntityPlayerLocal, Localization.Get("ttLockpickMissing"));
                    return false;
                }

                LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_player as EntityPlayerLocal);
                XUiC_PickLocking.Open(uiforPlayer, tileEntitySecureLootContainer, _blockValue, _blockPos);
                return false;
            }
            return true;
        }
    }

}