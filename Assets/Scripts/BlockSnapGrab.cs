using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using TMPro;

public class BlockSnapGrab : MonoBehaviourPun //attached to each tetris block
{
    public AudioSource snapSound;

    public AudioSource grabSound;

    public AudioSource errorSound;

    private const float gridSize = 0.5f; // Size of each grid square
    private const float offset = 0.25f;

    private XRGrabNetworkInteractable grabInteractable;
    private PhotonView PV;

    public GridManager gridManager;
    private PointManager pointManager;

    public bool isGrabbed = false;

    public bool successfulSnap = false;

    public Vector3 collisionPoint;

    public TMP_Text debugText;

    public void Start()
    {
        grabInteractable = this.GetComponent<XRGrabNetworkInteractable>();

        grabInteractable.selectEntered.AddListener(PlayGrabSound);
        grabInteractable.selectEntered.AddListener(SetGrabbedValue);
        grabInteractable.selectEntered.AddListener(Glow);
        grabInteractable.selectExited.AddListener(RemoveGlow);

        pointManager = GameObject.Find("PointManager").GetComponent<PointManager>();

        PV = this.GetComponent<PhotonView>();
        debugText = GameObject.Find("txtDebug").GetComponent<TMP_Text>();
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

    public void SetGrabbedValue(SelectEnterEventArgs arg0)
    {
        isGrabbed = true;
    }

    //is it cause it goes through the floor that the collision for user isn't working as expected?
   /** public void OnCollisionEnter(Collision collision)
    {
        if (grabInteractable.isSelected == false) //not currently being held by user, then do snap checking
        {
            if (collision.gameObject.tag == "Floor")
            {
                //debugText.text = "";
                if (PhotonNetwork.IsMasterClient)
                {
                    //1. Identify closest position and snap to it.
                     
                   if (successfulSnap == false) //&& selectExited == false) //know that the block is not snapped
                    {
                        collisionPoint = collision.contacts[0].point;
                        SnapAndFreeze(collisionPoint);
                        //debugText.text = debugText.text + "Debug: finished SnapAndFreeze " + count + " Block type: " + this.gameObject.tag + "\n";
                        //count++;
                    }
                    //debugText.text = debugText.text + "Debug: outside if block OnCollisionEnter " + count2 + " Block type: " + this.gameObject.tag + "\n";
                    //count2++;

                }
            }
        }
    }*/



    public void SnapAndFreeze()
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

        this.transform.rotation = Quaternion.Euler(currentRotation);

        //moved outside
        //this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

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

            // 7. add points
            pointManager.addTetrisPoints();

            successfulSnap = true;

            if (isGrabbed) //user grabbed it- extra points
            {
                pointManager.addUserInteractionPoints();
            }

            this.gameObject.GetComponent<Rigidbody>().useGravity = false;

            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        }
        else if (goodResult == false && successfulSnap == false)
        {
            successfulSnap = false;

            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None; //unfreezing it

            Debug.Log("Entered not able to snap code segment");
            errorSound.Play();
        }
    }


    private bool CheckAndSetGridPositions()
    {
        Transform[] childAndParentTransforms = GetComponentsInChildren<Transform>();

        Transform[] childTransforms = new Transform[childAndParentTransforms.Length - 1];

        bool goodResult = true;

        for (int i = 1; i < childAndParentTransforms.Length; i++)
        {
            childTransforms[i - 1] = childAndParentTransforms[i];
        }

        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();

        foreach (Transform childTransform in childTransforms)
        {
            bool valid = gridManager.isValidTransform(childTransform);
            if (valid == false)
            {
                Debug.Log("Found invalid position block script");
                debugText.text += "Collision Point: " + collisionPoint + "\n";
                debugText.text += "Not Valid Transform: " + childTransform.position + "\n"; //here
                goodResult = false;
                break;
   
            }
        }
        if (goodResult)
        {
            foreach (Transform childTransform in childTransforms)
            {
                bool occupied = gridManager.isOccupied(childTransform);
                if (occupied)
                {
                    Debug.Log("Found invalid position block script");
                    debugText.text += "Occupied Position " + childTransform.position;
                    goodResult = false;
                    break;
                }
            }
        }
        //know all positions are valid and empty when we reach here
        if (goodResult)
        {
            foreach (Transform childTransform in childTransforms)
            {
                gridManager.setPositionOccupied(childTransform);
                Debug.Log("set a position occupied block script");
            }
        }
        return goodResult;
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
