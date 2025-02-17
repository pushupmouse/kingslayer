using UnityEngine;

public class InitStatIncreases : Singleton<InitStatIncreases>
{   
    [SerializeField] private StatIncreaseData[] _increases;

    protected override void Awake()
    {
        base.Awake();
        
        foreach (StatIncreaseData increase in _increases)
        {
            increase.Init();
        }
    }
}
