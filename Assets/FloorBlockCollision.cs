using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using TMPro;
public class FloorBlockCollision : MonoBehaviourPun
{

    public void OnCollisionEnter(Collision collision) //collision with floor
    {
        if (collision.gameObject.tag == "I-Block" || collision.gameObject.tag == "J-Block"
            || collision.gameObject.tag == "L-Block" || collision.gameObject.tag == "S-Block"
            || collision.gameObject.tag == "T-Block" || collision.gameObject.tag == "Z-Block"
            || collision.gameObject.tag == "Square-Block")
            {
                if (PhotonNetwork.IsMasterClient)
                {

                /**if (successfulSnap == false) //&& selectExited == false) //know that the block is not snapped
                {*/
                if (collision.gameObject.GetComponent<BlockSnapGrab>().successfulSnap == false)
                {
                    Vector3 floorCollisionPoint = collision.contacts[0].point;
                    collision.gameObject.GetComponent<BlockSnapGrab>().collisionPoint = floorCollisionPoint;
                    collision.gameObject.GetComponent<BlockSnapGrab>().SnapAndFreeze();
                    //debugText.text = debugText.text + "Debug: finished SnapAndFreeze " + count + " Block type: " + this.gameObject.tag + "\n";
                    //count++;
                }
                    //}
                    //debugText.text = debugText.text + "Debug: outside if block OnCollisionEnter " + count2 + " Block type: " + this.gameObject.tag + "\n";
                    //count2++;

                }
            }
        }
    //TODO:: same 4 collision points, weird values even though position of block looks good on floor

}
