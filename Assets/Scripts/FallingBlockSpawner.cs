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

    private float spawnHeight = 6.5f;

    private float spawnTimer;

    private const int Level0 = 0;
    private const int Level1 = 1;
    private const int Level2 = 2;
    private const int Level3 = 3;

    private int curLevel;

    private int numBlocksSpawned;
    private int level1NumBlocksThreshold = 2; //15
    private int level2NumBlocksThreshold = 4; //25
    private int level3NumBlocksThreshold = 6; //35

    private float currSpawnInterval;
    private float level0SpawnInterval = 10f; 
    private float level1SpawnInterval = 8f; 
    private float level2SpawnInterval = 6f;
    private float level3SpawnInterval = 4f; 

    private float level0LinearDrag = 20.0f; //doing dramatic just to see if it works as expected
    private float level1LinearDrag = 10.0f;
    private float level2LinearDrag = 5.0f;
    private float level3LinearDrag = 3.0f;

    private void Start()
    {
        curLevel = Level0;

        numBlocksSpawned = 0;
        currSpawnInterval = level0SpawnInterval; //initial interval

        //string[] blockNames = { "I-Block", "J-Block", "L-Block", "S-Block", "Square-Block", "T-Block", "Z-Block" };
        string[] blockNames = { "I-Block", "J-Block"};

        float[] xzRange = { -0.75f, -0.25f, 0.25f, 0.75f}; //TODO: figure out how to do the range better. potentially dictionary for each block- array of possible values

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
                if (spawnTimer >= currSpawnInterval)
                {
                    SpawnRandomBlock();
                    spawnTimer = 0f;

                    numBlocksSpawned++;

                    //After a set number of blocks which have been spawned, decrease the time between the blocks spawning.
                    //Move onto next "level"
                    if (numBlocksSpawned > level3NumBlocksThreshold)
                    {
                        curLevel = Level3;
                        currSpawnInterval = level3SpawnInterval;
                    }
                    else if (numBlocksSpawned > level2NumBlocksThreshold)
                    {
                        curLevel = Level2;
                        currSpawnInterval = level2SpawnInterval;
                    }
                    else if (numBlocksSpawned > level1NumBlocksThreshold)
                    {
                        curLevel = Level1;
                        currSpawnInterval = level1SpawnInterval;
                    }
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

        Rigidbody rb = newBlock.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;

            //change the pace at which the object falls based on the level

            if (curLevel == Level0)
            {
                rb.drag = level0LinearDrag;
            }
            else if (curLevel == Level1)
            {
                rb.drag = level1LinearDrag;
            }
            else if (curLevel == Level2)
            {
                rb.drag = level2LinearDrag;
            }
            else //Level 3
            {
                rb.drag = level3LinearDrag;
            }
        }

    } 
}
