using System;
using System.Collections;
using System.Collections.Generic;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] private IntVariable _currentLevel;
    [SerializeField] private IntVariable _excessLevel;
    [SerializeField] private FloatVariable _currentExperience;
    [SerializeField] private FloatVariable _maxExperience;
    [SerializeField] private FloatVariable _maxExperienceIncrement;
    [SerializeField] private ScriptableEventNoParam _onLevelUp;

    private void Awake()
    {
        _currentLevel.OnValueChanged += UpdateMaxExperience;
    }

    private void OnDisable()
    {
        _currentLevel.OnValueChanged -= UpdateMaxExperience;
    }
    
    [ContextMenu("Level Up")]
    public void LevelUp()
    {
        _onLevelUp.Raise();
        _currentLevel.Add(1);
        _excessLevel.Add(1);
    }
    
    private void UpdateMaxExperience(int obj)
    {
        _currentExperience.ResetValue();
        
        _maxExperience.Value += _maxExperienceIncrement.Value;
    }
}
