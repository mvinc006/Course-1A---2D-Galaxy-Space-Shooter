using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ammo : Powerup_Base
{
    [Header("Implementing Class Settings")]
    [SerializeField] int _ammoCount = 15;
    [SerializeField] private TMP_Text powerupLabel;
    protected override void AddToListener(){ }
    protected override void RemoveFromListener() { }

    public override void Activate(GameObject owner)
    {        
        base.Activate(owner);
        powerupLabel.enabled = false;
        AddAmmo();
    }

    private void AddAmmo()
    {
        if (owner.TryGetComponent(out Player entity))
            entity.OnAmmoActive(_ammoCount);
    }

}
