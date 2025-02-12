using System;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "FinalStatData/Stat")]
public class FinalStatData : ScriptableObject
{
    [SerializeField] private FloatVariable _base;
    [FormerlySerializedAs("_percent")] [SerializeField] private FloatVariable _mult;
    [SerializeField] private FloatVariable _final;

    [ContextMenu("Initialize")]
    public void Init()
    {
        if (_base == null || _mult == null || _final == null) 
            return;
        
        _final.Value = _base.Value + _base.Value * _mult.Value;
        
        _base.OnValueChanged += OnValueChanged;
        _mult.OnValueChanged += OnValueChanged;
    }
    
    private void OnValueChanged(float obj)
    {
        _final.Value = _base.Value + _base.Value * _mult.Value;
    }
}
