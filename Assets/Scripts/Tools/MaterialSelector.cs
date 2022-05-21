using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialSelector : MonoBehaviour
{
    [SerializeField] private Material m_targeMat;
    private Renderer[] m_childrenMats;

    private void Update()
    {
        m_childrenMats = GetComponentsInChildren<Renderer>();
        
        if (m_childrenMats.Length <= 0) return;
        
        foreach (Renderer _renderer in m_childrenMats)
        {
            _renderer.material = m_targeMat;
        }
    }
}
