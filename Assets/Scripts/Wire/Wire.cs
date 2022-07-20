using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Wire : MonoBehaviour
{
    public Transform segmentStart;
    public Transform segmentEnd;
    [SerializeField] private List<Segment> m_segmentList = new List<Segment>();

    private Rigidbody m_prevBody;
    private Vector3 m_wireDir;
    private ConfigurableJoint m_configJointPrev;
    private ConfigurableJoint m_configJointNext;

    [SerializeField] private Segment segmentPrefab;

    [SerializeField] private ushort m_segmentCount;
    private float m_segmentPerDistance;
    private float m_maxDistance = 1f;
    private WaitForSeconds m_segmentDistCalcTime;
    
    private LineRenderer _line;

    private ObjectPool<Segment> m_segmentPool;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
        
        GameManager.Instance.onWireCreate += CreateWire;

        m_segmentDistCalcTime = new WaitForSeconds(0.5f);

        //m_segmentPool = new ObjectPool<Segment>(
        //    createFunc: () => Instantiate(segmentPrefab, new Vector3(-1000f, -1000f, -1000f), Quaternion.identity),
        //    actionOnGet: (segment) =>
        //    {
        //        if (!segment) return;
        //        segment.gameObject.SetActive(true);
        //    },
        //    actionOnRelease: (segment) =>
        //    {
        //        if (!segment) return;
        //        segment.gameObject.SetActive(false);
        //    },
        //    actionOnDestroy: (segment) =>
        //    {
        //        if (!segment) return;
        //        Destroy(segment.gameObject);
        //    }, maxSize: 100);
    }

    private void Update()
    {
        for (int i = 0; i < m_segmentList.Count; ++i)
        {
            _line.SetPosition(i, m_segmentList[i].transform.position);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator FillSegments()
    {
        while (true)
        {
            for (int i = 1; i < m_segmentList.Count - 1; ++i)
            {
                Vector3 _dir = m_segmentList[i + 1].transform.position - m_segmentList[i].transform.position;
                if (m_maxDistance * m_maxDistance < _dir.sqrMagnitude)
                {
                    m_segmentCount++;
                    float _targetDist = _dir.magnitude / 2;

                    Segment _wireSegment = Instantiate(segmentPrefab, m_segmentList[i].transform.position + _dir.normalized * _targetDist, Quaternion.identity);
                    _wireSegment.transform.parent = transform;

                    _wireSegment.myConfigJointPrev.connectedBody = m_segmentList[i].myRigidbody;
                    _wireSegment.myConfigJointNext.connectedBody = m_segmentList[i + 1].myRigidbody;

                    m_segmentList[i].myConfigJointNext.connectedBody = _wireSegment.myRigidbody;

                    // 마지막 세그먼트는 end에 붙어있어야 하므로 제외
                    if (i + 1 < m_segmentList.Count - 1)
                    {
                        m_segmentList[i + 1].myConfigJointPrev.connectedBody = _wireSegment.myRigidbody;
                    }

                    m_segmentList.Insert(i + 1, _wireSegment);
                    _line.positionCount = m_segmentCount;
                    _line.SetPosition(_line.positionCount - 1, m_segmentList[_line.positionCount - 1].transform.position);
                    break;
                }
            }

            yield return m_segmentDistCalcTime;
        }
    }

    private void CreateWire()
    {
        if (!segmentStart || !segmentEnd)
        {
            Debug.LogError($"시작점과 끝점이 지정되어 있지 않습니다.");
            return;
        }

        GameManager.Instance.onWireCreate -= CreateWire;
        m_prevBody = segmentStart.GetComponent<Rigidbody>();
        m_wireDir = segmentEnd.position - segmentStart.position;
        m_segmentPerDistance = m_wireDir.magnitude / m_segmentCount;

        for (int i = 0; i < m_segmentCount; ++i)
        {
            Segment _wireSegment = Instantiate(segmentPrefab, m_prevBody.position + m_wireDir.normalized * m_segmentPerDistance, Quaternion.identity);
            _wireSegment.transform.parent = transform;
            m_prevBody = _wireSegment.myRigidbody;

            m_segmentList.Add(_wireSegment);
        }

        m_prevBody = segmentStart.GetComponent<Rigidbody>();
        _line.positionCount = m_segmentCount;

        m_configJointPrev = m_segmentList[0].myConfigJointPrev;
        m_configJointNext = m_segmentList[0].myConfigJointNext;
        m_configJointPrev.connectedBody = m_prevBody;
        m_configJointNext.connectedBody = m_prevBody;
        m_prevBody = m_segmentList[0].myRigidbody;

        for (int i = 1; i < m_segmentCount - 1; ++i)
        {
            m_configJointPrev = m_segmentList[i].myConfigJointPrev;
            m_configJointNext = m_segmentList[i].myConfigJointNext;

            m_configJointPrev.connectedBody = m_prevBody;
            m_configJointNext.connectedBody = m_segmentList[i + 1].myRigidbody;
            m_prevBody = m_segmentList[i].myRigidbody;
        }

        m_configJointPrev = m_segmentList[m_segmentCount - 1].myConfigJointPrev;
        m_configJointNext = m_segmentList[m_segmentCount - 1].myConfigJointNext;
        m_configJointPrev.connectedBody = segmentEnd.GetComponent<Rigidbody>();
        m_configJointNext.connectedBody = segmentEnd.GetComponent<Rigidbody>();
        //m_prevBody = null;

        StartCoroutine(FillSegments());
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
