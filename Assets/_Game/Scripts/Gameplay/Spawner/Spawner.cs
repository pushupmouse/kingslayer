using System.Collections;
using System.Collections.Generic;
using MEC;
using Obvious.Soap;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    [SerializeField] private Vector2Variable _playerPosition;
    [SerializeField] private Vector2 _spawnRange;
    protected float _initialDelay;
    protected float _spawnInterval;
    protected int _amount;

    private float _currentAngle;
    private float _timer;
    private bool _isActive;
    protected Vector2 spawnPosition;
    
    protected virtual IEnumerator Start()
    {
        yield return new WaitForSeconds(_initialDelay);
        _timer = 0f;
        _isActive = true;
    }
    
    private void Update()
    {
        if (!_isActive)
            return;

        _timer += Time.deltaTime;
        if (!(_timer >= _spawnInterval)) return;

        for (int i = 0; i < _amount; i++)
            Spawn();

        _timer = 0;
    }
    
    protected virtual void Spawn()
    {
        _currentAngle = (_currentAngle + 180f + Random.Range(-45, 43)) % 360f;
        float angleInRad = _currentAngle * Mathf.Deg2Rad;
        float range = Random.Range(_spawnRange.x, _spawnRange.y);

        Vector2 relativePosition = new Vector2(
            Mathf.Cos(angleInRad) * range,
            Mathf.Sin(angleInRad) * range
        );

        spawnPosition = _playerPosition.Value + relativePosition;
    }
}
