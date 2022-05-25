using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.IO;

public class RandomMazeGenerator : MonoBehaviour
{
    [SerializeField] private int m_width;
    [SerializeField] private int m_height;

    private List<Node> m_grid = new List<Node>();

    //[SerializeField] private GameObject m_floorPrefab;
    [SerializeField] private GameObject m_straightWallPrefab;
    [SerializeField] private GameObject m_rightWallPrefab;
    [SerializeField] private GameObject m_leftWallPrefab;

    private const float GRID_X_OFFSET = 6f;
    private const float GRID_Z_OFFSET = 6f;

    private Node m_currentNode;

    private GameObject m_parentObject;
    private SaveData m_saveData = new SaveData();
    private string m_filePath;

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

        print($"{_startX}, {_startY}");
        //print(_startY * m_width + _startX);
        for (int i = 0; i < m_grid.Count; ++i)
        {
            print(m_grid[i].index);
        }
        
        RecursiveGrowingTreeNewest(_startX, _startY);
        
        m_filePath = $"{Application.dataPath}/MazeData/";
    }

    // private void Start()
    // {
    //     SaveMaze();
    // }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SaveMaze();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            LoadMaze();
        }
    }

    public void SaveMaze()
    {
        m_saveData.objs.Add(gameObject);
        // foreach (Transform _child in transform.GetComponentsInChildren<Transform>())
        // {
        //     m_saveData.objs.Add(_child.gameObject);
        // }

        string _saveData = JsonUtility.ToJson(m_saveData, true);

        int _count = 0;
        string _fileName = Path.Combine(m_filePath, $"MazeData_{_count}.json");
        while (File.Exists(_fileName))
        {
            _count++;
            _fileName = Path.Combine(m_filePath, $"MazeData_{_count}.json");
        }

        File.WriteAllText(_fileName, _saveData);
    }

    public void LoadMaze()
    {
        string _fileName = Path.Combine(m_filePath, $"MazeData_0.json");
        string _jsonData = File.ReadAllText(_fileName);
        SaveData _data = JsonUtility.FromJson<SaveData>(_jsonData);
        
        Instantiate(_data.objs[0], Vector3.zero, Quaternion.identity);

        foreach (GameObject _obj in _data.objs)
        {
            Instantiate(_obj, Vector3.zero, Quaternion.identity);
        }
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

        // 근접한 정점 중 가장 최신에 등록된(Index가 가장 큰) Node 선정
        Node _newestNeighbor = SelectNewestNode(m_currentNode.upNode, m_currentNode.downNode,
                m_currentNode.leftNode, m_currentNode.rightNode);

        // 이미 방문한 적이 있고 이웃 노드들도 이미 방문한 적이 있을 때
        if (_newestNeighbor == null) return;
        
        GenerateWalls(_prevNode, m_currentNode, startX, startY);
        
        // 시작 정점에 방문
        if (!m_currentNode.isVisited)
        {
            m_currentNode.isVisited = true;
        }
        
        m_currentNode.connectedNode = _newestNeighbor;
        
        Debug.Log(m_currentNode.index);
        
        int _newestX = 0;
        int _newestY = 0;
        switch (m_currentNode.nodeState)
        {
            case ENodeDirState.Up:
            {
                _newestX = (_nodeIndex + m_width) % m_width;
                _newestY = ((_nodeIndex + m_width) / m_width) % m_height;
            }
            break;
        
            case ENodeDirState.Down:
            {
                _newestX = (_nodeIndex - m_width) % m_width;
                _newestY = ((_nodeIndex - m_width) / m_width) % m_height;
            }
            break;
        
            case ENodeDirState.Left:
            {
                _newestX = (_nodeIndex - 1) % m_width;
                _newestY = ((_nodeIndex - 1) / m_width) % m_height;
            }
            break;
        
            case ENodeDirState.Right:
            {
                _newestX = (_nodeIndex + 1) % m_width;
                _newestY = ((_nodeIndex + 1) / m_width) % m_height;
            }
            break;
        }
        
        RecursiveGrowingTreeNewest(_newestX, _newestY);
    }

    private void GenerateWalls(Node prevNode, Node currentNode, int x, int y)
    {
        Quaternion _eulerAngle = Quaternion.identity;
        float _xOffset = x * GRID_X_OFFSET;
        float _zOffset = y * GRID_Z_OFFSET;

        if (prevNode == null)
        {
            switch (currentNode.nodeState)
            {
                case ENodeDirState.Right:
                    _eulerAngle = Quaternion.Euler(0f, 90f, 0f);
                    break;
                case ENodeDirState.Left:
                    _eulerAngle = Quaternion.Euler(0f, -90f, 0f);
                    break;
            }

            GameObject _straight = Instantiate(m_straightWallPrefab,
                    new Vector3(_xOffset, 0f, _zOffset),
                    _eulerAngle);

            _straight.transform.SetParent(m_parentObject.transform);
            return;
        }

        if (prevNode.nodeState == currentNode.nodeState)
        {
            if (currentNode.nodeState == ENodeDirState.Left 
                || currentNode.nodeState == ENodeDirState.Right)
            {
                GameObject _straight = Instantiate(m_straightWallPrefab, 
                    new Vector3(_xOffset, 0f, _zOffset), 
                    Quaternion.Euler(0f, 90f, 0f));

                _straight.transform.SetParent(m_parentObject.transform);
            }
            return;
        }

        switch (currentNode.nodeState)
        {
            case ENodeDirState.Right:
                {
                    if (prevNode.nodeState == ENodeDirState.Down)
                    {
                        _eulerAngle = Quaternion.Euler(0f, 180f, 0f);
        
                        GameObject _downToRight = Instantiate(m_leftWallPrefab,
                            new Vector3(_xOffset, 0f, _zOffset), _eulerAngle);
        
                        _downToRight.transform.SetParent(m_parentObject.transform);
                        break;
                    }
        
                    GameObject _upToRight = Instantiate(m_rightWallPrefab,
                            new Vector3(_xOffset, 0f, _zOffset), _eulerAngle);
        
                    _upToRight.transform.SetParent(m_parentObject.transform);
                }
                break;
        
            case ENodeDirState.Left:
                {
                    if (prevNode.nodeState == ENodeDirState.Down)
                    {
                        _eulerAngle = Quaternion.Euler(0f, 180f, 0f);
        
                        GameObject _downToLeft = Instantiate(m_rightWallPrefab,
                            new Vector3(_xOffset, 0f, _zOffset), _eulerAngle);
        
                        _downToLeft.transform.SetParent(m_parentObject.transform);
                        break;
                    }
        
                    GameObject _upToLeft = Instantiate(m_leftWallPrefab,
                            new Vector3(_xOffset, 0f, _zOffset), _eulerAngle);
        
                    _upToLeft.transform.SetParent(m_parentObject.transform);
                }
                break;
        
            case ENodeDirState.Up:
                {
                    if (prevNode.nodeState == ENodeDirState.Right)
                    {
                        _eulerAngle = Quaternion.Euler(0f, 90f, 0f);
        
                        GameObject _rightToUp = Instantiate(m_leftWallPrefab,
                            new Vector3(_xOffset, 0f, _zOffset), _eulerAngle);
        
                        _rightToUp.transform.SetParent(m_parentObject.transform);
                        break;
                    }
        
                    _eulerAngle = Quaternion.Euler(0f, -90f, 0f);
        
                    GameObject _leftToUp = Instantiate(m_rightWallPrefab,
                        new Vector3(_xOffset, 0f, _zOffset), _eulerAngle);
        
                    _leftToUp.transform.SetParent(m_parentObject.transform);
                }
                break;
        
            case ENodeDirState.Down:
                {
                    if (prevNode.nodeState == ENodeDirState.Right)
                    {
                        GameObject _rightToDown = Instantiate(m_leftWallPrefab,
                            new Vector3(_xOffset, 0f, _zOffset), _eulerAngle);
        
                        _rightToDown.transform.SetParent(m_parentObject.transform);
                        break;
                    }
        
                    GameObject _leftToUp = Instantiate(m_rightWallPrefab,
                        new Vector3(_xOffset, 0f, _zOffset), _eulerAngle);
        
                    _leftToUp.transform.SetParent(m_parentObject.transform);
                }
                break;
            case ENodeDirState.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
