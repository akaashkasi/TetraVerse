using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class SnapManager : MonoBehaviourPunCallbacks
{
    public Transform snapPoint;  // Assign the snap point transform in the Inspector.
    public Vector3 gridSize = new Vector3(0.5f, 0.5f, 0.5f); // Size of grid cells.
    public LayerMask floorLayer;

    // Define the grid positions where Tetris blocks can snap.
    private Vector3[] snapPositions;

    private void Start()
    {
        // Initialize the grid positions.
        InitializeSnapPositions();
    }

    private void InitializeSnapPositions()
    {
        // Calculate and store the grid positions based on your grid size.
        int gridSizeX = 5;
        int gridSizeZ = 5;
        snapPositions = new Vector3[gridSizeX * gridSizeZ];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                float xPos = x * gridSize.x - (gridSizeX - 1) * gridSize.x / 2f;
                float zPos = z * gridSize.z - (gridSizeZ - 1) * gridSize.z / 2f;
                snapPositions[x * gridSizeZ + z] = new Vector3(xPos, 0f, zPos);
            }
        }

    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        XRGrabInteractable[] tetrisBlocks = FindObjectsOfType<XRGrabInteractable>();
        foreach (XRGrabInteractable tetrisBlock in tetrisBlocks)
        {
            if (IsTouchingFloor(tetrisBlock))
            {
                SnapToGrid(tetrisBlock);
            }
        }

    }

    private bool IsTouchingFloor(XRGrabInteractable tetrisBlock)
    {
        // Use your logic to determine if the Tetris block is touching the floor.
        // You can use a raycast from the XR Socket Interactor's position.
        // Example:
        RaycastHit hit;
        if (Physics.Raycast(snapPoint.position, Vector3.down, out hit, 0.1f, floorLayer))
        {
            return true;
        }
        return false;
    }

    private void SnapToGrid(XRGrabInteractable tetrisBlock)
    {
        Transform tetrisTransform = tetrisBlock.transform;

        // Determine the closest grid position to the Tetris block's current position.
        Vector3 closestSnapPosition = FindClosestSnapPosition(tetrisTransform.position);

        // Snap the Tetris block to the closest grid position.
        tetrisTransform.position = closestSnapPosition;

        // You may also want to disable grabbing after snapping.
        tetrisBlock.enabled = false;

        // Synchronize the snapped position across the network.
        photonView.RPC("SyncSnap", RpcTarget.AllBuffered, closestSnapPosition);

    }

    // ...

    private Vector3 FindClosestSnapPosition(Vector3 currentPosition)
    {
        Vector3 closestPosition = snapPositions[0];
        float closestDistance = Vector3.Distance(currentPosition, closestPosition);

        foreach (Vector3 snapPosition in snapPositions)
        {
            float distance = Vector3.Distance(currentPosition, snapPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPosition = snapPosition;
            }
        }

        return closestPosition;

    }

    [PunRPC]
    private void SyncSnap(Vector3 newPosition)
    {
        // Apply the synchronized position to all Tetris blocks.
        XRGrabInteractable[] tetrisBlocks = FindObjectsOfType<XRGrabInteractable>();
        foreach (XRGrabInteractable tetrisBlock in tetrisBlocks)
        {
            tetrisBlock.transform.position = newPosition;
        }

    }
}
