using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity_Base : MonoBehaviour, IDamageable, IDealDamage, IMove
{
    [Header("Base Class Settings")]
    [SerializeField, Space, Range(1.0f, 10.0f)] protected float _speed = 3f;
    [SerializeField] protected float _verticalBoundMin = -3.8f;
    [SerializeField] protected float _verticalBoundMax = 0f;
    [SerializeField] protected float _horizontalBoundMin = -10f;
    [SerializeField] protected float _horizontalBoundMax = 10f;

    protected Rigidbody2D _entityRigidBody;
    public string LaserMask { get; set; }   // Implemetning class sets this on Awake/Start so their own lasers don't harm them
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
