using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class BlockSnapGrab : MonoBehaviourPun //attached to each tetris block
{
    public AudioSource snapSound;

    public AudioSource grabSound;

    public AudioSource errorSound;

    private float gridSize = 0.5f; // Size of each grid square
    private float offset = 0.5f / 2.0f;

    private float minX = -1.5f; // Minimum X bound
    private float maxX = 1.5f;  // Maximum X bound
    private float minZ = -2.25f; // Minimum Z bound
    private float maxZ = 2.25f;  // Maximum Z bound

    private XRGrabNetworkInteractable grabInteractable;
    private PhotonView PV;

    public GridManager gridManager; 


    public void Start()
    {
        grabInteractable = this.GetComponent<XRGrabNetworkInteractable>();

        grabInteractable.selectEntered.AddListener(PlayGrabSound);
        grabInteractable.selectEntered.AddListener(Glow);
        grabInteractable.selectExited.AddListener(RemoveGlow);

        PV = this.GetComponent<PhotonView>();
    }

    public void Glow(SelectEnterEventArgs arg0) //TODO: not working
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
                //1. Identify closest position and snap to it.
                Vector3 collisionPoint = collision.contacts[0].point;

                if (grabInteractable.enabled) //know that the block is not snapped/frozen
                {
                    SnapAndFreeze(collisionPoint);
                }

            }
        }
    }

    private void SnapAndFreeze(Vector3 collisionPoint)
    {
        float targetXRotation;
        float targetYRotation;
        float targetZRotation;
        Vector3 currentRotation = this.transform.rotation.eulerAngles;
        //1. Set rotation
        if (this.gameObject.tag == "I-Block")
        {

            targetZRotation = Mathf.Abs(currentRotation.z - 0f) < Mathf.Abs(currentRotation.z - 180f) ? 0f : 180f;
            currentRotation.z = targetZRotation;

            targetYRotation = Mathf.Round(this.transform.rotation.eulerAngles.y / 90.0f) * 90.0f;
            currentRotation.y = targetYRotation;

            targetXRotation = Mathf.Round(this.transform.rotation.eulerAngles.x / 90.0f) * 90.0f;
            currentRotation.x = targetXRotation;

        }
        else if (this.gameObject.tag == "J-Block" ||
            this.gameObject.tag == "L-Block" ||
            this.gameObject.tag == "S-Block" ||
            this.gameObject.tag == "Z-Block" ||
            this.gameObject.tag == "Square-Block" ||
            this.gameObject.tag == "T-Block")
        {

            //fix to either +90 or -90: must be "flat" on the floor
            targetXRotation = Mathf.Abs(currentRotation.x - 90f) < Mathf.Abs(currentRotation.x + 90f) ? 90f : -90f;
            currentRotation.x = targetXRotation;

            targetYRotation = Mathf.Round(this.transform.rotation.eulerAngles.y / 90.0f) * 90.0f;
            currentRotation.y = targetYRotation;

            targetZRotation = Mathf.Round(this.transform.rotation.eulerAngles.z / 90.0f) * 90.0f;
            currentRotation.z = targetZRotation;
        }
        //2. Compute Snap Position
        Vector3 snapPosition = new Vector3(
         Mathf.Round(collisionPoint.x / gridSize) * gridSize + offset,
         offset,
         Mathf.Round(collisionPoint.z / gridSize) * gridSize + offset);

        this.transform.position = snapPosition;

        this.transform.rotation = Quaternion.Euler(currentRotation); //has to happen before 

        bool goodResult = CheckAndSetGridPositions();

        if (goodResult == true) //returns true successful, then we can snap
        {
            Debug.Log("Entered snap code segment in block script");
            //3. Play snap sound
            snapSound.Play();

            //4. Freeze it so it doesn't move anymore
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            //5. Visual indication of "Freeze" via transparency
            FreezeColorChange();

            //6. disable grab:
            this.gameObject.GetComponent<XRGrabNetworkInteractable>().enabled = false;

            //TODO: Call public method in point system class. Should only calculate if snapped
            //PointSystem.CalculateBlockPoints(this.transform);
        }
        else if (goodResult == false)
        {
            //do nothing block
            Debug.Log("Entered not able to snap code segment");
            errorSound.Play();
        }
    }
    private bool CheckAndSetGridPositions()
    {
        Transform[] childAndParentTransforms = GetComponentsInChildren<Transform>();

        Transform[] childTransforms = new Transform[childAndParentTransforms.Length - 1];

        for (int i = 1; i < childAndParentTransforms.Length; i++)
        {
            childTransforms[i - 1] = childAndParentTransforms[i];
        }

        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();

        foreach (Transform childTransform in childTransforms)
        {
            bool valid = gridManager.isValidTransform(childTransform);
            if (!valid)
            {
                Debug.Log("Found invalid position block script"); 
                return false; 
            }
        }
        foreach (Transform childTransform in childTransforms)
        {
            bool occupied = gridManager.isOccupied(childTransform);
            if (occupied)
            {
                Debug.Log("Found invalid position block script");
                return false;
            }
        }
        //know all positions are valid and empty when we reach here
        foreach (Transform childTransform in childTransforms)
        {
            gridManager.setPositionOccupied(childTransform);
            Debug.Log("set a position occupied block script");
        }
        return true;
    }

    private void FreezeColorChange()
    {
        Renderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (Renderer childRenderer in childRenderers)
        {
            Color originalColor = childRenderer.material.color;

            Color lighterColor = new Color(
            originalColor.r + 0.5f * (1 - originalColor.r),
            originalColor.g + 0.5f * (1 - originalColor.g),
            originalColor.b + 0.5f * (1 - originalColor.b),
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
