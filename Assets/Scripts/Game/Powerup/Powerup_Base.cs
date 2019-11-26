using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup_Base : MonoBehaviour
{
    [SerializeField] protected float powerupMoveSpeed = 1f;
    [SerializeField] protected GameObject _prefab;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] protected Collider2D _collider;

    public delegate void WeaponChangeEventHandler(object sender, bool isEnabled);
    public static event WeaponChangeEventHandler PrimaryWeaponChange;
    protected delegate void PowerupChangeEventHandler(object sender);
    protected static event PowerupChangeEventHandler PowerupChanged;
    protected virtual void InvokeWeaponChanged(object sender, bool isEnabled)
    {
        if(PrimaryWeaponChange != null)
        {            
            PrimaryWeaponChange.Invoke(sender, isEnabled);
        }                    
    }

    protected virtual void OnDisableOtherWeapons(object sender) 
    {
        if(PowerupChanged != null)
        {            
            Debug.Log(sender + " is overriding other existing powerups in this powerup group- (" + this + ") will be destroyed");            
        }            
    }
   
    protected virtual void OnPickup(object sender, bool otherPowerupsOverride)
    {
        _collider.enabled = false;
        _renderer.enabled = false;
        if(PowerupChanged != null && otherPowerupsOverride)
        {
            PowerupChanged(sender); // call the events on any currently subscribed powerups that aren't us
            PowerupChanged -= OnDisableOtherWeapons; // unsubscribe
        }
        else if(PowerupChanged == null && otherPowerupsOverride)
        {
            PowerupChanged += OnDisableOtherWeapons; // invoked when powerups activate, disabling any others that might exist (weapons only should be single type)
        }                
    }
    protected virtual void Update()
    {
        if (transform.position.y >= -5.5f)
            transform.Translate(Vector3.down * powerupMoveSpeed * Time.deltaTime);
    }
}
