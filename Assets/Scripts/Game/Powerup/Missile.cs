using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Missile : Powerup_Base
{
    [Header("Implementing Class Settings")]
    [SerializeField] float fireRate;
    [SerializeField] private TMP_Text powerupLabel;
    private float canFire;
    protected static Missile Instance;
    protected override void AddToListener()
    {
        if (owner.TryGetComponent(out IDealDamage entityShooting))
            entityShooting.OnEntityFireWeapon += OnEntityFiredWeapon;
    }

    protected override void RemoveFromListener()
    {
        if (owner.TryGetComponent(out IDealDamage entityShooting))
            entityShooting.OnEntityFireWeapon -= OnEntityFiredWeapon;
    }

    public override void Activate(GameObject owner)
    {       
        if (Instance != null)
            Instance.StopPowerup();
                
        Instance = this;
        powerupLabel.enabled = false;
        base.Activate(owner);
    }

    protected override void OnEntityFiredWeapon()
    {
        if (Time.time > canFire)
        {
            canFire = Time.time + fireRate;
            base.OnEntityFiredWeapon();                                        
        }
    }



    public override void StopPowerup()
    {
        Instance = null;
        base.StopPowerup();
    }
}
