using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameStateManager : MonoBehaviourPun
{
    public TMP_Text gameStateText;

    private PointManager pointManager;

    private bool gameOver = false;

    public void Start()
    {
        pointManager = GameObject.Find("PointManager").GetComponent<PointManager>();

        gameStateText.text = "Game on";
    }

    public void Update() {
        if (pointManager.getPoints() >= 600) {
            gameOver = true;
        }
        if (gameOver) {
            gameStateText.text = "Game Over";
        }
    }

    public bool isGameOver() {
        return gameOver;
    }

    public void restart(){
        // delete all blocks in the scene
        gameOver = false;
        pointManager.resetPoints();
        gameStateText.text = "Game on";
    }
}
