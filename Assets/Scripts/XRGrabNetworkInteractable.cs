using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class XRGrabNetworkInteractable : XRGrabInteractable
{
    private PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = this.GetComponent<PhotonView>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        PV.RequestOwnership();
        base.OnSelectEntered(args);
    }
}

