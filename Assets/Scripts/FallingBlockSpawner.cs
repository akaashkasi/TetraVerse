using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FallingBlockSpawner : MonoBehaviourPunCallbacks
{
    public string[] tetrisBlockPrefabsNames;
    public float spawnInterval = 8f;
    public float spawnHeight = 10f;

    private float spawnTimer = 0f;

    private void Start()
    {
        string[] blockNames = { "I-Block", "J-Block", "L-Block", "S-Block" };
        tetrisBlockPrefabsNames = blockNames;
    }

    private void Update()
    {
        // Check if we are the master client (the one responsible for spawning)
        if (PhotonNetwork.IsMasterClient)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnRandomBlock();
                spawnTimer = 0f;
            }
        }
    }

    private void SpawnRandomBlock()
    {
        int randomIndex = Random.Range(0, tetrisBlockPrefabsNames.Length);
        string chosenBlockPrefabName = tetrisBlockPrefabsNames[randomIndex];

        // Calculate the spawn position above the ceiling
        Vector3 spawnPosition = new Vector3(Random.Range(-2.5f, 2.5f), spawnHeight, Random.Range(-2.5f, 2.5f));

        // Instantiate the chosen prefab and set its position
        //GameObject newBlock = PhotonNetwork.Instantiate(chosenBlockPrefab.name, spawnPosition, Quaternion.identity);

        GameObject newBlock = PhotonNetwork.Instantiate(chosenBlockPrefabName, spawnPosition, Quaternion.identity);

        // Apply gravity to make the block fall
        Rigidbody rb = newBlock.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }

        // Handle the block spawn as needed on all clients
       // HandleBlockSpawn(newBlock);
    }

   /** private void HandleBlockSpawn(GameObject block)
    {
        // Implement your logic for handling block spawn, e.g., parenting to a game board
    }*/

    //TODO: Need to implement synchronization
    /*public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We are the owner of this object; send data to others.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // We are receiving data from the owner.
            Vector3 receivedPosition = (Vector3)stream.ReceiveNext();
            Quaternion receivedRotation = (Quaternion)stream.ReceiveNext();

            // Update the position and rotation of the object on all clients.
            transform.position = receivedPosition;
            transform.rotation = receivedRotation;
        }
    }*/
}
