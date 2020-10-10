
using UnityEngine;
using System;
using UnityEngine.Video;

using GUI_2;
using System.Collections.Generic;
using Audio;

public class BlockSamaraWell : BlockDoor
{
    String OnActivatedBuff = "";
    private BlockActivationCommand[] cmds = new BlockActivationCommand[]
    {
        new BlockActivationCommand("Free Samara", "hand", true),
    };

    public BlockSamaraWell()
    {
        this.HasTileEntity = true;
        // SoundSource is the referenced SoundDataNode
        if (this.Properties.Values.ContainsKey("OnActivatedBuff"))
            this.OnActivatedBuff = this.Properties.Values["OnActivatedBuff"];

    }

    // Display custom messages for turning on and off the music box, based on the block's name.
    public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
    {
        return "Free Samara From The Well";
    }


    public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
    {
        this.cmds[0].enabled = true;
        return this.cmds;
    }

    // Play the music when its activated. We stop the sound broadcasting, in case they want to restart it again; otherwise we can get two sounds playing.
    public override bool OnBlockActivated(int _indexInBlockActivationCommands, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _player)
    {
        this.DamageBlock(_world, _cIdx, _blockPos, _blockValue, this.MaxDamage, _player.entityId, false, false);
        _player.Buffs.AddBuff(OnActivatedBuff);
        return false;
    }
}

