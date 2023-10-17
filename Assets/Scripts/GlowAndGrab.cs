using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class GrabAndGlow : MonoBehaviourPun
{
    private MeshRenderer _renderer;
    private Color _originalColor;
    private XRBaseInteractable _interactable;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _originalColor = _renderer.material.color;
        _interactable = GetComponent<XRBaseInteractable>();

        _interactable.selectEntered.AddListener(OnGrabbed);
        _interactable.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        photonView.RPC("SetGlow", RpcTarget.AllBuffered, true);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        photonView.RPC("SetGlow", RpcTarget.AllBuffered, false);
    }

    [PunRPC]
    private void SetGlow(bool glow)
    {
        if (glow)
        {
            _renderer.material.SetColor("_EmissionColor", _originalColor * 1.5f);
        }
        else
        {
            _renderer.material.SetColor("_EmissionColor", _originalColor);
        }
    }

    private void OnDestroy()
    {
        _interactable.selectEntered.RemoveListener(OnGrabbed);
        _interactable.selectExited.RemoveListener(OnReleased);
    }
}