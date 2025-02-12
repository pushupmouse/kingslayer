using System;
using System.Collections;
using System.Collections.Generic;
using Obvious.Soap;
using UnityEngine;

public class InitStatIncreases : MonoBehaviour
{   
    [SerializeField] private StatIncreaseData[] _increases;

    private void Awake()
    {
        foreach (StatIncreaseData increase in _increases)
        {
            increase.Init();
        }
    }
}
