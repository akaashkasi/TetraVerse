using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ColorChangeonGrab : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
            Debug.Log("Grab interactable listeners added.", this);
        }
        else
        {
            Debug.LogError("XRGrabInteractable component not found on " + gameObject.name, this);
        }

        // Initially set all emissions to off
        ToggleEmission(false);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("Object grabbed, enabling emission.", this);
        ToggleEmission(true);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log("Object released, disabling emission.", this);
        ToggleEmission(false);
    }

    private void ToggleEmission(bool shouldEnable)
    {
        foreach (Transform child in transform)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = renderer.material;
                if (shouldEnable)
                {
                    mat.EnableKeyword("_Emission");
                    mat.SetColor("_Emission Map", Color.yellow); // Ensure a visible emission color
                }
                else
                {
                    mat.DisableKeyword("_EMISSION");
                }
                // Update the material's global illumination flags
                mat.globalIlluminationFlags = shouldEnable ?
                    (mat.globalIlluminationFlags & ~MaterialGlobalIlluminationFlags.EmissiveIsBlack) :
                    (mat.globalIlluminationFlags | MaterialGlobalIlluminationFlags.EmissiveIsBlack);
            }
            else
            {
                Debug.LogError("Renderer not found on child " + child.gameObject.name, this);
            }
        }
    }
}
