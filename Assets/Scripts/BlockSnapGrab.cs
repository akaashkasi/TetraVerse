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

    private Color originalOutlineColor;
   
    private Color grabbedOutlineColor = new Color(1f / 255f, 255f / 255f, 31f / 255f, 1f);

    private Outline outlineComponent;

    private bool destroyActivated = false;

    private float elapsedTime = 0;


    public void Start()
    {
        grabInteractable = this.GetComponent<XRGrabNetworkInteractable>();

        grabInteractable.selectEntered.AddListener(PlayGrabSound);
        grabInteractable.selectEntered.AddListener(SetGrabbedValue);
        grabInteractable.selectEntered.AddListener(Glow);
        //grabInteractable.selectExited.AddListener(RemoveGlow);

        pointManager = GameObject.Find("PointManager").GetComponent<PointManager>();

        PV = this.GetComponent<PhotonView>();
        debugText = GameObject.Find("txtDebug").GetComponent<TMP_Text>();

        initialLocalPositions = new Vector3[4];
        initialLocalRotations = new Quaternion[4];

        originalColors = new Color[4];

        getChildLocalTransforms();

        getChildColors();

        collisionPoint = Vector3.zero;

        outlineComponent = this.GetComponent<Outline>();
        originalOutlineColor = outlineComponent.OutlineColor;

        /**((if (outlineComponent != null)
        {
            originalOutlineColor = outlineComponent.OutlineColor;
        }*/
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

    public void Update()
    {
        if (destroyActivated)
        {
            if (elapsedTime < 2.5f)
            {
                elapsedTime += Time.deltaTime;
            }
            else
            {
                PhotonNetwork.Destroy(this.gameObject);
                destroyActivated = false;
            }
        }

        if (this.transform.position.y < 0f) // || this.transform.position.x > 2.5f || this.transform.position.x < -2.5f
            //|| this.transform.position.z > 2.5f || this.transform.position.z < -2.5f
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

public void Glow(SelectEnterEventArgs arg0) 
    {

       // Outline outlineComponent = GetComponent<Outline>();
        /**if (outlineComponent != null)
        {*/
            outlineComponent.OutlineColor = grabbedOutlineColor;
       // }
    }

    /**public void RemoveGlow(SelectExitEventArgs arg0)
    {

       // Outline outlineComponent = GetComponent<Outline>();
        if (outlineComponent != null)
        {
            outlineComponent.OutlineColor = originalOutlineColor;
       // }
    }*/

    public void PlayGrabSound(SelectEnterEventArgs arg0)
    {
        grabSound.Play();
    }

    public void SetGrabbedValue(SelectEnterEventArgs arg0)
    {
        isGrabbed = true;
    }

    public void WhenSelectionIsEnded() 
    {
        Quaternion currentRot = this.gameObject.transform.rotation; //get current rotation of parent object
        Vector3 currentPos = this.gameObject.transform.position; //get current position of parent
        Transform[] childrensWithParent = this.GetComponentsInChildren<Transform>();
        Transform[] childrens = new Transform[4]; 

        for (int i = 0; i < childrens.Length; i++)
        {
            childrens[i] = childrensWithParent[i+1]; 

            if (childrens[i].transform.localRotation != Quaternion.identity)
                childrens[i].transform.localRotation = Quaternion.identity; //set child to have 0 rotation (as it is in the prefab)

            if (outlineComponent != null)
            {
                outlineComponent.OutlineColor = originalOutlineColor;
            }

            childrens[i].transform.localPosition = initialLocalPositions[i - 1]; 
        }
        
        this.gameObject.transform.rotation = currentRot; //set parent rotation back to what it was at start
        this.gameObject.transform.position = currentPos;

        outlineComponent.OutlineColor = originalOutlineColor; //change outline color back. TODO: Not working
    }

    public void OnCollisionEnter(Collision collision)
     {
         if (grabInteractable.isSelected == false && collision.gameObject.tag == "Floor" && successfulSnap == false) //not currently being held by user, then do snap checking
         {
            /**if (collisionPoint == Vector3.zero) 
                 collisionPoint = collision.contacts[0].point;*/
            if (_coroutine == null) //trying to make it not run multiple at same time
            {
                _coroutine = StartCoroutine(SnapAndFreeze());
            }
           // SnapAndFreeze();
            
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

           // this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None; //unfreezing it

          //  UnFreezeColorChange();

          //  this.gameObject.GetComponent<XRGrabNetworkInteractable>().enabled = true;

            Debug.Log("Entered not able to snap code segment");

            errorSound.Play();
            elapsedTime = 0;
            destroyActivated = true;


        }
        _coroutine = null;
        yield return new WaitForSeconds(1f);
    }


    private bool CheckAndSetGridPositions()
    {
        bool goodResult = true;
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();

        Transform[] childrensWithParent = this.GetComponentsInChildren<Transform>();

        Transform[] childrens = new Transform[4];
        for (int i = 0; i < childrens.Length; i++)
        {
           childrens[i] = childrensWithParent[i+1];
        }
        foreach (Transform child in childrens)
        {
            //debugText.text += " child " + child.position + "\n"; 
        }
        foreach(Transform child in childrens)
        {
            bool valid = gridManager.isValidTransform(child);
            if (valid == false)
            {
                // debugText.text += "Not valid position of child: " + child.position;
                goodResult = false;
                break;
            }
        }
        if (goodResult)
        {
            foreach (Transform child in childrens)
            {
                bool occupied = gridManager.isOccupied(child);
                if (occupied)
                {
                    //debugText.text += "Found Duplicate Occupied Position " + child.position + "\n";
                    goodResult = false;
                    break;
                }
            }
        }
        if (goodResult)
        {
            foreach (Transform child in childrens)
            {
                gridManager.setPositionOccupied(child);
               // debugText.text += "Set to Occupied: " + child.position + "\n"; //should print 4 times
            }
        }
        return goodResult;
    }

    private void FreezeColorChange()
    {
        Renderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i <  childRenderers.Length; ++i)
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

}
