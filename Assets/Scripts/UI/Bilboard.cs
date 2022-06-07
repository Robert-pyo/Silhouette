using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bilboard : MonoBehaviour
{
    [SerializeField] private Camera m_camera;

    private void Awake()
    {
        if (!m_camera)
        {
            m_camera = Camera.main;
        }
    }

    private void Update()
    {
        transform.LookAt(m_camera.transform);
    }
}
