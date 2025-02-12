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
        Timing.RunCoroutine(_Cr_Attract().CancelWith(gameObject));

        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private IEnumerator<float> _Cr_Attract()
    {
        var pickupsInRange = new List<Rigidbody2D>();

        // Collect all pickups within range that have a Rigidbody2D
        foreach (var p in _scriptableListExperiencePickup)
        {
            if (Vector2.Distance(p.transform.position, transform.position) < _radius)
            {
                var rb = p.GetComponent<Rigidbody2D>();
                if (rb != null)
                    pickupsInRange.Add(rb);
            }
        }

        int count = pickupsInRange.Count;

        foreach (var rb in pickupsInRange)
        {
            var pickup = rb.GetComponent<Pickup>();
            pickup.OnPickedUp += () => { count--; };
        }

        while (count > 0)
        {
            for (int i = pickupsInRange.Count - 1; i >= 0; i--)
            {
                var rb = pickupsInRange[i];

                if (rb == null)
                {
                    pickupsInRange.RemoveAt(i);
                    continue;
                }

                Vector2 dir = (_playerTransform.Value - rb.position).normalized;

                // Adjust attraction speed dynamically based on distance
                float distance = Vector2.Distance(rb.position, _playerTransform.Value);
                float dynamicSpeed = Mathf.Lerp(_attractSpeed * 0.5f, _attractSpeed * 2f, 1 - (distance / _radius));

                // Use Rigidbody2D for smooth attraction
                rb.MovePosition(rb.position + dir * dynamicSpeed * Time.deltaTime);
            }

            yield return Timing.WaitForOneFrame;
        }

        Destroy(gameObject);
    }
}
