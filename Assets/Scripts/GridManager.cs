﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static UnityEngine.Rendering.DebugUI;

public class GridManager : MonoBehaviourPun
{
    // private const int gridSize = 10;
    private Dictionary<ValueTuple<float, float>, bool> grid;

    private int occupiedCounter = 0; // To keep track of occupied spaces

    public void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        grid = new Dictionary<ValueTuple<float, float>, bool>();
        for (float x = -2.25f; x <= 2.25f; x += 0.5f)
        {
            for (float z = -2.25f; z <= 2.25f; z += 0.5f)
            {
                grid[(x, z)] = false; // Initially, no cells are occupied
            }
        }
    }

    public bool isOccupied(Transform transform) //assume it is valid
    {
        float x = RoundToTwoDecimalPlaces(transform.position.x); 
        float z = RoundToTwoDecimalPlaces(transform.position.z);
        return grid[(x, z)];
    }

    public bool isValidTransform(Transform transform)
    {
        float x = RoundToTwoDecimalPlaces(transform.position.x);
        float z = RoundToTwoDecimalPlaces(transform.position.z);
        Debug.Log("X position in gridManager isValidTransform: " + x); 
        Debug.Log("Z position in gridManager isValidTransform: " + z);
        Debug.Log("isValidTransform result: " + grid.ContainsKey((x, z)));

        return grid.ContainsKey((x, z));
    }

    public void setPositionOccupied(Transform transform) //assume it is valid
    {
        float x = RoundToTwoDecimalPlaces(transform.position.x);
        float z = RoundToTwoDecimalPlaces(transform.position.z);

            if (!grid[(x, z)])
            {
                grid[(x, z)] = true;
                occupiedCounter++;
            Debug.Log("setPositionOccupied in Grid class called");
            }            
    }

    public bool CheckCompletelyOccupiedAndReset()
    {
        if (occupiedCounter == 12) //TODO: check that it works twice. 100 is actual number
        {
            InitializeGrid();
            occupiedCounter = 0;
            return true;
        }
        return false;
    }

    private float RoundToTwoDecimalPlaces(float number) //round to the tenth's place
    {
        return Mathf.Round(number * 100f) / 100f;
    }
}