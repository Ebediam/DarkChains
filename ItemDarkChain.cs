using UnityEngine;
using BS;

namespace DarkChains
{
    // The item module will add a unity component to the item object. See unity monobehaviour for more information: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    // This component will apply a force on the player rigidbody to the direction of an item transform when the trigger is pressed (see custom reference in the item definition component of the item prefab)
    public class ItemDarkChain : MonoBehaviour
    {
        public ItemModuleDarkChain module;
        public Item item;
        public RagdollPart myRagdollPart;
        public RagdollHandle ragdollHandle;
        public Creature NPC;
        public bool neck;
        public bool isEnding;
        public bool teleUngrabbing = false;
        public AudioSource chokeSFX;
        public bool choking;
        public BrainData brain;

        public RagdollPart leftHand;
        public RagdollPart rightHand;
        



        public bool sliced;

        public bool sliceable;


        protected void Awake()
        {
            item = this.GetComponent<Item>();
            module = item.data.GetModule<ItemModuleDarkChain>();
            item.disallowDespawn = true;
            item.OnTeleUnGrabEvent += OnTeleUnGrabAction;
            item.OnTeleGrabEvent += OnTeleGrabAction;
            item.GetComponentInChildren<MeshRenderer>().enabled = false;


            if (!module.showChains)
            {
                item.GetComponentInChildren<ParticleSystem>().Stop();
            }

            


        }
        public void Initialize()
        {
            if (myRagdollPart)
            {
                NPC = myRagdollPart.ragdoll.creature;
                brain = NPC.brain;


                foreach (RagdollPart part in NPC.ragdoll.parts)
                {
                    if (part.name == "Part_LeftHand")
                    {
                        leftHand = part;
                    }
                    if (part.name == "Part_RightHand")
                    {
                        rightHand = part;
                    }
                }

                if (module.choking)
                {
                
                    if (NPC.umaCharacter.GetSex() == UMAPreset.Sex.Male)
                    {

                        if (item.transform.Find("MaleChokeSFX"))
                        {

                            chokeSFX = item.transform.Find("MaleChokeSFX").GetComponent<AudioSource>();


                        }
                        else
                        {
                            Debug.Log("AudioSource not found");
                        }

                    }
                    else if (NPC.umaCharacter.GetSex() == UMAPreset.Sex.Female)
                    {

                        chokeSFX = item.transform.Find("FemaleChokeSFX").GetComponent<AudioSource>();
                    }
                    else
                    {
                        Debug.Log("NPC is neither male or female");
                    }
                }
            }
            else
            {
                Debug.Log("MyRagdollPart doesn't exist!");
            }
        }

        public void OnTeleGrabAction(Handle handle, Telekinesis teleGrabber)
        {
            teleUngrabbing = false;

            if (item.rb.isKinematic == true)
            {
                item.rb.isKinematic = false;
            }

            if (neck && NPC.state != Creature.State.Dead && module.choking)
            {
                chokeSFX.Stop();
                chokeSFX.Play();
                choking = true;
                NPC.OnChoke(true);
                NPC.speak.enabled = false;
                NPC.speak.speakLoudness = 0f;
                

                if (NPC.GetHeldobject(Side.Left))
                {
                    if(NPC.GetHeldobject(Side.Left).handles.Count > 0)
                    {
                        NPC.GetHeldobject(Side.Left).handles[0].handlers[0].UnGrab(false);

                    }
                }

                if (NPC.GetHeldobject(Side.Right))
                {
                    if (NPC.GetHeldobject(Side.Right).handles.Count > 0)
                    {
                        NPC.GetHeldobject(Side.Right).handles[0].handlers[0].UnGrab(false);

                    }
                }
            }


        }


        public void OnTeleUnGrabAction(Handle handle, Telekinesis teleGrabber)
        {
            if (!teleUngrabbing)
            {
                teleUngrabbing = true;
                item.rb.isKinematic = true;
                if (!module.freezeEnemiesInTheAir)
                {
                    teleGrabber.TryRelease();
                    isEnding = true;
                    DarkChains.DarkChainDestroy(item, myRagdollPart, ragdollHandle, teleGrabber, neck);

                }

                if (neck && module.choking)
                {
                    
                    chokeSFX.Stop();
                    choking = false;
                    NPC.OnChoke(false);
                    NPC.speak.enabled = true;
                    NPC.speak.speakLoudness = 1f;


                }
            }
           
        }

       

        void FixedUpdate()
        {
            if (module.choking)
            {
                if (choking && !chokeSFX.isPlaying)
                {
                    NPC.health.Kill();
                }
                if (choking && NPC.state != Creature.State.Dead)
                {
                    if (Vector3.Distance(item.transform.position, rightHand.transf.position) > 0.1f)
                    {
                        rightHand.transf.position = Vector3.Lerp(rightHand.transf.position, item.transform.position, 0.1f);

                    }
                    if (Vector3.Distance(item.transform.position, leftHand.transf.position) > 0.1f)
                    {
                        leftHand.transf.position = Vector3.Lerp(leftHand.transf.position, item.transform.position, 0.1f);

                    }

                    if (Vector3.Distance(leftHand.transf.position, rightHand.transf.position) > 0.1f)
                    {
                        rightHand.transf.position = Vector3.Lerp(rightHand.transf.position, leftHand.transform.position, 0.02f);
                        leftHand.transf.position = Vector3.Lerp(leftHand.transf.position, rightHand.transform.position, 0.02f);
                    }

                }

                if (NPC.state == Creature.State.Dead && chokeSFX.isPlaying)
                {
                    chokeSFX.Stop();
                }
            }
            

            if(!myRagdollPart.ragdoll.creature)
            {
                DarkChains.DarkChainDestroy(item, myRagdollPart, ragdollHandle, neck);
            }
            else
            {                                
                if (Vector3.Distance(myRagdollPart.transf.position, myRagdollPart.ragdoll.creature.body.hipsBone.transform.position) > 1.6f && !sliced && sliceable)
                {
                    myRagdollPart.ragdoll.creature.health.currentHealth -= 2000f;
                    if (myRagdollPart.ragdoll.creature.health.currentHealth <= 0f)
                    {
                        myRagdollPart.ragdoll.creature.health.Kill();
                    }

                    if (myRagdollPart.partData.bone.ToString() != "Head")
                    {
                        myRagdollPart.ragdoll.Slice(myRagdollPart.partData.bone);
                    }
                }

                
                if (item.isTeleGrabbed && module.freezeEnemiesInTheAir)
                {
                    CheckForDestroy(PlayerControl.handLeft);
                    CheckForDestroy(PlayerControl.handRight);
                }

                
                if (myRagdollPart.rb.isKinematic == false && !isEnding && !DarkChains.reseting)
                {
                    myRagdollPart.rb.isKinematic = true;
                }
                else
                {
                    myRagdollPart.rb.isKinematic = false;
                }

                
                if (ragdollHandle != null && myRagdollPart != null && item != null && item.gameObject != null && !isEnding && !DarkChains.reseting)
                {

                    if (myRagdollPart.transf.position != item.transform.position)
                    {
                        myRagdollPart.transf.position = item.transform.position;
                        myRagdollPart.transform.rotation = item.transform.rotation;
                    }
                }
            }

           
        }
        void CheckForDestroy(PlayerControl.Hand hand)
        {            
            if(hand.alternateUsePressed && hand.usePressed)
            {
                
                isEnding = true;
                Telekinesis tele;
                if(hand.side == Side.Left)
                {
                    tele = Player.local.handLeft.bodyHand.telekinesis;
                }
                else
                {
                    tele = Player.local.handRight.bodyHand.telekinesis;
                }

                DarkChains.DarkChainDestroy(item, myRagdollPart, ragdollHandle, tele, neck);

            }
        }
        

        
    }
}