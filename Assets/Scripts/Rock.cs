using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Rock : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private bool m_isGhost;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Init(Vector3 velocity, bool isGhost)
    {
        m_isGhost = isGhost;
        _rigidbody.AddForce(velocity, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || 
        other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            // 부딪힐 때의 현재 rigidbody의 velocity 크기만큼 power 설정
            if (m_isGhost) return;

            //print(other.GetContact(0).point);
            SoundWaveManager.Instance.GenerateSoundWave(
                other.transform, other.GetContact(0).point, other.GetContact(0).normal, _rigidbody.velocity.sqrMagnitude);
        }
    }
}
