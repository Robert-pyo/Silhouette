using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Wire : MonoBehaviour
{
    [SerializeField] private Transform m_segmentStart;
    [SerializeField] private Transform m_segmentEnd;
    [SerializeField] private List<Transform> m_segmentList = new List<Transform>();

    private ObjectPool<Segment> m_segmentPool;

    private Rigidbody m_prevBody;
    private Vector3 m_wireDir;
    private ConfigurableJoint m_configJoint;
    private SpringJoint m_springJoint;

    [SerializeField] private GameObject segmentPrefab;

    [SerializeField] private ushort m_segmentCount;
    private float m_segmentPerDistance;
    
    private LineRenderer _line;

    private void Awake()
    {
        if (!m_segmentStart || !m_segmentEnd)
        {
            Debug.LogError($"시작점과 끝점이 정해져있지 않습니다.");
            return;
        }

        m_prevBody = m_segmentStart.GetComponent<Rigidbody>();

        m_wireDir = m_segmentEnd.position - m_segmentStart.position;
        m_segmentPerDistance = m_wireDir.magnitude / m_segmentCount;

        _line = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        CreateWire();
    }

    private void Update()
    {
        for (int i = 0; i < m_segmentList.Count; ++i)
        {
            _line.SetPosition(i, m_segmentList[i].position);
        }
    }

    private void CreateWire()
    {
        for (int i = 0; i < m_segmentCount; ++i)
        {
            GameObject _wireSegment = Instantiate(segmentPrefab, m_prevBody.position + m_wireDir.normalized * m_segmentPerDistance, Quaternion.identity);
            _wireSegment.transform.parent = transform;
            m_prevBody = _wireSegment.GetComponent<Rigidbody>();

            m_segmentList.Add(_wireSegment.transform);
        }

        _line.positionCount = m_segmentCount;
        m_prevBody = m_segmentStart.GetComponent<Rigidbody>();

        for (int i = 0; i < m_segmentCount; ++i)
        {
            m_configJoint = m_segmentList[i].GetComponent<ConfigurableJoint>();
            m_springJoint = m_segmentList[i].GetComponent<SpringJoint>();

            if (i > m_segmentCount / 2)
            {
                if (i == m_segmentCount - 1)
                {
                    m_prevBody = m_segmentEnd.GetComponent<Rigidbody>();
                    m_configJoint.connectedBody = m_prevBody;
                    m_springJoint.connectedBody = m_prevBody;
                    break;
                }

                m_prevBody = m_segmentList[i + 1].GetComponent<Rigidbody>();
                m_configJoint.connectedBody = m_prevBody;
                m_springJoint.connectedBody = m_prevBody;
                continue;
            }
            else if (i == m_segmentCount / 2)
            {
                m_configJoint.connectedBody = m_prevBody;
                m_springJoint.connectedBody = m_segmentList[i + 1].GetComponent<Rigidbody>();
                continue;
            }

            m_configJoint.connectedBody = m_prevBody;
            m_springJoint.connectedBody = m_prevBody;
            m_prevBody = m_segmentList[i].GetComponent<Rigidbody>();
        }

        m_configJoint.connectedBody = m_segmentEnd.GetComponent<Rigidbody>();
    }

    // private void CreateWire()
    // {
    //     GameObject _start = Instantiate(segmentPrefab, m_segmentStart.position, Quaternion.identity);
    //     _start.transform.SetParent(transform);

    //     HingeJoint _startJoint = _start.GetComponent<HingeJoint>();
    //     _startJoint.connectedBody = fixedBody;

    //     m_segmentList.Add(_start.transform);
    //     m_prevBody = _start.GetComponent<Rigidbody>();

    //     for (int i = 0; i < m_segmentCount - 1; ++i)
    //     {
    //         GameObject _segment = Instantiate(segmentPrefab, 
    //         m_prevBody.transform.position + new Vector3(0f, 0f, m_segmentDistance), 
    //         Quaternion.identity);
    //         _segment.transform.SetParent(transform);

    //         HingeJoint _joint = _segment.GetComponent<HingeJoint>();
    //         _joint.connectedBody = m_prevBody;
    //         m_prevBody = _segment.GetComponent<Rigidbody>();

    //         m_segmentList.Add(_segment.transform);
    //     }

    //     // 마지막 인덱스 넣어줌
    //     m_segmentEnd = m_segmentList[m_segmentList.Count - 1];
    // }

    // private IEnumerator AddSegmentCoroutine()
    // {
    //     yield return new WaitForSeconds(0.3f);
    //     AddSegment();
    // }

    // private void AddSegment()
    // {
    //     float _segDist = (m_segmentList[m_segmentCount - 2].position - m_segmentEnd.position).sqrMagnitude;
    //     if (_segDist < m_segmentDistance * m_segmentDistance)
    //     {
    //         return;
    //     }

    //     GameObject _segment = Instantiate(segmentPrefab, m_prevBody.transform.position, Quaternion.identity);
    //     _segment.transform.SetParent(transform);

    //     HingeJoint _joint = _segment.GetComponent<HingeJoint>();
    //         _joint.connectedBody = m_prevBody;
    //         m_prevBody = _segment.GetComponent<Rigidbody>();

    //     m_segmentList.Add(_segment.transform);
    //     m_segmentEnd = _segment.transform;
    //     m_segmentCount++;
    // }
}
