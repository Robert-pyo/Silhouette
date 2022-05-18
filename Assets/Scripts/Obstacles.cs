using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EObstacleType
{
    Climbable,
    Breakable,
    Movable,
    ETC,
}

public abstract class Obstacles : MonoBehaviour
{
    [Header("Type")]
    public EObstacleType obstacleType;

    [Header("Info")]
    public float mass;
    
    [Header("Climb Info")]
    public Transform[] fourWayToClimb;

    protected Rigidbody obstacleRigidbody;
}
