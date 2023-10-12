using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class TetrisBlockSnap : MonoBehaviourPun //attached to each tetris block
{
    //todo: evan add public AudioSource audio
    public float gridSize = 0.5f; // Size of each grid square
    public float offset = 0.5f / 2.0f;

    public float minX = -1.5f; // Minimum X bound
    public float maxX = 1.5f;  // Maximum X bound
    public float minZ = -2.25f; // Minimum Z bound
    public float maxZ = 2.25f;  // Maximum Z bound

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //1. Identify closest position and snap to it.
                Vector3 collisionPoint = collision.contacts[0].point;
                Debug.Log("Collision pos" + collision.contacts[0].point);
                //TODO: jumping more than needed
                //TODO: shaking/buggy
                //TODO: do I need to customize the script for each of the blocks?
                Vector3 snapPosition = new Vector3(Mathf.Round(collisionPoint.x / gridSize) * gridSize + offset, 0.25f,Mathf.Round(collisionPoint.z / gridSize) * gridSize + offset);
                Debug.Log("Snap pos" + snapPosition);

                //TODO: don't want it to snap to being outside
                //snapPosition.x = Mathf.Clamp(snapPosition.x, minX, maxX);
               // snapPosition.z = Mathf.Clamp(snapPosition.z, minZ, maxZ);

                this.transform.position = snapPosition;

                //TODO: evan play sound

                //want it to stay in same rotation
                //TODO: rotation not working
                float angle = Mathf.Round(this.transform.rotation.eulerAngles.y / 90.0f) * 90.0f;
                Quaternion newRotation = Quaternion.Euler(0, angle, 0);
                this.transform.rotation = newRotation;

                //Freeze it so it doesn't move anymore
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                

                //2. set the corresponding floor positions to occupied
                //int gridX = Mathf.FloorToInt(snapPosition.x / gridSize);
                //int gridY = Mathf.FloorToInt(snapPosition.z / gridSize);
                //TODO: but it could be more than just one position!
                //grid[gridX, gridY] = true;

                //3. disable grab:
                this.gameObject.GetComponent<XRGrabNetworkInteractable>().enabled = false;
            }
        }
        /*else if (collision.gameObject.tag == "Block") //collision with another block
        {
            //transform so long as no part of the block is inside another block
        }*/
    }

}
