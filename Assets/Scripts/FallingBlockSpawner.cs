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
    private float[] xzRangePositions;
    private float spawnInterval = 8f;
    public float spawnHeight = 10f;

    private float spawnTimer;


    private void Start()
    {

        // string[] blockNames = { "I-Block", "J-Block", "L-Block", "S-Block", "Square-Block", "T-Block", "Z-Block" };
        string[] blockNames = { "I-Block" };

        float[] xzRange = { -1.25f, -0.75f, -0.25f, 0.25f, 0.75f, 1.25f};

        xzRangePositions = xzRange;

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

        int randomXIndex = Random.Range(0, xzRangePositions.Length);
        float xPos = xzRangePositions[randomXIndex];
        int randomZIndex = Random.Range(0, xzRangePositions.Length);
        float zPos = xzRangePositions[randomZIndex];

        Vector3 spawnPosition = new Vector3(xPos, spawnHeight, zPos);

        GameObject newBlock = PhotonNetwork.Instantiate(chosenBlockPrefabName, spawnPosition, Quaternion.identity);

        // Apply gravity to make the block fall
        Rigidbody rb = newBlock.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.drag = 3.0f;
        }

    }

    //TODO: post processing- glow when grabbed, should be scripts on the individual tetris blocks. 
}
