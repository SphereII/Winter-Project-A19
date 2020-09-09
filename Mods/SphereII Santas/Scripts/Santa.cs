using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000014 RID: 20
internal class SantaTrader : EntityNPC
{
    // Token: 0x060000A5 RID: 165 RVA: 0x00007280 File Offset: 0x00006280
    public override void Init(int _entityClass)
    {
        base.Init(_entityClass);
        EntityClass entityClass = EntityClass.list[this.entityClass];
        if (entityClass.Properties.Values.ContainsKey(EntityClass.PropParticleOnDeath))
        {
            this.particleOnDeath = entityClass.Properties.Values[EntityClass.PropParticleOnDeath];
        }
    }

    // Token: 0x060000A6 RID: 166 RVA: 0x000072E0 File Offset: 0x000062E0
    public override void OnUpdateLive()
    {
        if (this.questList == null)
        {
            base.PopulateQuestList();
        }
        if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
            return;

   //     if (Steam.Network.IsServer)
        {
            if (this.traderArea == null)
            {
                this.traderArea = this.world.GetTraderAreaAt(new Vector3i(this.position));
            }
            if (this.traderArea != null && Time.time > this.updateTime)
            {
                this.updateTime = Time.time + 1f;
                List<Entity> entitiesInBounds = GameManager.Instance.World.GetEntitiesInBounds(this, new Bounds(this.position, Vector3.one * 10f));
                if (entitiesInBounds.Count > 0)
                {
                    for (int i = 0; i < entitiesInBounds.Count; i++)
                    {
                        if (entitiesInBounds[i] is EntityNPC)
                        {
                            if (entitiesInBounds[i].entityId < this.entityId)
                            {
                                this.IsDespawned = true;
                                this.MarkToUnload();
                                break;
                            }
                        }
                        else if (entitiesInBounds[i] is EntityPlayer)
                        {
                            EntityPlayer entityPlayer = entitiesInBounds[i] as EntityPlayer;
                            if (base.CanSee(entityPlayer))
                            {
                                if (this.GreetingDictionary.ContainsKey(entityPlayer))
                                {
                                    if (Time.time < this.GreetingDictionary[entityPlayer])
                                    {
                                        this.GreetingDictionary[entityPlayer] = Time.time + SantaTrader.traderTalkDelayTime;
                                    }
                                    this.GreetingDictionary[entityPlayer] = Time.time + SantaTrader.traderTalkDelayTime;
                                }
                                else
                                {
                                    this.GreetingDictionary.Add(entityPlayer, Time.time + SantaTrader.traderTalkDelayTime);
                                }
                                Debug.Log("playing initial greeting");
                                this.PlayVoiceSetEntry("greeting", false, true);
                            }
                        }
                    }
                }
            }
        }
    }

    // Token: 0x060000A7 RID: 167 RVA: 0x0000751C File Offset: 0x0000651C
    public virtual void PlayVoiceSetEntry(string name, bool ignoreTime, bool showReactionAnim)
    {
        name = "santa" + name;
        Debug.Log("Checking Sound: " + name);
        if (this.lastVoiceTime - Time.time < 0f || ignoreTime)
        {
            string text = name.ToLower();
            if (this.lastSoundPlayed != string.Empty)
            {
                base.StopOneShot(this.lastSoundPlayed);
                this.lastSoundPlayed = string.Empty;
            }
            Debug.Log("playing Sound from Sound Groupd: " + text);
            base.PlayOneShot(text);
            this.lastSoundPlayed = text;
            AvatarController avatarController = this.emodel.avatarController;
            if (avatarController)
            {
                Debug.Log("Sending Animation Trigger: " + text);
                avatarController.SetTrigger(name);
            }
            if (!ignoreTime)
            {
                this.lastVoiceTime = Time.time + 5f;
            }
        }
    }

    // Token: 0x060000A8 RID: 168 RVA: 0x0000760F File Offset: 0x0000660F
    public new void PlayAnimReaction(EntityNPC.AnimReaction reaction)
    {
    }

    // Token: 0x060000A9 RID: 169 RVA: 0x00007612 File Offset: 0x00006612
    public override void ProcessDamageResponseLocal(DamageResponse _dmResponse)
    {
    }

    // Token: 0x060000AA RID: 170 RVA: 0x00007618 File Offset: 0x00006618
    public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float _impulseScale)
    {
        this.MarkToUnload();
        int result;
        if (this.world.IsRemote())
        {
            Debug.Log("World is remote, not spawning.");
            result = 1;
        }
        else
        {
            if (this.particleOnDeath != null && this.particleOnDeath.Length > 0)
            {
                EntityClass entityClass = EntityClass.list[this.entityClass];
                this.AddParticle(entityClass);
            }
            if (this.isGameMessageOnDeath())
            {
                GameManager.Instance.GameMessage(EnumGameMessages.EntityWasKilled, this, this.entityThatKilledMe);
            }
            int classID = 0;
            Entity entity = EntityFactory.CreateEntity(EntityGroups.GetRandomFromGroup("santaspawn", ref classID), this.position);
            if (entity == null)
            {
                Debug.Log("No bad santa spawn");
                result = 1;
            }
            else
            {
                Vector3i blockPos = default(Vector3i);
                blockPos.x = (int)this.position.x;
                blockPos.y = (int)this.position.y;
                blockPos.z = (int)this.position.z;
                this.world.SetBlockRPC(blockPos, BlockValue.Air);
                entity.SetSpawnerSource(EnumSpawnerSource.StaticSpawner);
                GameManager.Instance.World.SpawnEntityInWorld(entity);
                result = 0;
            }
        }
        return result;
    }

    // Token: 0x060000AB RID: 171 RVA: 0x0000775C File Offset: 0x0000675C
    public void AddParticle(EntityClass entityClass)
    {
        GameObject gameObject = DataLoader.LoadAsset<GameObject>(this.particleOnDeath);
        if (gameObject == null)
        {
            Debug.Log("Could not load particle: " + this.particleOnDeath);
        }
        else
        {
            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
            if (gameObject2 == null)
            {
                Debug.Log("Could not instantiate the Game object");
            }
            else
            {
                foreach (ParticleSystem particleSystem in gameObject2.GetComponentsInChildren<ParticleSystem>())
                {
                    particleSystem.transform.parent = this.emodel.GetModelTransform();
                    if (entityClass.particleOnSpawn.shapeMesh != null && entityClass.particleOnSpawn.shapeMesh.Length > 0)
                    {
                        SkinnedMeshRenderer[] componentsInChildren2 = base.GetComponentsInChildren<SkinnedMeshRenderer>();
                        ParticleSystem.ShapeModule shape = particleSystem.shape;
                        shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
                        string text = entityClass.particleOnSpawn.shapeMesh.ToLower();
                        if (text.Contains("setshapetomesh"))
                        {
                            text = text.Replace("setshapetomesh", string.Empty);
                            int num = int.Parse(text);
                            if (num >= 0 && num < componentsInChildren2.Length)
                            {
                                shape.skinnedMeshRenderer = componentsInChildren2[num];
                                ParticleSystem[] componentsInChildren3 = particleSystem.transform.GetComponentsInChildren<ParticleSystem>();
                                if (componentsInChildren3 != null)
                                {
                                    for (int j = 0; j < componentsInChildren3.Length; j++)
                                    {
                                        shape = componentsInChildren3[j].shape;
                                        shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
                                        shape.skinnedMeshRenderer = componentsInChildren2[num];
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // Token: 0x0400008B RID: 139
    private bool firstTime = true;

    // Token: 0x0400008C RID: 140
    private float updateTime;

    // Token: 0x0400008D RID: 141
    private float greetingTime;

    // Token: 0x0400008E RID: 142
    private bool warningPlayed;

    // Token: 0x0400008F RID: 143
    private float lastVoiceTime;

    // Token: 0x04000090 RID: 144
    private string lastSoundPlayed;

    // Token: 0x04000091 RID: 145
    private static float traderTalkDelayTime = 90f;

    // Token: 0x04000092 RID: 146
    private string particleOnDeath;
}
