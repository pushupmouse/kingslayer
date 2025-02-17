using Obvious.Soap;
using UnityEngine;
using Yade.Runtime;

public class InitExperienceStats : Singleton<InitExperienceStats>
{
    [SerializeField] private YadeSheetData _experienceStatsData;
    [SerializeField] private FloatVariable _maxExperience;
    [SerializeField] private FloatVariable _maxExperienceIncrement;
    [SerializeField] private FloatVariable _maxExperienceMultiplier;
    [SerializeField] private FloatVariable _expPickupValue;
    
    private class Experience
    {
        [DataField(0)] public string MaxExperience;
        [DataField(1)] public string MaxExperienceIncrement;
        [DataField(2)] public string MaxExperienceMultiplier;
        [DataField(3)] public string ExpPickupValue;
    }

    protected override void Awake()
    {
        base.Awake();
        
        var list = _experienceStatsData.AsList<Experience>();

        _maxExperience.Value = float.Parse(list[1].MaxExperience);
        _maxExperienceIncrement.Value = float.Parse(list[1].MaxExperienceIncrement);
        _maxExperienceMultiplier.Value = float.Parse(list[1].MaxExperienceMultiplier);
        _expPickupValue.Value = float.Parse(list[1].ExpPickupValue);
    }
}   
