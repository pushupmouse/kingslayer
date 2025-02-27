using System;
using System.Collections;
using Obvious.Soap;
using UnityEngine;
using Yade.Runtime;
using Random = UnityEngine.Random;

public class EnemySpawner : Spawner
{
    [SerializeField] private YadeSheetData _enemySpawnData;
    [SerializeField] private IntVariable _currentRoundPhase;
    [SerializeField] private Enemy _normalPrefab;
    [SerializeField] private Enemy _tankPrefab;
    [SerializeField] private Enemy _roguePrefab;
    [SerializeField] private Enemy _slayerPrefab;
    [SerializeField] private Enemy _butcherPrefab;
    [SerializeField] private Enemy _jokerPrefab;
    [SerializeField] private Enemy _sniperPrefab;
    [SerializeField] private Enemy _rangerPrefab;

    private Enemy[] _enemyPrefabs;
    
    private class EnemySpawn
    {
        [DataField(0)] public string Id;
        [DataField(1)] public string Delay;
        [DataField(2)] public string Interval;
        [DataField(3)] public string Amount;
    }
    
    protected override IEnumerator Start()
    {
        var list = _enemySpawnData.AsList<EnemySpawn>();
        
        _initialDelay = float.Parse(list[_currentRoundPhase].Delay);
        _spawnInterval = float.Parse(list[_currentRoundPhase].Interval);
        _amount = int.Parse(list[_currentRoundPhase].Amount);
        
        _currentRoundPhase.OnValueChanged += OnRoundPhaseChange;
        
        yield return base.Start();

        _enemyPrefabs = new Enemy[]
        {
            _normalPrefab,
            _tankPrefab,
            _roguePrefab,
            _slayerPrefab,
            _butcherPrefab,
            _jokerPrefab,
            _sniperPrefab,
            _rangerPrefab,
        };
    }

    private void OnDisable()
    {
        _currentRoundPhase.OnValueChanged -= OnRoundPhaseChange;
    }

    private void OnRoundPhaseChange(int obj)
    {
        var list = _enemySpawnData.AsList<EnemySpawn>();
        
        if (_currentRoundPhase.Value < list.Count - 1)
        {
            _spawnInterval = float.Parse(list[_currentRoundPhase].Interval);
            _amount = int.Parse(list[_currentRoundPhase].Amount);
        }
        else
        {
            _spawnInterval = float.Parse(list[^1].Interval);
            _amount = int.Parse(list[^1].Amount);
        }
    }


    protected override void Spawn()
    {
        base.Spawn();
        
        int randomIndex = Random.Range(0, _enemyPrefabs.Length);

        if (_enemyPrefabs[randomIndex] == null)
            return;
        
        var spawnPoint = new Vector3(spawnPosition.x, spawnPosition.y, 0);

        Enemy enemy = ObjectPool.Instance.GetObject(_enemyPrefabs[randomIndex]);

        enemy.Init(spawnPoint);
    }
}

public enum EnemyType
{
    Normal = 1,
    Tank = 2,
    Rogue = 3,
    Slayer = 4,
    Butcher = 5,
    Joker = 6,
    Sniper = 7,
    Ranger = 8,
}