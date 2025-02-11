using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Serialization;
using Yade.Runtime;

public class PickupSpawner : Spawner
{
    [SerializeField] private YadeSheetData _pickupSpawnData;
    [SerializeField] private Pickup _pickupPrefab;
    [SerializeField] private PickupType _pickupType;

    private class PickupSpawn
    {
        [DataField(0)] public string PickupType;
        [DataField(1)] public string Delay;
        [DataField(2)] public string Interval;
        [DataField(3)] public string Amount;
    }
    
    protected override IEnumerator Start()
    {
        var list = _pickupSpawnData.AsList<PickupSpawn>();

        int index = list.FindIndex(item => item.PickupType == _pickupType.ToString());
        
        _initialDelay = float.Parse(list[index].Delay);
        _spawnInterval = float.Parse(list[index].Interval);
        _amount = int.Parse(list[index].Amount);
        
        yield return base.Start();
    }
    
    protected override void Spawn()
    {
        base.Spawn();
        
        Pickup pickup = Instantiate(_pickupPrefab, spawnPosition, Quaternion.identity);
    }
}

public enum PickupType
{
    Health,
    Chest,
    Attractor,
}
