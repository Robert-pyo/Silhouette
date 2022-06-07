using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Obstacles
{
    private void Awake()
    {
        obstacleType = EObstacleType.Climbable;

        m_obstacleRigidbody = GetComponent<Rigidbody>();
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (!collision.gameObject.CompareTag("Player") &&
    //        !collision.gameObject.CompareTag("Enemy")) return;

    //    m_obstacleRigidbody.isKinematic = true;
    //}

    //private void OnCollisionStay(Collision collision)
    //{
    //    m_obstacleRigidbody.isKinematic = true;
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (!collision.gameObject.CompareTag("Player") &&
    //        !collision.gameObject.CompareTag("Enemy")) return;

    //    m_obstacleRigidbody.isKinematic = false;
    //}
}
