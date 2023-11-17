using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NumFloorCollision : MonoBehaviourPun
{
    public AudioSource numSound;
    private float dissolve_duration = 0.5f;
    private float dissolveStrength = 1;
    private bool activated = false;
    private Material dissolveMaterial;
    private float elapsedTime = 0;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                activated = true;
                dissolveMaterial = this.gameObject.GetComponent<Renderer>().material;
                elapsedTime = 0;
                numSound.Play();
            }
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (activated)
            {
                if (elapsedTime < dissolve_duration)
                {
                    elapsedTime += Time.deltaTime;
                    dissolveStrength = Mathf.Lerp(0, 1, elapsedTime / dissolve_duration);
                    dissolveMaterial.SetFloat("_DissolveStrength", dissolveStrength);
                }
                if (dissolveStrength > 0.5) //0.05 destroys but don't see the dissolve effect
                {
                    PhotonNetwork.Destroy(this.gameObject);
                    activated = false;
                }
            }
           
        }
    }
}
