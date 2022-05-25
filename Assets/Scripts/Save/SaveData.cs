using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [Header("---- Save things ----")]
    public List<Vector3> positions = new List<Vector3>();
    public List<Quaternion> rotations = new List<Quaternion>();
    public List<Vector3> scales = new List<Vector3>();

    public List<GameObject> objs = new List<GameObject>();
}
