using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GridManager : MonoBehaviourPun
{
    private int[,] publicGrid;
    private int width = 10;
    private int depth = 10;

    private int layersCompleted = 0;

    private void Start()
    {

        int[,] grid = new int[width, depth];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                grid[i, j] = 0;
            }
        }
        publicGrid = grid;
    }

    public void SetPositionOccupied(int x, int z)
    {
        publicGrid[x, z] = 1;
    }

    public void ClearLayer()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                publicGrid[i, j] = 0;
            }
        }
        layersCompleted++;
    }

    public bool FloorCompletelyOccupied()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                if (publicGrid[i, j] == 0)
                    return false;
            }
        }
        return true;
    }

}

