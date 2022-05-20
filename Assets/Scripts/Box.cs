using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Obstacles
{
    private void Awake()
    {
        obstacleType = EObstacleType.Climbable;
    }
}
