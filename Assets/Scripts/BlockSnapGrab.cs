using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using TMPro;
using UnityEngine.XR;
public class BlockSnapGrab : MonoBehaviourPun //attached to each tetris block
{
    public AudioSource snapSound;
    public AudioSource grabSound;
    public AudioSource errorSound;
    private Material material;
    private const float gridSize = 0.5f; // Size of each grid square
    private const float offset = 0.25f;
    private XRGrabNetworkInteractable grabInteractable;
    private PhotonView PV;
    public GridManager gridManager;
    private PointManager pointManager;
    public bool isGrabbed = false;
    public bool successfulSnap = false;
    public Vector3 collisionPoint;
    public int count = 0;
    public TMP_Text debugText;
    private Vector3[] initialLocalPositions;
    private Quaternion[] initialLocalRotations;
    private Color[] originalColors;
    private Coroutine _coroutine;
    public void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;
        grabInteractable = this.GetComponent<XRGrabNetworkInteractable>();
        grabInteractable.selectEntered.AddListener(PlayGrabSound);
        grabInteractable.selectEntered.AddListener(SetGrabbedValue);
        grabInteractable.selectEntered.AddListener(Glow);
        grabInteractable.selectExited.AddListener(RemoveGlow);
        pointManager = GameObject.Find("PointManager").GetComponent<PointManager>();
        PV = this.GetComponent<PhotonView>();
        debugText = GameObject.Find("txtDebug").GetComponent<TMP_Text>();
        initialLocalPositions = new Vector3[4];
        initialLocalRotations = new Quaternion[4];
        originalColors = new Color[4];
        getChildLocalTransforms();
        getChildColors();
        collisionPoint = Vector3.zero;
    }
    private void getChildLocalTransforms()
    {
        int childCount = this.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            initialLocalPositions[i] = child.localPosition;
            initialLocalRotations[i] = child.localRotation;
        }
    }
    private void getChildColors()
    {
        Renderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < childRenderers.Length; i++)
        {
            Renderer childRenderer = childRenderers[i];
            // Store the original color
            originalColors[i] = childRenderer.material.color;
        }
    }
    public void Glow(SelectEnterEventArgs arg0) //TODO: not working
    {
        // PV.RequestOwnership();
        material.EnableKeyword("_EMISSION");
        // PV.RPC("TriggerGlow", RpcTarget.AllBuffered);
    }
    public void RemoveGlow(SelectExitEventArgs arg0)
    {
        // PV.RequestOwnership();
        material.DisableKeyword("_EMISSION");
        // PV.RPC("TriggerRemoveGlow", RpcTarget.AllBuffered);
    }
    public void PlayGrabSound(SelectEnterEventArgs arg0)
    {
        grabSound.Play();
    }
    public void SetGrabbedValue(SelectEnterEventArgs arg0)
    {
        isGrabbed = true;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (grabInteractable.isSelected == false && collision.gameObject.tag == "Floor" && successfulSnap == false) //not currently being held by user, then do snap checking
        {
            /** if (collisionPoint == Vector3.zero)
                 collisionPoint = collision.contacts[0].point;*/
            if (_coroutine == null) //trying to make it not run multiple at same time
            {
                _coroutine = StartCoroutine(SnapAndFreeze());
            }
            // }
        }
    }
    private IEnumerator SnapAndFreeze()
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
            targetXRotation = Mathf.Abs(currentRotation.x - 90f) < Mathf.Abs(currentRotation.x + 90f) ? 90f : -90f;
            currentRotation.x = targetXRotation;
            targetYRotation = Mathf.Round(this.transform.rotation.eulerAngles.y / 90.0f) * 90.0f;
            currentRotation.y = targetYRotation;
            targetZRotation = Mathf.Round(this.transform.rotation.eulerAngles.z / 90.0f) * 90.0f;
            currentRotation.z = targetZRotation;
        }
        //2. Compute Snap Position
        /**Vector3 snapPosition = new Vector3(
         Mathf.Round(collisionPoint.x / gridSize) * gridSize + offset,
         offset,
         Mathf.Round(collisionPoint.z / gridSize) * gridSize + offset);*/
        //new way of trying
        Vector3 snapPosition = new Vector3(
        Mathf.Round(this.transform.position.x / gridSize) * gridSize + offset,
        offset,
        Mathf.Round(this.transform.position.z / gridSize) * gridSize + offset);
        this.transform.position = snapPosition;
        this.transform.rotation = Quaternion.Euler(currentRotation);
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
        }
        else if (goodResult == false)
        {
            successfulSnap = false;
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None; //unfreezing it
            UnFreezeColorChange();
            this.gameObject.GetComponent<XRGrabNetworkInteractable>().enabled = true;
            Debug.Log("Entered not able to snap code segment");
            errorSound.Play();
            pointManager.subtractPoints(5);
        }
        _coroutine = null;
        yield return new WaitForSeconds(1f);
    }
    private bool CheckAndSetGridPositions()
    {
        bool goodResult = true;
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        foreach (Vector3 childLocalPosition in initialLocalPositions)
        {
            Vector3 currentChildPosition = this.transform.position + childLocalPosition;
            bool valid = gridManager.isValidPositionVector(currentChildPosition);
            if (valid == false)
            {
                // debugText.text += "Not valid position of child: " + currentChildPosition;
            }
        }
        if (goodResult)
        {
            foreach (Vector3 childLocalPosition in initialLocalPositions)
            {
                Vector3 currentChildPosition = this.transform.position + childLocalPosition;
                bool occupied = gridManager.isOccupiedVector(currentChildPosition);
                if (occupied)
                {
                    Debug.Log("Found invalid position block script");
                    // debugText.text += "Found Duplicate Occupied Position " + currentChildPosition + "\n";
                    goodResult = false;
                    break;
                }
            }
        }
        if (goodResult)
        {
            foreach (Vector3 childLocalPosition in initialLocalPositions)
            {
                Vector3 currentChildPosition = this.transform.position + childLocalPosition;
                gridManager.setPositionOccupiedVector(currentChildPosition);
                // debugText.text += "Set to Occupied: " + currentChildPosition + "\n";
            }
        }
        return goodResult;
    }
    private void FreezeColorChange()
    {
        Renderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < childRenderers.Length; ++i)
        {
            Color originalColor = originalColors[i];
            Color lighterColor = new Color(
            originalColor.r + 0.5f * (1 - originalColor.r),
            originalColor.g + 0.5f * (1 - originalColor.g),
            originalColor.b + 0.5f * (1 - originalColor.b),
            originalColor.a
            );
            childRenderers[i].material.color = lighterColor;
        }
    }
    private void UnFreezeColorChange()
    {
        Renderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < childRenderers.Length; ++i)
        {
            childRenderers[i].material.color = originalColors[i];
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