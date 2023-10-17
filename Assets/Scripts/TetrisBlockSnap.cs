using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class TetrisBlockSnap : MonoBehaviourPun //attached to each tetris block
{
    public AudioSource snapSound;

    public AudioSource grabSound; 

    public float gridSize = 0.5f; // Size of each grid square
    public float offset = 0.5f / 2.0f;

    public float minX = -1.5f; // Minimum X bound
    public float maxX = 1.5f;  // Maximum X bound
    public float minZ = -2.25f; // Minimum Z bound
    public float maxZ = 2.25f;  // Maximum Z bound

    private XRGrabNetworkInteractable grabInteractable;

    public void Start()
    {
        grabInteractable = this.GetComponent<XRGrabNetworkInteractable>();

        grabInteractable.selectEntered.AddListener(PlayGrabSound);
    }
    /**public void Update()
    {
        if (this.GetComponent<XRGrabNetworkInteractable>().isSelected)
        {
            PhotonView PV = this.GetComponent<PhotonView>();

            if (PV.IsMine) // If the photon view component that I am interacting with, is owned by me
            {
                //TODO: glow by Akaash
            }
        }
    }*/

    public void PlayGrabSound(SelectEnterEventArgs arg0)
    {
        grabSound.Play();
    }

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
                //Vector3 snapPosition = new Vector3(Mathf.Round(collisionPoint.x / gridSize) * gridSize + offset, 0.25f,Mathf.Round(collisionPoint.z / gridSize) * gridSize + offset);
                Vector3 snapPosition = new Vector3(
                     Mathf.Round(collisionPoint.x / gridSize) * gridSize + offset,
                     Mathf.Round(collisionPoint.y / gridSize) * gridSize + offset,
                     Mathf.Round(collisionPoint.z / gridSize) * gridSize + offset);
                Debug.Log("Snap pos" + snapPosition);

                //TODO: don't want it to snap to being outside
                //snapPosition.x = Mathf.Clamp(snapPosition.x, minX, maxX);
               // snapPosition.z = Mathf.Clamp(snapPosition.z, minZ, maxZ);

                this.transform.position = snapPosition;


                //2. Stay in same rotation
                float anglex = Mathf.Round(this.transform.rotation.eulerAngles.x / 90.0f) * 90.0f;
                float angley = Mathf.Round(this.transform.rotation.eulerAngles.y / 90.0f) * 90.0f;
                float anglez = Mathf.Round(this.transform.rotation.eulerAngles.z / 90.0f) * 90.0f;
                Quaternion newRotation = Quaternion.Euler(anglex, angley, anglez);

                this.transform.rotation = newRotation;

                snapSound.Play();

                //3. Freeze it so it doesn't move anymore
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                //4. Visual indication of "Freeze" via lighter color.
                /**foreach (Transform child in this.gameObject.transform)
                {
                    Renderer childRenderer = child.GetComponent<Renderer>();
                    Material childMaterial = childRenderer.materials[0];
                    Color materialColor = childMaterial.color;
                    materialColor.a += 0.3f;
                    materialColor.a = Mathf.Clamp01(materialColor.a);
                }*/

                //4. set the corresponding floor positions to occupied and compute points

                //5. disable grab:
                this.gameObject.GetComponent<XRGrabNetworkInteractable>().enabled = false;
            }
        }
        /*else if (collision.gameObject.tag == "Block") //collision with another block
        {
            //transform so long as no part of the block is inside another block
        }*/
    }

}
