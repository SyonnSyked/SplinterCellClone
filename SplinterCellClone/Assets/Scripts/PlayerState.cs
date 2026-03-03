using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using static EnemyAI;


public class PlayerState : MonoBehavior
{
    [SerializeField] public float currentVis;
    [SerializeField] public float currentAudib;
    [SerializeField] public float maxVis;
    [SerializeField] public float maxAudib;
}


