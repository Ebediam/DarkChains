using UnityEngine;
using BS;
using System.Collections.Generic;

namespace DarkChains
{
    // This create an level module that can be referenced in the level JSON
    public class DarkChains : LevelModule
    {
        public static bool reseting = false;
        public bool activeScript;
        public List<Item> darkChains = new List<Item>();
        public List<RagdollHandle> chainedParts = new List<RagdollHandle>();


        public override void OnLevelLoaded(LevelDefinition levelDefinition)
        {
            activeScript = true;
        }

        public override void Update(LevelDefinition levelDefinition)
        {

            if (reseting)
            {
                reseting = false;
            }

            if (activeScript && Creature.player != null)
            {
                if(PlayerControl.handLeft.alternateUsePressed && PlayerControl.handLeft.usePressed && PlayerControl.handRight.alternateUsePressed && PlayerControl.handRight.usePressed)
                {


                    reseting = true;
                    
                    foreach (Item chain in darkChains)
                    {
                        if (chain != null)
                        {
                            Player.local.handLeft.bodyHand.telekinesis.TryRelease();
                            Player.local.handRight.bodyHand.telekinesis.TryRelease();
                            chain.GetComponent<ItemDarkChain>().isEnding = true;
                            DestroySelf.Destroy(chain.gameObject);
                        }

                    }
                    darkChains.Clear();

                    foreach (RagdollHandle ragdollHandle in chainedParts)
                    {
                        if (ragdollHandle != null)
                        {
                            foreach (RagdollPart part in ragdollHandle.ragdollPart.ragdoll.parts)
                            {
                                part.rb.isKinematic = false;
                            }

                            /*if (ragdollHandle.ragdollPart.ragdoll.state != BS.Ragdoll.State.Dead)
                            {
                                ragdollHandle.ragdollPart.ragdoll.SetState(BS.Ragdoll.State.Fallen);
                               
                            }*/
                            ragdollHandle.ragdollPart.ragdoll.allowStandUp = true;
                            ragdollHandle.name = "NPC";
                            ragdollHandle.ragdollPart.ragdoll.RefreshFall();
                                
                            ragdollHandle.tag = "Untagged";
                            ragdollHandle.ragdollPart.ragdoll.creature.name = "UncheckedNPC";
                        }

                    }
                    chainedParts.Clear();


                }

                if (Creature.list.Count > 0)
                {
                    foreach (Creature npc in Creature.list)
                    {
                        if (npc != Creature.player)
                        {
                            if (npc.ragdoll != null)
                            {
                                if (npc.ragdoll.parts.Count > 0)
                                {
                                    foreach (RagdollPart part in npc.ragdoll.parts)
                                    {
                                        if (part.handles.Count > 0)
                                        {
                                            foreach (RagdollHandle ragdollHandle in part.handles)
                                            {
                                                if (ragdollHandle.name == "HumanHeadSkull" || ragdollHandle.name == "HumanShoulder" || ragdollHandle.name == "NPC(chained)")
                                                {
                                                    //Not looking good
                                                }
                                                else
                                                {
                                                    if(ragdollHandle.name == "HumanHeadNeck")
                                                    {
                                                        ragdollHandle.name = "NPC(neck)";
                                                    }
                                                    else
                                                    {
                                                        ragdollHandle.name = "NPC";
                                                    }

                                                    ragdollHandle.tag = "Untagged";
                                                    

                                                    ragdollHandle.item = Item.list[0];
                                                    
                                                }




                                            }
                                        }

                                    }
                                }
                            }
                            
                           
                           

                        }
                    }

                }



                if(Player.local.handLeft.bodyHand.telekinesis != null)
                {
                    CheckTeleNPC(Player.local.handLeft.bodyHand.telekinesis);
                }
                if (Player.local.handRight.bodyHand.telekinesis != null)
                {
                    CheckTeleNPC(Player.local.handRight.bodyHand.telekinesis);
                }
            }   
        }


        public void CheckTeleNPC(Telekinesis telekinesis)
        {
            if (telekinesis.catchedHandle != null)
            {
                if (telekinesis.catchedHandle.tag == "Untagged")
                {
                    if (telekinesis.catchedHandle.name == "NPC" || telekinesis.catchedHandle.name == "NPC(neck)")
                    {
                        RagdollHandle rdHandle = telekinesis.catchedHandle as RagdollHandle;

                        if (rdHandle.ragdollPart != null)
                        {
                            
                            if (rdHandle.ragdollPart.ragdoll.state != BS.Ragdoll.State.Dead || true)
                            {
                                if (rdHandle.ragdollPart.ragdoll.state != BS.Ragdoll.State.Dead)
                                {
                                    rdHandle.ragdollPart.ragdoll.SetState(BS.Ragdoll.State.Fallen);
                                }

                                Item chain = Catalog.current.GetData<ItemData>("DarkChain", true).Spawn(true);
                                if(telekinesis.catchedHandle.name == "NPC(neck)")
                                {
                                    chain.gameObject.GetComponent<ItemDarkChain>().sliceable = false;
                                }
                                else
                                {
                                    chain.gameObject.GetComponent<ItemDarkChain>().sliceable = true;
                                }
                                rdHandle.ragdollPart.ragdoll.allowStandUp = false;
                                //rdHandle.ragdollPart.rb.isKinematic = true;
                                telekinesis.catchedHandle.name = "NPC(chained)";
                                telekinesis.catchedHandle.tag = "IgnoreTK";
                                


                                chainedParts.Add(rdHandle);
                                darkChains.Add(chain);
                                chain.transform.position = rdHandle.ragdollPart.transf.position;
                                chain.transform.rotation = rdHandle.ragdollPart.transf.rotation;
                                chain.gameObject.GetComponent<ItemDarkChain>().myRagdollPart = rdHandle.ragdollPart;
                                chain.gameObject.GetComponent<ItemDarkChain>().ragdollHandle = rdHandle;
                                

                                telekinesis.TryRelease();
                                telekinesis.targetHandle = chain.mainHandleRight;
                                telekinesis.TryCatch();

                            }
                        }


                    }
                }
            }
        }
     
        public static void DarkChainDestroy(Item chain, RagdollPart ragdollPart, RagdollHandle ragdollHandle, Telekinesis telekinesis)
        {

            telekinesis.TryRelease();
            telekinesis.other.TryRelease();
            DestroySelf.Destroy(chain.gameObject);

            foreach (RagdollPart part in ragdollPart.ragdoll.parts)
            {
                part.rb.isKinematic = false;
            }

            ragdollPart.ragdoll.SetState(BS.Ragdoll.State.Fallen);
            if(ragdollPart.ragdoll.creature.health.currentHealth <= 0)
            {
                ragdollPart.ragdoll.SetState(BS.Ragdoll.State.Dead);
            }


            ragdollHandle.name = "NPC";
            ragdollPart.ragdoll.RefreshFall();
            ragdollPart.ragdoll.allowStandUp = true;
            ragdollHandle.tag = "Untagged";
            ragdollPart.ragdoll.creature.name = "UncheckedNPC";


        }
        public override void OnLevelUnloaded(LevelDefinition levelDefinition)
        {

        }
    }
}
