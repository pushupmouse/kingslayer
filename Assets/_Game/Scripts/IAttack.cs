using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack
{
    float GetDamage(bool isCriticalHit);
    void SpawnAttack(Vector2 directionNormalized);
}
