using Audio;
using UnityEngine;

class BlockCaveSpawner : BlockPlayerSign
{
    private readonly BlockActivationCommand[] cmds = new BlockActivationCommand[] { };

    public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
    {
            return new BlockActivationCommand[0];
    }


     public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
    {
        if (_ebcd == null)
            return;

        // Hide the sign, so its not visible. Without this, it errors out.
        _ebcd.bHasTransform = false;
        base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);

        // Re-show the transform. This won't have a visual effect, but fixes when you pick up the block, the outline of the block persists.
        _ebcd.bHasTransform = true;

    }

}

