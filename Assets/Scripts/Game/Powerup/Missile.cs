using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Missile : Powerup_Base
{
    [Header("Implementing Class Settings")]
    [SerializeField] float _fireRate;
    [SerializeField] private TMP_Text _powerupLabel;
    private float _canFire;
    protected static Missile Instance;
    protected override void AddToListener()
    {
        if (_owner.TryGetComponent(out IDealDamage entityShooting))
            entityShooting.OnEntityFireWeapon += OnEntityFiredWeapon;
    }

    protected override void RemoveFromListener()
    {
        if (_owner.TryGetComponent(out IDealDamage entityShooting))
            entityShooting.OnEntityFireWeapon -= OnEntityFiredWeapon;
    }

    public override void Activate(GameObject owner)
    {       
        if (Instance != null)
            Instance.StopPowerup();
                
        Instance = this;
        _powerupLabel.enabled = false;
        base.Activate(owner);
    }

    protected override void OnEntityFiredWeapon()
    {
        if (Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            base.OnEntityFiredWeapon();                                        
        }
    }



    public override void StopPowerup()
    {
        Instance = null;
        base.StopPowerup();
    }
}
