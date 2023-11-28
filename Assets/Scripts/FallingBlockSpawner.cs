using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class FallingBlockSpawner : MonoBehaviourPun
{
    private const float countdownInterval = 1.2f;
    private float countdownTime = 3.0f;

    private string[] tetrisBlockPrefabsNames;

    private float[] xzRangePositions;
    private int[] blockRotations;

    private const float spawnHeight = 6.5f;

    private float spawnTimer;

    private const int Level0 = 0;
    private const int Level1 = 1;
    private const int Level2 = 2;
    private const int Level3 = 3;

    private int curLevel;

    private int numBlocksSpawned;
    private int level1NumBlocksThreshold = 3; //25
    private int level2NumBlocksThreshold = 5; //50
    private int level3NumBlocksThreshold = 10; //75

    private float currSpawnInterval;
    private const float level0SpawnInterval = 12f;
    private const float level1SpawnInterval = 10f; 
    private const float level2SpawnInterval = 9f;
    private const float level3SpawnInterval = 8f; 

    private const float level0LinearDrag = 20.0f; 
    private const float level1LinearDrag = 18.0f;
    private const float level2LinearDrag = 16.0f;
    private const float level3LinearDrag = 14.0f;

    private const int rotation90 = 90;
    private const int rotation180 = 180;

    private bool firstSpawn = true;

    public TMP_Text levelText;

    public AudioSource levelUp;
    public AudioSource clearLayerSound;

    public GridManager gridManager;
    public GameStateManager gameStateManager;

    private void Start()
    {
        curLevel = Level0;
        numBlocksSpawned = 0;
        currSpawnInterval = level0SpawnInterval; //initial interval

         string[] blockNames = { "I-Block", "J-Block", "L-Block", "S-Block", "Square-Block", "T-Block", "Z-Block" };
        //string[] blockNames = { "J-Block" };
        tetrisBlockPrefabsNames = blockNames;

        int[] rotations = { 0, rotation90, rotation180};
        blockRotations = rotations;

        float[] xzRange = { -1.25f, -0.75f, -0.25f, 0.25f, 0.75f, 1.25f};
        xzRangePositions = xzRange;

        gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
    }
    private void Update() 
    {
        if (PhotonNetwork.IsMasterClient)
        {
            spawnTimer += Time.deltaTime;

            if (countdownTime > 0) 
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
                    if (countdownTime == 0 && firstSpawn) //delete the wall on the first spawned block
                    {
                        GameObject wall = GameObject.FindGameObjectWithTag("InvisibleWall");
                        Destroy(wall);
                        firstSpawn = false; //don't want this to execute more than once
                    }

                    if (gameStateManager.isGameOver())
                    {
                        return;
                    }

                    SpawnRandomBlock();
                    spawnTimer = 0f;

                    numBlocksSpawned++;

                    //After a set number of blocks which have been spawned, move onto next "level"
                    if (numBlocksSpawned > level3NumBlocksThreshold)
                    {
                        if (numBlocksSpawned == level3NumBlocksThreshold + 1) //only play once
                        {
                            levelUp.Play();
                        }
                        curLevel = Level3;
                        currSpawnInterval = level3SpawnInterval;
                        levelText.text = "Level: 3";

                        
                    }
                    else if (numBlocksSpawned > level2NumBlocksThreshold)
                    {
                        if (numBlocksSpawned == level2NumBlocksThreshold + 1)
                        {
                            levelUp.Play();
                        }
                        curLevel = Level2;
                        currSpawnInterval = level2SpawnInterval;
                        levelText.text = "Level: 2";

                    }
                    else if (numBlocksSpawned > level1NumBlocksThreshold)
                    {
                        if (numBlocksSpawned == level1NumBlocksThreshold + 1)
                        {
                            levelUp.Play();
                        }
                        curLevel = Level1;
                        currSpawnInterval = level1SpawnInterval;
                        levelText.text = "Level: 1";
                    }
                }
                
            }

            /**if (gridManager.CheckCompletelyOccupiedAndReset()) //returns true if completely occupied, resets grid dictionary
            {
                ClearLayer();
                clearLayerSound.Play();
            }*/

        }
    }

    private void ClearLayer()
    {
        foreach (string oneTag in tetrisBlockPrefabsNames)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(oneTag);
            foreach (GameObject obj in objectsWithTag)
            {
                if (Mathf.Approximately(obj.transform.position.y, 0.25f))
                {
                    // Remove the object
                    Destroy(obj);
                }
            }
        }
    }

    private void SpawnCountdownBlock()
    {
        string time = "Num" + ((int) countdownTime).ToString();
        Vector3 spawnPosition = new Vector3(-0.75f, spawnHeight, 2.0f);
        GameObject newBlock = PhotonNetwork.Instantiate(time, spawnPosition, Quaternion.identity);
        Rigidbody rb = newBlock.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = true;
            newBlock.transform.rotation = Quaternion.Euler(90, 180, 0);
            rb.constraints = RigidbodyConstraints.FreezeRotationX;
            rb.drag = 3.0f;
        }
    }

    private void SpawnRandomBlock()
    {
        int randomIndex = Random.Range(0, tetrisBlockPrefabsNames.Length);
        string chosenBlockPrefabName = tetrisBlockPrefabsNames[randomIndex];

        //1. Determine drop position
        int randomXIndex = Random.Range(0, xzRangePositions.Length);
        float xPos = xzRangePositions[randomXIndex];
        int randomZIndex = Random.Range(0, xzRangePositions.Length);
        float zPos = xzRangePositions[randomZIndex];

        Vector3 spawnPosition = new Vector3(xPos, spawnHeight, zPos);
        // Debug.Log("Spawn pos" + spawnPosition);
         
        //2. Determine drop rotation
        int randomRotationIndexZ = Random.Range(0, blockRotations.Length);
        int zRotation = blockRotations[randomRotationIndexZ];

        GameObject newBlock = PhotonNetwork.Instantiate(chosenBlockPrefabName, spawnPosition, Quaternion.identity);
        
        Rigidbody rb = newBlock.GetComponent<Rigidbody>();
        if (rb != null)
        {
            newBlock.transform.rotation = Quaternion.Euler(0, 0, zRotation);
            rb.useGravity = true;

            //3. Change the pace at which the object falls based on the level
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
