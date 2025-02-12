using System.Collections;
using System.Collections.Generic;
using Obvious.Soap;
using UnityEngine;

public class PlayerPositionSaver : MonoBehaviour
{
    [SerializeField] private Vector2Variable _playerPosition;

    private void Start()
    {
        transform.position = _playerPosition.Value;
    }

    private void Update()
    {
        _playerPosition.Value = transform.position;
    }
}
