using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRNoPeeking : MonoBehaviour
{
    [SerializeField] LayerMask collisionLayer;
    [SerializeField] float fadeSpeed;
    [SerializeField] float sphereCheckSize = 0.15f;

    private Material cameraFadeMat;
    private bool isCameraFadeOut = false;

    private void Awake() => cameraFadeMat = GetComponent<Renderer>().material;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.CheckSphere(transform.position, sphereCheckSize, collisionLayer, QueryTriggerInteraction.Ignore ))
        {
            CameraFade(1f);
            isCameraFadeOut = true;
        } else {
            if (!isCameraFadeOut)
            {
                return;
            }
            CameraFade(0f);
        }
    }

    public void CameraFade(float targetAlpha) {
        var fadeValue = Mathf.MoveTowards(cameraFadeMat.GetFloat("_AlphaValue"), targetAlpha, Time.deltaTime * fadeSpeed);
        cameraFadeMat.SetFloat("_AlphaValue", fadeValue);

        if (fadeValue <= 0.01f)
        {
            isCameraFadeOut = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.75f);
        Gizmos.DrawSphere(transform.position, sphereCheckSize);
    }
}
