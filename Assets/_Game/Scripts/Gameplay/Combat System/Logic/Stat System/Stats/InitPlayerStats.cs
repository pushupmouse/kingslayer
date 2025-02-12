using Obvious.Soap;
using UnityEngine;
using Yade.Runtime;

//surely there is a better way hahah
public class InitPlayerStats : MonoBehaviour
{
    [SerializeField] private YadeSheetData _initPlayerStatsData;
    [SerializeField] private FloatVariable _baseAttack;
    [SerializeField] private FloatVariable _baseAttackSpeed;
    [SerializeField] private FloatVariable _baseCritRate;
    [SerializeField] private FloatVariable _baseCritMult;
    [SerializeField] private FloatVariable _baseAmp;
    [SerializeField] private FloatVariable _basePenetration;
    [SerializeField] private FloatVariable _baseMaxHealth;
    [SerializeField] private FloatVariable _baseDefense;
    [SerializeField] private FloatVariable _baseHealthRegen;
    [SerializeField] private FloatVariable _baseSpeed;
    [SerializeField] private FloatVariable _baseMaxRange;
    
    private class PlayerStat
    {
        [DataField(0)] public string Attack;
        [DataField(1)] public string AttackSpeed;
        [DataField(2)] public string CritRate;
        [DataField(3)] public string CritMult;
        [DataField(4)] public string Amp;
        [DataField(5)] public string Penetration;
        [DataField(6)] public string MaxHealth;
        [DataField(7)] public string Defense;
        [DataField(8)] public string HealthRegen;
        [DataField(9)] public string Speed;
        [DataField(10)] public string MaxRange;
    }

    private void Awake()
    {
        InitStats();
    }

    private void InitStats()
    {
        var list = _initPlayerStatsData.AsList<PlayerStat>();
        
        _baseAttack.Value = float.Parse(list[1].Attack);
        _baseAttackSpeed.Value = float.Parse(list[1].AttackSpeed);
        _baseCritRate.Value = float.Parse(list[1].CritRate);
        _baseCritMult.Value = float.Parse(list[1].CritMult);
        _baseAmp.Value = float.Parse(list[1].Amp);
        _basePenetration.Value = float.Parse(list[1].Penetration);
        _baseMaxHealth.Value = float.Parse(list[1].MaxHealth);
        _baseDefense.Value = float.Parse(list[1].Defense);
        _baseHealthRegen.Value = float.Parse(list[1].HealthRegen);
        _baseSpeed.Value = float.Parse(list[1].Speed);
        _baseMaxRange.Value = float.Parse(list[1].MaxRange);
    }
}
