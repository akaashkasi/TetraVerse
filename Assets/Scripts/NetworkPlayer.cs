using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
public class networkPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    private PhotonView photonView;

    private Transform headRig;
    private Transform LeftHandRig;
    private Transform RightHandRig;
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        headRig = GameObject.Find("XR Origin (XR Rig)/Camera Offset/Main Camera").transform;
        LeftHandRig = GameObject.Find("XR Origin (XR Rig)/Camera Offset/Left Controller").transform;
        RightHandRig = GameObject.Find("XR Origin (XR Rig)/Camera Offset/Right Controller").transform;

    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            rightHand.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);
            head.gameObject.SetActive(false);
            MapPosition(head, headRig);
            MapPosition(leftHand, LeftHandRig);
            MapPosition(rightHand, RightHandRig);
        }

    }

    void MapPosition(Transform target, Transform rigTransform)
    {

        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}

