using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TetrisManager : MonoBehaviour
{
    public TMP_Text scoreText;
    // width, height, depth
    public Vector3 gridSize = new Vector3(10, 10, 10); 

    // 3D grid representation
    private bool[,,] gridData;

    private void setCanvasPoint(){
        scoreText.text = "Score: " + points;
    }

    public LayerMask tetrisBlockLayer;

    private long points = 0;

    public long getPoints(){
        return points;
    }

    public void addPoints(long pointsToAdd){
        points += pointsToAdd;
        setCanvasPoint();
    }

    public void resetPoints(){
        points = 0;
        setCanvasPoint();
    }

    public void subtractPoints(long pointsToSubtract){
        points -= pointsToSubtract;
        setCanvasPoint();
    }

    // Initialization
    private void Awake()
    {
        gridData = new bool[(int)gridSize.x, (int)gridSize.y, (int)gridSize.z];

    }

    public bool IsSpaceOccupied(Vector3 position)
    {
        Vector3 fixedSize = new Vector3(0.5f, 0.5f, 0.5f);
        return Physics.CheckBox(position, fixedSize, Quaternion.identity, tetrisBlockLayer);
    }

    public bool isBottomOccupied(Vector3 position)
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.z; z++)
            {
                if (!gridData[x, 0, z])
                {
                    return false;
                }
            }
        }
        return true;
    }


    public void SetBlockAtGridPosition(Vector3Int position)
    {
        if (IsWithinGridBounds(position))
        {
            gridData[position.x, position.y, position.z] = true;
        }
    }

    public bool IsGameOver()
    {
        if (points < 0){
            return true;
        }

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.z; z++)
            {
                if (gridData[x, (int)gridSize.y - 1, z])
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsWithinGridBounds(Vector3Int position)
    {
        return position.x >= 0 && position.x < gridSize.x &&
               position.y >= 0 && position.y < gridSize.y &&
               position.z >= 0 && position.z < gridSize.z;
    }

    // private void Update()
    // {
        
    // }

}