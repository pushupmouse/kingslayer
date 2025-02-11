using Obvious.Soap;
using UnityEngine;
using Yade.Runtime;

public class RoundPhaseTimer : MonoBehaviour
{
    [SerializeField] private YadeSheetData _roundPhaseData;
    [SerializeField] private IntVariable _currentRoundPhase;
    private float _duration;
    private float _elapsedTime;
    private bool _isRunning;

    private class RoundPhase
    {
        [DataField(0)] public string Id;
        [DataField(1)] public string Duration;
    }
    
    private void Awake()
    {
        UpdateDuration(0);
        
        ResetPhase();
        
        _currentRoundPhase.OnValueChanged += UpdateDuration;
        
        StartTimer();
    }
    
    private void OnDisable()
    {
        _currentRoundPhase.OnValueChanged -= UpdateDuration;
    }
    
    private void Update()
    {
        if (_isRunning)
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _duration)
            {
                EnterNextPhase();
            }
        }
    }

    private void EnterNextPhase()
    {
        _currentRoundPhase.Add(1);
    }

    private void UpdateDuration(int obj)
    {
        var list = _roundPhaseData.AsList<RoundPhase>();

        if (_currentRoundPhase.Value < list.Count - 1)
        {
            _duration = float.Parse(list[_currentRoundPhase].Duration);
        }
        else
        {
            _duration = float.Parse(list[^1].Duration);
        }
        
        _elapsedTime = 0f;
    }
    
    public void ResetPhase()
    {
        _currentRoundPhase.Value = _currentRoundPhase.Min;
    }
    
    public void StartTimer()
    {
        _elapsedTime = 0;
        
        _isRunning = true;
    }

    public void StopTimer()
    {
        _isRunning = false;
    }
}
