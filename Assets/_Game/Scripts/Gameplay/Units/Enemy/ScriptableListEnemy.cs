using System.Linq;
using UnityEngine;
using Obvious.Soap;

[CreateAssetMenu(fileName = "scriptable_list_" + nameof(Enemy), menuName = "Soap/ScriptableLists/"+ nameof(Enemy))]
public class ScriptableListEnemy : ScriptableList<Enemy>
{
    public Enemy GetClosest(Vector3 position, float maxRange)
    {
        if (IsEmpty)
            return null;

        Enemy closest = null;
        float maxRangeSquared = maxRange * maxRange;
        float closestDistance = float.MaxValue;

        foreach (var enemy in _list)
        {
            float distance = (position - enemy.transform.position).sqrMagnitude;

            if (distance <= maxRangeSquared && distance < closestDistance)
            {
                closest = enemy;
                closestDistance = distance;
            }
        }

        return closest;
    }
}
