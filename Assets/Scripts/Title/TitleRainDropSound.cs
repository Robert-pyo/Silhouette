using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleRainDropSound : MonoBehaviour
{
    private ParticleSystem m_rainParticle;
    private List<ParticleCollisionEvent> m_collisionEvents;

    private float m_lastCollidedTime;

    private void Awake()
    {
        m_rainParticle = GetComponent<ParticleSystem>();
        m_collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (Time.time <= m_rainParticle.main.startLifetime.constant + m_lastCollidedTime) return;
        
        m_lastCollidedTime = Time.time;

        int _numCollisionEvents = m_rainParticle.GetCollisionEvents(other, m_collisionEvents);

        for (int i = 0; i < _numCollisionEvents; ++i)
        {
            SoundWaveManager.Instance.GenerateSoundWave(
                other.transform, 
                m_collisionEvents[i].intersection, 
                m_collisionEvents[i].normal, 
                7f);
        }
    }
}
