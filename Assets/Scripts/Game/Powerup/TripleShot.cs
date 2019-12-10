using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShot : Powerup_Base
{
    [Header("Implementing Class Settings")]
    [SerializeField] float fireRate;
    [SerializeField] AudioClip laserSound;

    private float canFire;
    protected static TripleShot Instance;    
    
    protected override void AddToListener()
    {
        if (owner.TryGetComponent(out IDealDamage shootingEntity))
        {
            shootingEntity.OnEntityFireWeapon += OnEntityFiredWeapon;
            Debug.Log(this + " Successfully Registered to " + owner.name);
        }            
    }

    protected override void RemoveFromListener()
    {
        if (owner.TryGetComponent(out IDealDamage shootingEntity))
        {
            shootingEntity.OnEntityFireWeapon -= OnEntityFiredWeapon;
            Debug.Log(this + " Successfully Un-Registered to " + owner.name);
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
        if (Time.time > canFire)
        {
            canFire = Time.time + fireRate;

            if(laserSound)
                AudioSource.PlayClipAtPoint(laserSound, transform.position);
            
            base.OnEntityFiredWeapon();           
        }
    }

    public override void StopPowerup()
   {
        Instance = null;
        base.StopPowerup();
    }
   
}
