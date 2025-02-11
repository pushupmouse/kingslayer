using UnityEngine;
using Obvious.Soap;

[CreateAssetMenu(fileName = "scriptable_list_" + nameof(Transform), menuName = "Soap/ScriptableLists/"+ nameof(Transform))]
public class ScriptableListTransform : ScriptableList<Transform>
{
    
}
