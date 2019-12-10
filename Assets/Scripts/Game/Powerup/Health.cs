using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Health : Powerup_Base
{
    [Header("Implementing Class Settings")]
    [SerializeField] private TMP_Text _powerupLabel;
    protected override void AddToListener() { }
    protected override void RemoveFromListener() { }

    [HideInInspector]
    

    public override void Activate(GameObject owner)
    {
        base.Activate(owner);
        _powerupLabel.enabled = false;
        AddHealth();
    }

    private void AddHealth()
    {
        // Enemy don't implement health 
        if (_owner.TryGetComponent(out Player entity))
            entity.OnHealthPickup();
    }

    

}
