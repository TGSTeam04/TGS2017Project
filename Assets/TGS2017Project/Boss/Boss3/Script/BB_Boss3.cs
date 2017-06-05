using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//ボス３behaviorないで使用する変数引渡しクラス
public class BB_Boss3 : BBoard
{
    [HideInInspector]
    public NavMeshAgent nAgent;
    public GameObject target;
    private void Start()
    {
        nAgent = GetComponent<NavMeshAgent>();
    }
}
