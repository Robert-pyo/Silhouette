using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMovement
{
    protected float moveSpeed;
    protected float rotateSpeed;
    
    public abstract void Execute(Vector3 dest);
    public abstract void Rotate();
}
