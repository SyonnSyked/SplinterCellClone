using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static EnemyAI;


public class PlayerState : MonoBehaviour
{
    [SerializeField] public float currentVis;
    [SerializeField] public float currentAudib;
    [SerializeField] public float maxVis;
    [SerializeField] public float maxAudib;

    public bool isFullyVisible;
    public bool isFullyAudible;
    public bool isDetected;

    public bool isClimbing;
}



