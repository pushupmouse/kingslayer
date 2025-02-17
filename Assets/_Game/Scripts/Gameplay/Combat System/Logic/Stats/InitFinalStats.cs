using UnityEngine;

public class InitFinalStats : Singleton<InitFinalStats>
{
    [SerializeField] private FinalStatData[] _stats;

    protected override void Awake()
    {
        base.Awake();
        
        foreach (FinalStatData stat in _stats)
        {
            stat.Init();
        }
    }
}
