using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDirectionIndicator : MonoBehaviour
{
    private Transform m_parent;

    private void Awake()
    {
        m_parent = transform.parent.transform;
        
        //CameraManager.Instance.onCameraDirectionReset.AddListener(Reset);
    }

    private void Update()
    {
        Reset();
    }

    private void Reset()
    {
        transform.localRotation = Quaternion.Euler(m_parent.rotation.eulerAngles.x * (-1), 0f, 0f);
        //transform.localRotation = Quaternion.Euler(0f, m_parent.rotation.eulerAngles.y, 0f);
    }
}
