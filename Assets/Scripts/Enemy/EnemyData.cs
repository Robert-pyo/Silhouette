using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Data/EnemyData")]
public class EnemyData : ScriptableObject
{
    public GameObject enemyPrefab;
    
    [Header("Enemy Status")]
    public ushort maxHp;

    public float recognizeRange;
    public float attackRange;
    public ushort attackDamage;
}
