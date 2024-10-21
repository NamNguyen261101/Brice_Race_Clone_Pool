using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Level : MonoBehaviour
{
    public NavMeshData navMeshData;
    public Transform startPoint;
    public Transform finishPoint;
    public int botAmount;
    public Stage[] stages;

    public void OnInit()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            stages[i].OnInit();
        }
    }
}
