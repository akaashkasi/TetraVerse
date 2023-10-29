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
    private PhotonView PV;

    TetrisManager targetScript = GameObject.Find("TetrisManager").GetComponent<TetrisManager>();
    public bool isGrabbed = false;

    public void Start()
    {
        grabInteractable = this.GetComponent<XRGrabNetworkInteractable>();

        grabInteractable.selectEntered.AddListener(PlayGrabSound);
        grabInteractable.selectEntered.AddListener(Glow);
        grabInteractable.selectExited.AddListener(RemoveGlow);

        PV = this.GetComponent<PhotonView>();
    }

    public void Glow(SelectEnterEventArgs arg0)
    {
        PV.RequestOwnership(); 

        PV.RPC("TriggerGlow", RpcTarget.AllBuffered);
    }

    public void RemoveGlow(SelectExitEventArgs arg0)
    {
        PV.RequestOwnership();

        PV.RPC("TriggerRemoveGlow", RpcTarget.AllBuffered);
    }

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
                //TODO: blocks 
                //1. Identify closest position and snap to it.
                Vector3 collisionPoint = collision.contacts[0].point;
                /**Debug.Log("Collision floor pos" + collision.contacts[0].point);

                //aligning with grid lines
                Vector3 snapPosition = new Vector3(
                     Mathf.Round(collisionPoint.x / gridSize) * gridSize + offset,
                     Mathf.Round(collisionPoint.y / gridSize) * gridSize + offset,
                     Mathf.Round(collisionPoint.z / gridSize) * gridSize + offset);
                

                //2. Stay in same rotation
                float anglex = Mathf.Round(this.transform.rotation.eulerAngles.x / 90.0f) * 90.0f;
                float angley = Mathf.Round(this.transform.rotation.eulerAngles.y / 90.0f) * 90.0f;
                float anglez = Mathf.Round(this.transform.rotation.eulerAngles.z / 90.0f) * 90.0f;
                Quaternion newRotation = Quaternion.Euler(anglex, angley, anglez);*/

                //bounds checking for each specific block

                if (this.gameObject.tag == "I-Block")
                {
                    Vector3 currentRotation = this.transform.rotation.eulerAngles;

                    float targetZRotation = Mathf.Abs(currentRotation.z - 0f) < Mathf.Abs(currentRotation.z - 180f) ? 0f : 180f;
                    currentRotation.z = targetZRotation;

                    float targetYRotation = Mathf.Round(this.transform.rotation.eulerAngles.y / 90.0f) * 90.0f;
                    currentRotation.y = targetYRotation;

                    float targetXRotation = Mathf.Round(this.transform.rotation.eulerAngles.z / 90.0f) * 90.0f;
                    currentRotation.x = targetZRotation;

                    Vector3 snapPosition = new Vector3(
                     Mathf.Round(collisionPoint.x / gridSize) * gridSize + offset,
                     offset,
                     Mathf.Round(collisionPoint.z / gridSize) * gridSize + offset);

                    this.transform.position = snapPosition;

                    this.transform.rotation = Quaternion.Euler(currentRotation);

                }
                else if (this.gameObject.tag == "J-Block" ||
                    this.gameObject.tag == "L-Block" ||
                    this.gameObject.tag == "S-Block" ||
                    this.gameObject.tag == "Z-Block" ||
                    this.gameObject.tag == "Square-Block" ||
                    this.gameObject.tag == "T-Block")
                {
                    Vector3 currentRotation = this.transform.rotation.eulerAngles;

                    //fix to either +90 or -90: must be "flat" on the floor
                    float targetXRotation = Mathf.Abs(currentRotation.x - 90f) < Mathf.Abs(currentRotation.x + 90f) ? 90f : -90f;
                    currentRotation.x = targetXRotation;

                    float targetYRotation = Mathf.Round(this.transform.rotation.eulerAngles.y / 90.0f) * 90.0f;
                    currentRotation.y = targetYRotation;

                    float targetZRotation = Mathf.Round(this.transform.rotation.eulerAngles.z / 90.0f) * 90.0f;
                    currentRotation.z = targetZRotation;

                    Vector3 snapPosition = new Vector3(
                     Mathf.Round(collisionPoint.x / gridSize) * gridSize + offset,
                     offset,
                     Mathf.Round(collisionPoint.z / gridSize) * gridSize + offset);
                    //TODO: y is exactly 0.25f- I think?

                    this.transform.position = snapPosition;

                    this.transform.rotation = Quaternion.Euler(currentRotation);
                }

                //this.transform.position = snapPosition;
                //Debug.Log("Snap pos" + snapPosition);

                //this.transform.rotation = newRotation;
               // Debug.Log("Snap Rotation" + newRotation);

                //3. Play snap sound
                snapSound.Play();

                //4. Freeze it so it doesn't move anymore
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                //5. Visual indication of "Freeze" via transparency
                FreezeColorChange();

                targetScript.addPoints(5);
                if (isGrabbed) {
                    targetScript.addPoints(10);
                }
                Debug.Log(targetScript.getPoints());

                //6. disable grab:
                this.gameObject.GetComponent<XRGrabNetworkInteractable>().enabled = false;
            }
        }
        /**else if (collision.gameObject.tag == "Block") //collision with another block
        {
            Vector3 collisionPoint = collision.contacts[0].point;
            Vector3 snapPosition = new Vector3(
                     Mathf.Round(collisionPoint.x / gridSize) * gridSize + offset,
                     Mathf.Round(collisionPoint.y / gridSize) * gridSize + offset,
                     Mathf.Round(collisionPoint.z / gridSize) * gridSize + offset);
            this.transform.position = snapPosition;

            //2. Stay in same rotation
            float anglex = Mathf.Round(this.transform.rotation.eulerAngles.x / 90.0f) * 90.0f;
            float angley = Mathf.Round(this.transform.rotation.eulerAngles.y / 90.0f) * 90.0f;
            float anglez = Mathf.Round(this.transform.rotation.eulerAngles.z / 90.0f) * 90.0f;
            Quaternion newRotation = Quaternion.Euler(anglex, angley, anglez);

            this.transform.rotation = newRotation;

            //3. Play snap sound
            snapSound.Play();

            //4. Freeze it so it doesn't move anymore
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            //5. Visual indication of "Freeze" via transparency
            FreezeColorChange();

            //6. disable grab:
            this.gameObject.GetComponent<XRGrabNetworkInteractable>().enabled = false;
        }*/
    }

    private void FreezeColorChange()
    {
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer childRenderer in childRenderers)
        {
            Color originalColor = childRenderer.material.color;

            Color lighterColor = new Color(
            originalColor.r + 0.2f * (1 - originalColor.r),
            originalColor.g + 0.2f * (1 - originalColor.g),
            originalColor.b + 0.2f * (1 - originalColor.b),
            originalColor.a
            );
            childRenderer.material.color = lighterColor;

        }
    }
    [PunRPC]
    public void TriggerGlow()
    {
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer childRenderer in childRenderers)
        {
            Color originalColor = childRenderer.material.color;

            Color glowColor = originalColor * 5f;
            childRenderer.material.SetColor("_EmissionColor", glowColor);

        }
    }
    [PunRPC]
    public void TriggerRemoveGlow()
    {
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer childRenderer in childRenderers)
        {
            Color glowColor = childRenderer.material.color;

            Color originalColor = glowColor / 5f;
            childRenderer.material.SetColor("_EmissionColor", originalColor);

        }
    }

}
