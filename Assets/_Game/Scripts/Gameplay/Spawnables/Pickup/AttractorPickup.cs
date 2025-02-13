using System.Collections.Generic;
using MEC;
using Obvious.Soap;
using UnityEngine;

public class AttractorPickup : Pickup
{
    [SerializeField] private ScriptableListTransform _scriptableListExperiencePickup;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        AttractPickups();

        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void AttractPickups()
    {
        var pickups = new List<ExpPickup>();

        // Collect all ExpPickup objects from the list
        foreach (var p in _scriptableListExperiencePickup)
        {
            var expPickup = p.GetComponent<ExpPickup>();
            if (!expPickup.isActiveAndEnabled) continue;
            pickups.Add(expPickup);
        }

        // Call method in ExpPickup to start moving toward the player
        foreach (var pickup in pickups)
        {
            pickup.EnableAttracted();
        }

        ObjectPool.Instance.ReturnObject(this);
    }
}