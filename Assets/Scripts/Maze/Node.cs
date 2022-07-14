using System.Collections;
using System.Collections.Generic;

public enum ENodeDirState
{
    Up,
    Down,
    Left,
    Right,
    None
}

[System.Serializable]
public class Node
{
    public static int s_UNodeID = 0;
    
    public int index;
    public ENodeDirState nodeState = ENodeDirState.None;

    public bool isVisited = false;
    
    public Node upNode;
    public Node downNode;
    public Node leftNode;
    public Node rightNode;

    public Node connectedNode;

    public Node()
    {
        index = s_UNodeID++;
    }

    public void Reset()
    {
        s_UNodeID = 0;
        
        nodeState = ENodeDirState.None;
        index = 0;
        
        isVisited = false;
        
        upNode = null;
        downNode = null;
        leftNode = null;
        rightNode = null;
    }
}
