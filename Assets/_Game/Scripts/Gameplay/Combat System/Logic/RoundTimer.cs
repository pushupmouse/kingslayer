using System.Collections.Generic;
using MEC;
using Obvious.Soap;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yade.Runtime;

public class RoundTimer : MonoBehaviour
{
    [SerializeField] private YadeSheetData _roundData;
    [SerializeField] private IntVariable _currentRound;
    [SerializeField] private SceneAsset _boostCenterScene;
    [SerializeField] private float _sceneTransitionDelay = 5f;
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
        
        _currentRound.OnValueChanged += UpdateDuration;
        
        StartTimer();
    }
    
    private void OnDisable()
    {
        _currentRound.OnValueChanged -= UpdateDuration;
    }
    
    private void Update()
    {
        if (!_isRunning) return;
        _elapsedTime += Time.deltaTime;

        if (!(_elapsedTime >= _duration)) return;
        StopTimer();
        Timing.RunCoroutine(_EnterBoostCenterScene(_sceneTransitionDelay));
    }

    private IEnumerator<float> _EnterBoostCenterScene(float delay)
    {
        yield return Timing.WaitForSeconds(delay);
        GameManager.Instance.ChangeScene(_boostCenterScene.name);
    }

    private void EnterNextRound()
    {
        _currentRound.Add(1);
    }

    private void UpdateDuration(int obj)
    {
        var list = _roundData.AsList<RoundPhase>();

        if (_currentRound.Value < list.Count - 1)
        {
            _duration = float.Parse(list[_currentRound].Duration);
        }
        else
        {
            _duration = float.Parse(list[^1].Duration);
        }
        
        _elapsedTime = 0f;
    }
    
    private void StartTimer()
    {
        _elapsedTime = 0;
        
        _isRunning = true;
    }

    private void StopTimer()
    {
        _isRunning = false;
        Debug.Log("Timer stopped");
    }
}
