using System;
using System.Collections.Generic;
using UnityEngine;

class BlockTeleportDMT : Block
{
    String prefabName = "";
    Vector3 destination = new Vector3();

    public override BlockFace getInventoryFace()
    {
        return BlockFace.Top;
    }
    public void TeleportPlayer( EntityPlayer player, Vector3 position)
    {

        position.y =+ 1;

        // If the player is remote, meaning, if the player is connected to a server, let the server do the teleport.
        if (player.isEntityRemote)
        {
            SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(player.entityId).SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(position, null, false));
            return;
        }

        // Is there a vehicle attached? Send it as well.
        if (player.AttachedToEntity != null)
        {
            player.AttachedToEntity.SetPosition(position, true);
            return;
        }


        // Change the palyer's position
        player.SetPosition(position, true);
        if (player is EntityPlayer)
        {
            player.Respawn(RespawnType.Teleport);
        }
    }
    
    // This gets triggered when an entity talks on it.
    public override void OnEntityWalking(WorldBase _world, int _x, int _y, int _z, BlockValue _blockValue, Entity entity)
    {
        // If they aren't a player, don't teleport them
        if (entity is EntityPlayer == false)
            return;

        // This will read if you have a   <property name="Prefab" value="abandoned_house_01" />
        if (this.Properties.Values.ContainsKey("Prefab"))
            this.prefabName = this.Properties.Values["Prefab"];

        // This will read if you have a   <property name="Destination" value="4323,43,234" />
        if (this.Properties.Values.ContainsKey("Destination"))
            this.destination = StringParsers.ParseVector3(this.Properties.Values["Destination"], 0, -1);

        // make sure its an actual player.
        EntityPlayer entityPlayer = entity as EntityPlayer;
        if (entityPlayer == null)
            return;

        if (!String.IsNullOrEmpty(this.prefabName))
        {
            TeleportToPrefab(_world, entityPlayer);
            return;
        }

        if (this.destination != Vector3.zero)
        {
            Vector3 position = this.destination;

            // Make sure the player spawns a bit higher than the ground.
            position.y = GameManager.Instance.World.GetTerrainHeight((int)position.x, (int)position.z) + 1;

            TeleportPlayer(entityPlayer, this.destination);
            return;
        }
    }

    // Searches for the prefab, and then teleports the player there.
    public void TeleportToPrefab(WorldBase _world, EntityPlayer entityPlayer)
    {
        Vector3 position = Vector3.zero;
        if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
        {
            // Search for the prefab in the global listing.
            PrefabInstance randomPOINearWorldPos = GameManager.Instance.GetDynamicPrefabDecorator().GetPOIPrefabs().Find(instance => instance.prefab.location.Name.ToLower().Contains(this.prefabName.ToLower()));
            if (randomPOINearWorldPos != null)
            {
                // Find the center of the prefab
                Vector2 vector = new Vector2(randomPOINearWorldPos.boundingBoxPosition.x + randomPOINearWorldPos.boundingBoxSize.x / 2f, randomPOINearWorldPos.boundingBoxPosition.z + randomPOINearWorldPos.boundingBoxSize.z / 2f);
                if (vector.x == -0.1f && vector.y == -0.1f)
                    return;

                position = randomPOINearWorldPos.boundingBoxPosition.ToVector3CenterXZ();

                // Find the terrain height so we plop the player there.
                // Bump them one block higher
                position.y = GameManager.Instance.World.GetTerrainHeight((int)position.x, (int)position.z) + 1;

                // If there's a Rally Point, use that as a destination.
                Vector3i prefabPosition = new Vector3i(position);
                Vector3i rallyPoint = GetRallyPosition(GameManager.Instance.World, prefabPosition, randomPOINearWorldPos.prefab.size);
                if (rallyPoint == Vector3i.zero)
                    TeleportPlayer(entityPlayer, position);
                else
                    TeleportPlayer(entityPlayer, rallyPoint.ToVector3());
                return;
            }
        }
    
        return;
    }

    // Code from Vanillia to scan a prefab to find the rally flag.
    public Vector3i GetRallyPosition(World _world, Vector3i _prefabPosition, Vector3i _prefabSize)
    {
        int num = World.toChunkXZ(_prefabPosition.x - 1);
        int num2 = World.toChunkXZ(_prefabPosition.x + _prefabSize.x + 1);
        int num3 = World.toChunkXZ(_prefabPosition.z - 1);
        int num4 = World.toChunkXZ(_prefabPosition.z + _prefabSize.z + 1);
        Rect rect = new Rect((float)_prefabPosition.x, (float)_prefabPosition.z, (float)_prefabSize.x, (float)_prefabSize.z);
        for (int i = num; i <= num2; i++)
        {
            for (int j = num3; j <= num4; j++)
            {
                Chunk chunk = _world.GetChunkSync(i, j) as Chunk;
                if (chunk != null)
                {
                    List<Vector3i> list = chunk.IndexedBlocks["Rally"];
                    if (list != null)
                    {
                        for (int k = 0; k < list.Count; k++)
                        {
                            Vector3 vector = chunk.ToWorldPos(list[k]).ToVector3();
                            if (rect.Contains(new Vector2(vector.x, vector.z)))
                            {
                                return chunk.ToWorldPos(list[k]);
                            }
                        }
                    }
                }
            }
        }
        return Vector3i.zero;
    }
}

