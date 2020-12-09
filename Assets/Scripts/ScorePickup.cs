using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePickup : MonoBehaviour
{
    public int score = 5;

    public void Pickup(out int scoreToAssign)
    {
        scoreToAssign = score;
        Destroy(gameObject);
    }
}
