using UnityEngine;
using BS;

namespace DarkChains
{
    // The item module will add a unity component to the item object. See unity monobehaviour for more information: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    // This component will apply a force on the player rigidbody to the direction of an item transform when the trigger is pressed (see custom reference in the item definition component of the item prefab)
    public class ItemDarkChain : MonoBehaviour
    {
        protected Item item;
        public RagdollPart myRagdollPart;
        public RagdollHandle ragdollHandle;
        public bool isEnding;

        public bool sliced;

        public bool sliceable;


        protected void Awake()
        {
            item = this.GetComponent<Item>();
            item.disallowDespawn = true;
            item.OnTeleUnGrabEvent += OnTeleUnGrabAction;
            item.OnTeleGrabEvent += OnTeleGrabAction;
            item.GetComponentInChildren<MeshRenderer>().enabled = false;

            
        }


        public void OnTeleGrabAction(Handle handle, Telekinesis teleGrabber)
        {
            if (item.rb.isKinematic == true)
            {
                item.rb.isKinematic = false;
            }

        }


        public void OnTeleUnGrabAction(Handle handle, Telekinesis teleGrabber)
        {
            item.rb.isKinematic = true;
        }

       

        void FixedUpdate()
        {

            if(Vector3.Distance(myRagdollPart.transf.position, myRagdollPart.ragdoll.creature.body.hipsBone.transform.position) > 2f && !sliced && sliceable)
            {
               
                                   
                myRagdollPart.ragdoll.creature.health.currentHealth -= 100f;
                if (myRagdollPart.ragdoll.creature.health.currentHealth <= 0f)
                {
                    myRagdollPart.ragdoll.creature.health.Kill();
                }

                if(myRagdollPart.partData.bone.ToString () != "Head")
                {
                    myRagdollPart.ragdoll.Slice(myRagdollPart.partData.bone);
                }
                
                

                
            }

            if (item.isTeleGrabbed)
            {
                CheckForDestroy(PlayerControl.handLeft);
                CheckForDestroy(PlayerControl.handRight);
            }


            

            if(myRagdollPart.rb.isKinematic == false && !isEnding && !DarkChains.reseting)
            {
                myRagdollPart.rb.isKinematic = true;
            }

            if(ragdollHandle != null && myRagdollPart != null)
            {

                if (myRagdollPart.transf.position != item.transform.position)
                {
                    myRagdollPart.transf.position = item.transform.position;
                    myRagdollPart.transform.rotation = item.transform.rotation;
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

                DarkChains.DarkChainDestroy(item, myRagdollPart, ragdollHandle, tele);



            }
        }
        
        
    }
}