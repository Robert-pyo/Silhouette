using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.IO;
using Michsky.UI.ModernUIPack;

struct NodePoint
{
    public int x;
    public int y;

    public NodePoint(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class RandomMazeGenerator : MonoBehaviour
{
    [SerializeField] private int m_width;
    [SerializeField] private int m_height;

    private List<Node> m_grid = new List<Node>();

    [SerializeField] private GameObject m_floorPrefab;
    [SerializeField] private GameObject m_wallPrefab;
    //[SerializeField] private GameObject m_rightWallPrefab;
    //[SerializeField] private GameObject m_leftWallPrefab;

    private List<GameObject> m_wallList = new List<GameObject>();

    private const float GRID_X_OFFSET = 6f;
    private const float GRID_Z_OFFSET = 6f;

    private Node m_currentNode;

    private GameObject m_parentObject;
    //private SaveData m_saveData = new SaveData();
    private string m_filePath;

    public CustomInputField mazeWidthInput;
    public CustomInputField mazeHeightInput;

    private void Awake()
    {
        // if (!m_floorPrefab) return;
        //
        // for (int i = 0; i < m_height; ++i)
        // {
        //     for (int j = 0; j < m_width; ++j)
        //     {
        //         // floor 생성
        //         Instantiate(m_floorPrefab, new Vector3(j * GRID_X_OFFSET, 0f, i * GRID_Z_OFFSET), Quaternion.identity).transform.SetParent(transform);
        //     }
        // }
        
        m_parentObject = new GameObject("Maze");

        // 리스트에 노드 추가하고 셔플
        InsertRandomNodeToList();
        // 리스트에 추가된 각 노드들의 이웃 노드 연결
        LinkNodes();

        int _startX = Random.Range(0, m_width);
        int _startY = Random.Range(0, m_height);

        //print(_startY * m_width + _startX);
        //for (int i = 0; i < m_grid.Count; ++i)
        //{
        //    print(m_grid[i].index);
        //}

        GenerateWalls();
        RecursiveGrowingTreeNewest(_startX, _startY);

        m_filePath = $"{Application.dataPath}/MazeData/";
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void ResetMaze()
    {
        m_grid.Clear();
        m_wallList.Clear();
        Destroy(m_parentObject);

        int _newWidth = Convert.ToInt32(mazeWidthInput.inputText.text);
        int _newHeight = Convert.ToInt32(mazeHeightInput.inputText.text);
        m_width = _newWidth;
        m_height = _newHeight;

        m_parentObject = new GameObject("Maze");

        // 리스트에 노드 추가하고 셔플
        InsertRandomNodeToList();
        // 리스트에 추가된 각 노드들의 이웃 노드 연결
        LinkNodes();

        int _startX = Random.Range(0, _newWidth);
        int _startY = Random.Range(0, _newHeight);

        GenerateWalls();
        RecursiveGrowingTreeNewest(_startX, _startY);
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
            if (i + m_width < m_grid.Count)
            {
                m_grid[i].upNode = m_grid[i + m_width];
                //Debug.Log($"{i} - UP : {m_grid[i].upNode.index}");
            }
            
            if (i - m_width >= 0)
            {
                m_grid[i].downNode = m_grid[i - m_width];
                //Debug.Log($"{i} - DOWN : {m_grid[i].downNode.index}");
            }

            if (i - 1 >= 0 && i % m_width > 0)
            {
                m_grid[i].leftNode = m_grid[i - 1];
                //Debug.Log($"{i} - LEFT : {m_grid[i].leftNode.index}");
            }

            if (i + 1 < m_grid.Count && i % m_width < m_width - 1)
            {
                m_grid[i].rightNode = m_grid[i + 1];
                //Debug.Log($"{i} - RIGHT : {m_grid[i].rightNode.index}");
            }
        }
    }
    
    private Node SelectNewestNode(params Node[] nodes)
    {
        int _newestValue = 0;
        Node _newestNode = null;
        
        foreach (Node _node in nodes)
        {
            if (_node == null) continue;
            if (_node.isVisited) continue;

            if (_node.index < _newestValue) continue;
            
            _newestValue = _node.index;
            _newestNode = _node;

            if (m_currentNode.upNode?.index == _newestValue)
            {
                m_currentNode.nodeState = ENodeDirState.Up;
                continue;
            }
            
            if (m_currentNode.downNode?.index == _newestValue)
            {
                m_currentNode.nodeState = ENodeDirState.Down;
                continue;
            }

            if (m_currentNode.leftNode?.index == _newestValue)
            {
                m_currentNode.nodeState = ENodeDirState.Left;
                continue;
            }

            if (m_currentNode.rightNode?.index == _newestValue)
            {
                m_currentNode.nodeState = ENodeDirState.Right;
                continue;
            }
        }

        return _newestNode;
    }

    private void RecursiveGrowingTreeNewest(int startX, int startY)
    {
        int _nodeIndex = m_width * startY + startX;

        Node _prevNode = m_currentNode;
        m_currentNode = m_grid[_nodeIndex];

        // 시작 정점에 방문
        if (!m_currentNode.isVisited)
        {
            m_currentNode.isVisited = true;
        }

        // 근접한 정점 중 가장 최신에 등록된(Index가 가장 큰) Node 선정
        Node _newestNeighbor = SelectNewestNode(m_currentNode.upNode, m_currentNode.downNode,
                m_currentNode.leftNode, m_currentNode.rightNode);

        // 이미 방문한 적이 있고 이웃 노드들도 이미 방문한 적이 있을 때
        if (_newestNeighbor == null) return;

        DeletePassThroughWalls(_nodeIndex, m_currentNode);
        
        m_currentNode.connectedNode = _newestNeighbor;
        
        Debug.Log($"Index - {_nodeIndex} : {startX}, {startY}");

        NodePoint _newest = SelectNewsetNodePoint(_nodeIndex);

        RecursiveGrowingTreeNewest(_newest.x, _newest.y);

        m_currentNode = m_grid[_nodeIndex];

        _newestNeighbor = SelectNewestNode(m_currentNode.upNode, m_currentNode.downNode,
                m_currentNode.leftNode, m_currentNode.rightNode);

        if (_newestNeighbor == null) return;

        DeletePassThroughWalls(_nodeIndex, m_currentNode);

        _newest = SelectNewsetNodePoint(_nodeIndex);
        RecursiveGrowingTreeNewest(_newest.x, _newest.y);
    }

    private NodePoint SelectNewsetNodePoint(int index)
    {
        int _newestX = 0;
        int _newestY = 0;
        switch (m_currentNode.nodeState)
        {
            case ENodeDirState.Up:
                {
                    _newestX = (index + m_width) % m_width;
                    _newestY = ((index + m_width) / m_width) % m_height;
                }
                break;

            case ENodeDirState.Down:
                {
                    _newestX = (index - m_width) % m_width;
                    _newestY = ((index - m_width) / m_width) % m_height;
                }
                break;

            case ENodeDirState.Left:
                {
                    _newestX = (index - 1) % m_width;
                    _newestY = ((index - 1) / m_width) % m_height;
                }
                break;

            case ENodeDirState.Right:
                {
                    _newestX = (index + 1) % m_width;
                    _newestY = ((index + 1) / m_width) % m_height;
                }
                break;
        }

        return new NodePoint(_newestX, _newestY);
    }

    private void GenerateWalls()
    {
        for (int i = 0; i < m_height; ++i)
        {
            for (int j = 0; j < m_width; ++j)
            {
                // floor 생성
                GameObject _obj = Instantiate(m_wallPrefab, new Vector3(j * GRID_X_OFFSET, 0f, i * GRID_Z_OFFSET), Quaternion.identity);
                _obj.transform.parent = m_parentObject.transform;

                m_wallList.Add(_obj);
            }
        }
    }

    private void DeletePassThroughWalls(int index, Node curNode)
    {
        if (m_wallList.Count < 1) return;

        switch (curNode.nodeState)
        {
            case ENodeDirState.Up:
                {
                    print("Up");
                    Destroy(m_wallList[index].transform.Find("Forward").gameObject);
                    Destroy(m_wallList[index + m_width].transform.Find("Back").gameObject);
                    //Instantiate(m_floorPrefab, m_wallList[index].transform.position, Quaternion.identity);
                }
                break;
            case ENodeDirState.Down:
                {
                    print("Down");
                    Destroy(m_wallList[index].transform.Find("Back").gameObject);
                    Destroy(m_wallList[index - m_width].transform.Find("Forward").gameObject);
                    //Instantiate(m_floorPrefab, m_wallList[index].transform.position, Quaternion.identity);
                }
                break;
            case ENodeDirState.Left:
                {
                    print("Left");
                    Destroy(m_wallList[index].transform.Find("Left").gameObject);
                    Destroy(m_wallList[index - 1].transform.Find("Right").gameObject);
                    //Instantiate(m_floorPrefab, m_wallList[index].transform.position, Quaternion.identity);
                }
                break;
            case ENodeDirState.Right:
                {
                    print("Right");
                    Destroy(m_wallList[index].transform.Find("Right").gameObject);
                    Destroy(m_wallList[index + 1].transform.Find("Left").gameObject);
                    //Instantiate(m_floorPrefab, m_wallList[index].transform.position, Quaternion.identity);
                }
                break;
            case ENodeDirState.None:
                print("None");
                break;
            default:
                break;
        }
    }
}
