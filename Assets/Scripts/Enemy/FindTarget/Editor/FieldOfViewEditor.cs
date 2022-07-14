using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Soldier))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        Soldier enemy = (Soldier)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(enemy.transform.position, Vector3.up, Vector3.forward, 360, enemy.viewRadius);

        Vector3 viewAngleA = enemy.DirFromAngle(-enemy.viewAngle * 0.5f, false);
        Vector3 viewAngleB = enemy.DirFromAngle(enemy.viewAngle * 0.5f, false);

        Handles.DrawLine(enemy.transform.position, enemy.transform.position + viewAngleA * enemy.viewRadius);
        Handles.DrawLine(enemy.transform.position, enemy.transform.position + viewAngleB * enemy.viewRadius);

        Handles.color = Color.green;
        if (enemy.target)
        {
            Handles.DrawLine(enemy.transform.position, enemy.target.position);
        }
    }
}
