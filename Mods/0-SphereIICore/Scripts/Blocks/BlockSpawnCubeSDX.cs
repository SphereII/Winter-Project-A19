using Audio;
using System;
using UnityEngine;

class BlockSpawnCubeSDX : BlockPlayerSign
{
    public string SpawnGroup;
    public string Task = "Wander";

    private BlockActivationCommand[] cmds = new BlockActivationCommand[]
{
    new BlockActivationCommand("edit", "pen", true)

};

    public override bool OnBlockActivated(int _indexInBlockActivationCommands, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _player)
    {
        if (_blockValue.ischild)
        {
            Vector3i parentPos = Block.list[_blockValue.type].multiBlockPos.GetParentPos(_blockPos, _blockValue);
            BlockValue block = _world.GetBlock(parentPos);
            return this.OnBlockActivated(_indexInBlockActivationCommands, _world, _cIdx, parentPos, block, _player);
        }
        TileEntitySign tileEntitySign = _world.GetTileEntity(_cIdx, _blockPos) as TileEntitySign;
        if (tileEntitySign == null)
        {
            return false;
        }
        switch (_indexInBlockActivationCommands)
        {
            case 0:
                return this.OnBlockActivated(_world, _cIdx, _blockPos, _blockValue, _player);

            default:
                return false;
        }
    }
    public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
    {
        if (_world.IsEditor() || _entityFocusing.IsGodMode.Value)
            return base.GetActivationText(_world, _blockValue, _clrIdx, _blockPos, _entityFocusing);
        else
            return "";
    }
    //private BlockActivationCommand[] cmds = new BlockActivationCommand[0];
    public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
    {
        TileEntitySign tileEntitySign = (TileEntitySign)_world.GetTileEntity(_clrIdx, _blockPos);
        if (tileEntitySign == null)
        {

            Debug.Log("No Sign");
            return new BlockActivationCommand[0];
        }
        if (_world.IsEditor() || _entityFocusing.IsGodMode.Value)
        {
            //Debug.Log("IsEditor or GodMod is true");
            //string @string = GamePrefs.GetString(EnumGamePrefs.PlayerId);
            //PersistentPlayerData playerData = _world.GetGameManager().GetPersistentPlayerList().GetPlayerData(tileEntitySign.GetOwner());

            //bool flag = !tileEntitySign.IsOwner(@string) && (playerData != null && playerData.ACL != null) && playerData.ACL.Contains(@string);
            //cmds[0].enabled = true;
            //cmds[1].enabled = (!tileEntitySign.IsLocked() && (tileEntitySign.IsOwner(@string) || flag));
            //cmds[2].enabled = (tileEntitySign.IsLocked() && tileEntitySign.IsOwner(@string));
            //cmds[3].enabled = ((!tileEntitySign.IsUserAllowed(@string) && tileEntitySign.HasPassword() && tileEntitySign.IsLocked()) || tileEntitySign.IsOwner(@string));
            return cmds;
        }

        //        Debug.Log("No commands.");
        return new BlockActivationCommand[0];
    }

    public string GetValue(String signText, String key)
    {
        foreach (String text in signText.Split(';'))
        {
            string[] parse = text.Split('=');
            if (parse.Length == 2)
            {
                if (parse[0].ToLower() == key.ToLower())
                    return parse[1];
            }
        }
        return "";
    }

    public string SetValue(String signText, String key, String value)
    {
        String newSign = "";
        // If the sign doesn't have the key, then just add it, and return it.
        if (!signText.Contains(key + "="))
        {
            signText += ";" + key + "=" + value;
            return signText;
        }

        // Loop through the text
        foreach (String text in signText.Split(';'))
        {
            string[] parse = text.Split('=');
            if (parse.Length == 2)
            {
                if (parse[0].ToLower() == key.ToLower())
                    parse[1] = value;

                newSign += parse[0] + "=" + parse[1];
            }
        }
        return newSign;
    }
    public void CheckForSpawn(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
    {
        Chunk chunk = (Chunk)((World)_world).GetChunkFromWorldPos(_blockPos);
        TileEntitySign tileEntitySign = (TileEntitySign)_world.GetTileEntity(_clrIdx, _blockPos);
        if (tileEntitySign == null)
            return;

        string signText = tileEntitySign.GetText();
        String entityClassID = GetValue(signText, "entityid");

        // If there's already an entityID, check 
        if (!String.IsNullOrEmpty(entityClassID))
        {
            // make sure its an int.
            if (StringParsers.TryParseSInt32(entityClassID, out int entityid))
            {
                // Check if the entity is still spawned, and if so, don't respawn.
                Entity spawnedEntity = GameManager.Instance.World.GetEntity(entityid);
                if (spawnedEntity != null)
                    return;
            }

        }

        Entity myEntity = null;
        string Task = "Wander";

        // entityclass:zombieWightFeral;task:wander
        Debug.Log("SignText: " + signText);
        if (string.IsNullOrEmpty(signText))
            return;

        try
        {
            // Read the entity class
            String entityClass = GetValue(signText, "entityclass");
            if (String.IsNullOrEmpty(entityClass))
                entityClass = GetValue(signText, "ec");

            // no entity name, no spawn.
            if (String.IsNullOrEmpty(entityClass))
                return;

            Task = GetValue(signText, "task");
            if (String.IsNullOrEmpty(Task))
                Task = "Wander";

            myEntity = EntityFactory.CreateEntity(EntityClass.FromString(entityClass), _blockPos.ToVector3());
            if (myEntity == null)
                return;

            // Update the sign with the new entity ID.
            String newSign = SetValue(signText, "entityid", myEntity.entityId.ToString());
            tileEntitySign.SetText(newSign);

            GameManager.Instance.World.SpawnEntityInWorld(myEntity);
            if (Task.ToLower() == "stay")
                EntityUtilities.SetCurrentOrder(myEntity.entityId, EntityUtilities.Orders.Stay);
            if (Task.ToLower() == "patrol")
                EntityUtilities.SetCurrentOrder(myEntity.entityId, EntityUtilities.Orders.Patrol);
            if (Task.ToLower() == "wander")
                EntityUtilities.SetCurrentOrder(myEntity.entityId, EntityUtilities.Orders.Wander);


        }
        catch (Exception ex)
        {
            Debug.Log("Invalid String on Sign: " + signText + " Example:  ec=zombieBoe;task=Wander");
            return;
        }



    }

    public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
    {
        Debug.Log("OnBlockAdded()");
        base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue);
        CheckForSpawn(_world, _chunk.ClrIdx, _blockPos, _blockValue);
    }
    public override void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
    {
        Debug.Log("OnBlockLoaded()");
        base.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);
        CheckForSpawn(_world, _clrIdx, _blockPos, _blockValue);
    }

    public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
    {
        if (_ebcd == null)
            return;

        // Hide the sign, so its not visible. Without this, it errors out.
        //   _ebcd.bHasTransform = false;
        base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);

        // Re-show the transform. This won't have a visual effect, but fixes when you pick up the block, the outline of the block persists.
        //  _ebcd.bHasTransform = true;

    }

}

