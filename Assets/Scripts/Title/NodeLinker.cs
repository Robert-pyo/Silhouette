using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeLinker : MonoBehaviour
{
    public List<LevelNode> nodeList;

    [SerializeField] private GameObject waypointPrefab;

    [SerializeField] private float m_waypointGap;

    private void Awake()
    {
        LinkNodes();
    }

    private void LinkNodes()
    {
        for (int i = 0; i < nodeList.Count; ++i)
        {
            if (i == nodeList.Count - 1) break;
            
            Vector3 _dir = nodeList[i + 1].endPos.position - nodeList[i].startPos.position;
            float _dist = _dir.magnitude;
            int _count = (int)(_dist / m_waypointGap);
            
            print(_count);
            
            GameObject _waypointObj = Instantiate(waypointPrefab, nodeList[i].startPos.position + _dir.normalized * m_waypointGap,
                Quaternion.LookRotation(new Vector3(_dir.x, 0f, _dir.z)));
            _waypointObj.transform.parent = transform;

            for (int j = 0; j < _count - 1; ++j)
            {
                _waypointObj = Instantiate(waypointPrefab, _waypointObj.transform.position + _dir.normalized * m_waypointGap,
                    Quaternion.LookRotation(new Vector3(_dir.x, 0f, _dir.z)));
                _waypointObj.transform.parent = transform;
            }
        }
    }
}
