using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup_Base : MonoBehaviour
{
    [SerializeField] protected float powerupMoveSpeed = 1f;
    [SerializeField] protected GameObject _prefab;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] protected Collider2D _collider;

    protected delegate void PowerupChangeEventHandler(object sender, bool removeFromListener = false);
    protected static event PowerupChangeEventHandler PowerupChanged;

    public delegate void WeaponChangeEventHandler(object sender, bool isEnabled);
    public static event WeaponChangeEventHandler PrimaryWeaponChange;

    protected abstract void OnTriggerEnter2D(Collider2D collision);

    protected virtual void OnWeaponChange(object sender, bool isEnabled)
    {
        if(PrimaryWeaponChange != null)
        {            
            PrimaryWeaponChange.Invoke(sender, isEnabled);
        }                    
    }

    protected virtual void OnPowerupChangedEvent(object sender, bool removeFromListener = false) 
    {
        if(PowerupChanged != null)
        {
            if (removeFromListener)
            {
                PowerupChanged -= OnPowerupChangedEvent;
                Debug.Log(sender + " is overriding other existing powerups in this powerup group- (" + this + ") will be destroyed");
            }
            else
            {
                Debug.Log(sender + " has triggered the PowerupChanged event for this group - (sent from: " + this + ")");
            }            
        }         
    }
   
    protected virtual void OnPowerupPickedUp(object sender, bool bIsSingleInstancePowerup)
    {
        _collider.enabled = false;
        _renderer.enabled = false;
        if(bIsSingleInstancePowerup)
        {
            PowerupChanged?.Invoke(sender); // call the events on any currently subscribed powerups that aren't us            
            PowerupChanged += OnPowerupChangedEvent; // subscribe after all other powerups marked as otherPowerupsOverride have responded to the event.
        }                
    }
    protected virtual void Update()
    {
        if (transform.position.y >= -5.5f)
            transform.Translate(Vector3.down * powerupMoveSpeed * Time.deltaTime);
    }
}
