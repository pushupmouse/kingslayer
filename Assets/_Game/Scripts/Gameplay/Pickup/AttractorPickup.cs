using System.Collections.Generic;
using System.Linq;
using MEC;
using Obvious.Soap;
using UnityEngine;

public class AttractorPickup : Pickup
{
    [SerializeField] private float _radius = 20f;
    [SerializeField] private float _attractSpeed = 20f;
    [SerializeField] private Vector2Variable _playerTransform;
    [SerializeField] private ScriptableListTransform _scriptableListExperiencePickup;

    protected override void OnTriggerEnter2D(Collider2D  other)
    {
        Timing.RunCoroutine(_Cr_Attract());

        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private IEnumerator<float> _Cr_Attract()
    {
        var pickupsInRange = new List<Transform>(_scriptableListExperiencePickup.Where(
            p => Vector2.Distance(p.transform.position,transform.position) < _radius));

        var count = pickupsInRange.Count;

        foreach (var p in pickupsInRange)
        {
            var pickup = p.GetComponent<Pickup>();
            pickup.OnPickedUp += () => { count--; };
        }

        while (count > 0)
        {
            for (int i = pickupsInRange.Count - 1; i >= 0; i--)
            {
                var pickup = pickupsInRange[i];
                if (pickup == null)
                    continue;

                Vector2 dir = (_playerTransform.Value - (Vector2)pickup.position).normalized;
                pickup.position += (Vector3)(dir * (_attractSpeed * Time.deltaTime));

                yield return Timing.WaitForOneFrame;
            }
        }
        
        Destroy(gameObject);
    }
}
