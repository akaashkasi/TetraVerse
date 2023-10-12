using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class FallingBlockSpawner : MonoBehaviourPun
{
    private float countdownInterval = 1.0f;
    private float countdownTime = 3.0f;

    private string[] tetrisBlockPrefabsNames;
    private float spawnInterval = 8f;
    public float spawnHeight = 10f;

    private float spawnTimer;


    private void Start()
    {

        string[] blockNames = { "I-Block", "J-Block", "L-Block", "S-Block", "Square-Block", "T-Block", "Z-Block" };
        tetrisBlockPrefabsNames = blockNames;
    }
    private void Update() 
    {
        if (PhotonNetwork.IsMasterClient)
        {
            spawnTimer += Time.deltaTime;
            if (countdownTime > 0) //TODO: okay way to check 1 sec?
            {
                if (spawnTimer >= countdownInterval)
                {
                    SpawnCountdownBlock();
                    countdownTime--;
                    spawnTimer = 0f;
                }
            }
            else
            {
                if (spawnTimer >= spawnInterval)
                {
                    SpawnRandomBlock();
                    spawnTimer = 0f;
                }
            }

        }
    }

    private void SpawnCountdownBlock()
    {
        string time = "Num" + ((int) countdownTime).ToString();
        Vector3 spawnPosition = new Vector3(0, spawnHeight, 0);
        GameObject newBlock = PhotonNetwork.Instantiate(time, spawnPosition, Quaternion.identity);
        Rigidbody rb = newBlock.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.drag = 2.0f;
        }
    }

    private void SpawnRandomBlock()
    {
        int randomIndex = Random.Range(0, tetrisBlockPrefabsNames.Length);
        string chosenBlockPrefabName = tetrisBlockPrefabsNames[randomIndex];

        // Calculate the spawn position above the ceiling
        Vector3 spawnPosition = new Vector3(Random.Range(-2.5f, 2.5f), spawnHeight, Random.Range(-2.5f, 2.5f));


        GameObject newBlock = PhotonNetwork.Instantiate(chosenBlockPrefabName, spawnPosition, Quaternion.identity);

        // Apply gravity to make the block fall
        Rigidbody rb = newBlock.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.drag = 2.0f;
        }

    }

    //TODO: post processing- glow when grabbed, should be scripts on the individual tetris blocks. 
}
