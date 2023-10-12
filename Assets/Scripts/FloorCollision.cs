using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class FloorCollision : MonoBehaviourPun
{
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Num")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                collision.gameObject.GetComponent<AudioSource>().Play();
                Destroy(collision.gameObject);
            }

        }
        else if (collision.gameObject.tag == "Block")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                collision.gameObject.GetComponent<XRGrabNetworkInteractable>().enabled = false;
            }
        }
    }
}
