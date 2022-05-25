using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RandomMazeGenerator : MonoBehaviour
{
    [SerializeField] private int m_width;
    [SerializeField] private int m_height;
    private int m_seed;

    private List<Node> m_grid = new List<Node>();

    [SerializeField] private GameObject m_floorPrefab;
    [SerializeField] private GameObject m_wallPrefab;
    private const float GRID_X_OFFSET = 6f;
    private const float GRID_Z_OFFSET = 6f;

    private Node m_currentNode;

    private void Awake()
    {
        m_seed = (int)(Time.time * 123f);
        
        // 랜덤 시드 부여
        Random.InitState(m_seed);

        if (!m_floorPrefab) return;
        
        for (int i = 0; i < m_height; ++i)
        {
            for (int j = 0; j < m_width; ++j)
            {
                // floor 생성
                Instantiate(m_floorPrefab, new Vector3(j * GRID_X_OFFSET, 0f, i * GRID_Z_OFFSET), Quaternion.identity).transform.SetParent(transform);
            }
        }
        
        // 리스트에 노드 추가하고 셔플
        InsertRandomNodeToList();
        // 리스트에 추가된 각 노드들의 이웃 노드 연결
        LinkNodes();

        int _x = Random.Range(0, m_width);
        int _y = Random.Range(0, m_height);
        
        GrowingTreeAlgorithm(_x, _y);
    }

    private void InsertRandomNodeToList()
    {
        for (int i = 0; i < m_height; ++i)
        {
            for (int j = 0; j < m_width; ++j)
            {
                m_grid.Add(new Node());
            }
        }
        
        for (int i = 0; i < m_grid.Count; ++i)
        {
            int _rndNum = Random.Range(0, m_grid.Count);
            (m_grid[i], m_grid[_rndNum]) = (m_grid[_rndNum], m_grid[i]);
        }
    }

    private void LinkNodes()
    {
        for (int i = 0; i < m_grid.Count; ++i)
        {
            if (i - m_width >= 0)
            {
                m_grid[i].upNode = m_grid[i - m_width];
                Debug.Log($"{i}번 : {m_grid[i].upNode.index}");
            }

            if (i + m_width < m_grid.Count)
            {
                m_grid[i].downNode = m_grid[i + m_width];
                Debug.Log($"{i}번 : {m_grid[i].downNode.index}");
            }

            if (i - 1 >= 0)
            {
                m_grid[i].leftNode = m_grid[i - 1];
                Debug.Log($"{i}번 : {m_grid[i].leftNode.index}");
            }

            if (i + 1 < m_grid.Count)
            {
                m_grid[i].rightNode = m_grid[i + 1];
                Debug.Log($"{i}번 : {m_grid[i].rightNode.index}");
            }
        }
    }

    private void GrowingTreeAlgorithm(int startX, int startY)
    {
        int _nodeIndex = m_width * startY - (m_width - startX) - 1;

        if (m_grid[_nodeIndex].isVisited)
        {
            Node _targetNode = SelectNewestNode(m_grid[_nodeIndex].upNode, m_grid[_nodeIndex].downNode,
                m_grid[_nodeIndex].leftNode, m_grid[_nodeIndex].rightNode);

            if (_targetNode == null) return;

            int _targetX = (_targetNode.index / m_width) % m_height;
            int _targetY = _targetNode.index % m_width;
            GrowingTreeAlgorithm(_targetX, _targetY);

            return;
        }
        
        // 시작 정점에 방문
        m_grid[_nodeIndex].isVisited = true;
    }

    private Node SelectNewestNode(params Node[] nodes)
    {
        int _newestValue = 0;
        Node _newestNode = null;
        
        foreach (Node _node in nodes)
        {
            if (_node == null) continue;
            if (_node.isVisited) continue;

            if (_node.index <= _newestValue) continue;
            
            _newestValue = _node.index;
            _newestNode = _node;
        }

        return _newestNode;
    }
}
