using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class FloorCollision : MonoBehaviourPun
{
    //TODO:: dissolve and audio not working. How do I make the inner object the ONLY object?
    private float dissolve_duration = 1f;
    public float dissolveStrength;
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Num")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //collision.gameObject.GetComponent<AudioSource>().Play();
                GameObject num = collision.gameObject;
                Material dissolveMaterial = num.GetComponent<Renderer>().material;
                float elapsed_time = 0;
                while (elapsed_time < dissolve_duration)
                {
                    elapsed_time += Time.deltaTime;
                    dissolveStrength = Mathf.Lerp(0, 1, elapsed_time / dissolve_duration);
                    dissolveMaterial.SetFloat("_DissolveStrength", dissolveStrength);
                }
                Destroy(collision.gameObject);
            }

        }
    }
}
