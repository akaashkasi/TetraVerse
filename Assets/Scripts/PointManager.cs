using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PointManager : MonoBehaviourPun
{
    public TMP_Text scoreText;

    private void setCanvasPoint()
    {
        scoreText.text = "Score: " + points;
    }

    private long points = 0;

    public long getPoints()
    {
        return points;
    }

    private void addPoints(long pointsToAdd)
    {
        points += pointsToAdd;
        setCanvasPoint();
    }

    // user pass to others
    public void addCollaborationPoints()
    {
        addPoints(10);
    }

    // complete a layer
    public void addLayerPoints()
    {
        addPoints(100);
    }

    // 
    public void addTetrisPoints()
    {
        addPoints(5);
    }

    public void addUserInteractionPoints()
    {
        addPoints(5);
    }


    public void resetPoints()
    {
        points = 0;
        setCanvasPoint();
    }

    public void subtractPoints(long pointsToSubtract)
    {
        points -= pointsToSubtract;
        setCanvasPoint();
    }

}