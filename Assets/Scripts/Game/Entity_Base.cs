using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity_Base : MonoBehaviour, IDamageable, IDealDamage, IMove
{
    public string laserMask { get; set; }
    public event Action OnEntityFireWeapon;

    public abstract void OnDealDamage();
    public abstract void OnDeath();
    public abstract void OnSpeedBoost(float speed);
    public abstract void OnTakeDamage();    

    public bool ShouldFireSpecialWeapon()
    {
        if (OnEntityFireWeapon != null)
        {
            OnEntityFireWeapon();
            return true;
        }
            
        return false;
    }
       
}
