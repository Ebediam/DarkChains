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

        public WaveSpawner waveSpawner;

        public bool firstGrab;

        public override void OnLevelLoaded(LevelDefinition levelDefinition)
        {
            activeScript = true;
            
        }

        void OnWaveBegin()
        {
            ResetAll();
        }


        void CrateGrab(Interactor interactor, EventTime eventTime)
        {
            
            if (interactor.nearestInteractable)
            {
                
                Handle ragdollHandle = interactor.nearestInteractable as Handle;
                if (ragdollHandle.item)
                {
                    
                    ragdollHandle.item = null;
                   

                }

            }             
          }

        void CrateUngrab(Interactor interactor, EventTime eventTime)
        {
            if (interactor.grabbedHandle)
            {
                interactor.grabbedHandle.item = Item.list[0];
            }
        }


        public override void Update(LevelDefinition levelDefinition)
        {

            if(waveSpawner == null)
            {
                
                if(WaveSpawner.local != null)
                {
                    waveSpawner = WaveSpawner.local;
                    waveSpawner.OnWaveBeginEvent += OnWaveBegin;
                    
                }

            }


            if (reseting)
            {
                reseting = false;
            }

            if (activeScript && Creature.player != null)
            {
                if(PlayerControl.handLeft.alternateUsePressed && PlayerControl.handLeft.usePressed && PlayerControl.handRight.alternateUsePressed && PlayerControl.handRight.usePressed)
                {
                    ResetAll();
                                        
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
                                                if (ragdollHandle.name == "HumanHeadSkull" || ragdollHandle.name == "HumanShoulder" || ragdollHandle.name == "NPC(chained)" || ragdollHandle.name == "NPC(neck)(chained)")
                                                {
                                                    //Not looking good
                                                }
                                                else
                                                {


                                                    if (ragdollHandle.name == "NPC(neck)" || ragdollHandle.name == "NPC")
                                                    {

                                                    }
                                                    else
                                                    {
                                                        if(!ragdollHandle.item)
                                                        {
                                                            Debug.Log("Ragdoll handle didn't have an item attached");
                                                            ragdollHandle.item = Item.list[0];

                                                            ragdollHandle.Grabbed += CrateGrab;
                                                            ragdollHandle.UnGrabbed += CrateUngrab;
                                                            
                                                            
                                                            
                                                            foreach (CatalogData.Localization name in ragdollHandle.item.data.displayNames)
                                                            {
                                                                name.text = "NPC";
                                                            }
                                                        }

                                                    }

                                                    if (ragdollHandle.name == "HumanHeadNeck" || ragdollHandle.name == "NPC(neck)")
                                                    {
                                                        ragdollHandle.name = "NPC(neck)";
                                                    }
                                                    else
                                                    {
                                                        ragdollHandle.name = "NPC";
                                                    }

                                                    ragdollHandle.tag = "Untagged";






                                                    
                                                    
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


        public void ResetAll()
        {
            reseting = true;

            if (darkChains.Count > 0)
            {
                foreach (Item chain in darkChains)
                {
                    if (chain != null)
                    {

                        chain.GetComponent<ItemDarkChain>().isEnding = true;
                        if (chain.GetComponent<ItemDarkChain>().myRagdollPart != null)
                        {
                            chain.GetComponent<ItemDarkChain>().myRagdollPart.rb.isKinematic = false;
                        }

                    }

                }

            }

            if (chainedParts.Count > 0)
            {
                foreach (RagdollHandle ragdollHandle in chainedParts)
                {
                    
                    if (ragdollHandle != null)
                    {
                        if (ragdollHandle.ragdollPart != null)
                        {
                            if (ragdollHandle.ragdollPart.ragdoll != null)
                            {
                                if (ragdollHandle.ragdollPart.ragdoll.parts.Count > 0)
                                {
                                    foreach (RagdollPart part in ragdollHandle.ragdollPart.ragdoll.parts)
                                    {
                                        part.rb.isKinematic = false;
                                    }

                                    ragdollHandle.ragdollPart.ragdoll.allowStandUp = true;
                                    ragdollHandle.ragdollPart.ragdoll.RefreshFall();

                                    if (ragdollHandle.name == "NPC(neck)(chained)")
                                    {
                                        ragdollHandle.name = "NPC(neck)";
                                    }
                                    else if (ragdollHandle.name == "NPC(chained)")
                                    {
                                        ragdollHandle.name = "NPC";
                                    }
                                    else
                                    {
                                        Debug.Log("The ragdollHandle was neither the neck nor a default part");
                                    }
                                    ragdollHandle.tag = "Untagged";
                                    
                                }
                            }
                        }
                                                  
                            
                    }

                }
                chainedParts.Clear();
            }

            if (darkChains.Count > 0)
            {
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
                            Item chain = Catalog.current.GetData<ItemData>("DarkChain", true).Spawn(true);
                            
                            rdHandle.ragdollPart.ragdoll.allowStandUp = false;
                            
                            
                            chainedParts.Add(rdHandle);
                            darkChains.Add(chain);

                            chain.transform.position = rdHandle.ragdollPart.transf.position;
                            chain.transform.rotation = rdHandle.ragdollPart.transf.rotation;


                            if(chain.gameObject != null)
                            {
                                if(chain.gameObject.GetComponent<ItemDarkChain>() != null)
                                {
                                    chain.gameObject.GetComponent<ItemDarkChain>().myRagdollPart = rdHandle.ragdollPart;
                                    chain.gameObject.GetComponent<ItemDarkChain>().ragdollHandle = rdHandle;
                                    
                                }
                                
                            }
                            

                            if (telekinesis.catchedHandle.name == "NPC(neck)")
                            {
                                chain.gameObject.GetComponent<ItemDarkChain>().sliceable = false;
                                chain.gameObject.GetComponent<ItemDarkChain>().neck = true;
                                telekinesis.catchedHandle.name = "NPC(neck)(chained)";
                            }
                            else
                            {
                                chain.gameObject.GetComponent<ItemDarkChain>().sliceable = true;
                                chain.gameObject.GetComponent<ItemDarkChain>().neck = false;
                                telekinesis.catchedHandle.name = "NPC(chained)";
                            }

                            if (rdHandle.ragdollPart.ragdoll.state != BS.Ragdoll.State.Dead)
                            {
                                rdHandle.ragdollPart.ragdoll.SetState(BS.Ragdoll.State.Fallen);
                            }

                            chain.gameObject.GetComponent<ItemDarkChain>().Initialize();

                            Debug.Log("Catched handle name: " + telekinesis.catchedHandle.name);
                            
                            telekinesis.catchedHandle.tag = "IgnoreTK";

                            telekinesis.TryRelease();
                            telekinesis.targetHandle = chain.mainHandleRight;
                            telekinesis.TryCatch();

                            
                        }


                    }
                }
            }
        }
     
        public static void DarkChainDestroy(Item chain, RagdollPart ragdollPart, RagdollHandle ragdollHandle, Telekinesis telekinesis, bool neck)
        {
            telekinesis.TryRelease();

            DestroySelf.Destroy(chain.gameObject);

            if(ragdollPart.ragdoll.parts.Count > 0)
            {
                foreach (RagdollPart part in ragdollPart.ragdoll.parts)
                {
                    part.rb.isKinematic = false;
                }
            }
            
            if(ragdollHandle.ragdollPart.ragdoll.parts.Count > 0)
            {
                foreach (RagdollPart part in ragdollHandle.ragdollPart.ragdoll.parts)
                {
                    part.rb.isKinematic = false;
                }
            }


            ragdollPart.ragdoll.SetState(BS.Ragdoll.State.Fallen);
            if(ragdollPart.ragdoll.creature.health.currentHealth <= 0)
            {
                ragdollPart.ragdoll.SetState(BS.Ragdoll.State.Dead);
            }


            if (ragdollHandle.name == "NPC(neck)(chained)")
            {
                ragdollHandle.name = "NPC(neck)";
            }
            else if (ragdollHandle.name == "NPC(chained)")
            {
                ragdollHandle.name = "NPC";
            }
            else
            {
                Debug.Log("The ragdollHandle was neither the neck nor a default part");
            }
            //ragdollPart.ragdoll.RefreshFall();
            ragdollPart.ragdoll.allowStandUp = true;
            ragdollHandle.tag = "Untagged";

        }

        public static void DarkChainDestroy(Item chain, RagdollPart ragdollPart, RagdollHandle ragdollHandle, bool neck)
        {

            DestroySelf.Destroy(chain.gameObject);

            if (ragdollPart.ragdoll.parts.Count > 0)
            {
                foreach (RagdollPart part in ragdollPart.ragdoll.parts)
                {
                    part.rb.isKinematic = false;
                }
            }

            if (ragdollHandle.ragdollPart.ragdoll.parts.Count > 0)
            {
                foreach (RagdollPart part in ragdollHandle.ragdollPart.ragdoll.parts)
                {
                    part.rb.isKinematic = false;
                }
            }


            if (ragdollHandle.name == "NPC(neck)(chained)")
            {
                ragdollHandle.name = "NPC(neck)";
            }
            else if (ragdollHandle.name == "NPC(chained)")
            {
                ragdollHandle.name = "NPC";
            }
            else
            {
                Debug.Log("The ragdollHandle was neither the neck nor a default part");
            }

            //ragdollPart.ragdoll.RefreshFall();
            ragdollPart.ragdoll.allowStandUp = true;
            ragdollHandle.tag = "Untagged";

        }
        public override void OnLevelUnloaded(LevelDefinition levelDefinition)
        {

        }
    }
}
