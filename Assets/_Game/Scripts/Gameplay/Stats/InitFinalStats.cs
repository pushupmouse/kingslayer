using System;
using System.Collections;
using System.Collections.Generic;
using Obvious.Soap;
using UnityEngine;

public class InitFinalStats : MonoBehaviour
{
    [SerializeField] private FinalStatData[] _stats;

    private void Awake()
    {
        foreach (FinalStatData stat in _stats)
        {
            stat.Init();
        }
    }
}
