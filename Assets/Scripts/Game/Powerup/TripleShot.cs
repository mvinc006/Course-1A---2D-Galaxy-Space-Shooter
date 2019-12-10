using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShot : Powerup_Base
{
    [Header("Implementing Class Settings")]
    [SerializeField] float _fireRate;
    [SerializeField] AudioClip _laserSound;

    private float _canFire;
    protected static TripleShot Instance;    
    
    protected override void AddToListener()
    {
        if (_owner.TryGetComponent(out IDealDamage shootingEntity))
        {
            shootingEntity.OnEntityFireWeapon += OnEntityFiredWeapon;
            Debug.Log(this + " Successfully Registered to " + _owner.name);
        }            
    }

    protected override void RemoveFromListener()
    {
        if (_owner.TryGetComponent(out IDealDamage shootingEntity))
        {
            shootingEntity.OnEntityFireWeapon -= OnEntityFiredWeapon;
            Debug.Log(this + " Successfully Un-Registered to " + _owner.name);
        }            
    }

    public override void Activate(GameObject owner)
    {
        if (Instance != null)
            Instance.StopPowerup();
       
        Instance = this;
        base.Activate(owner);
    }

    protected override void OnEntityFiredWeapon()
    {
        if (Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;

            if(_laserSound)
                AudioSource.PlayClipAtPoint(_laserSound, transform.position);
            
            base.OnEntityFiredWeapon();           
        }
    }

    public override void StopPowerup()
   {
        Instance = null;
        base.StopPowerup();
    }
   
}
