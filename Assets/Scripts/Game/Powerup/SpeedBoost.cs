using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Powerup_Base
{
    [Header("Implementing Class Settings")]
    [SerializeField, Range(2f, 10f)] float speedBoost = 2f;
    protected static SpeedBoost Instance;
    
    protected override void AddToListener()
    {
        // not implemented
    }

    protected override void RemoveFromListener()
    {
        // not implemented
    }

    public override void Activate(GameObject owner)
    {
        if (Instance != null)
            Instance.StopPowerup();
        
        Instance = this;            
        base.Activate(owner);
        ActivateSpeedBoost();        
    }

    private void ActivateSpeedBoost()
    {
        if(owner.TryGetComponent(out IMove entity))
        {
            entity.OnSpeedBoost(speedBoost);
        }
    }

    public override void StopPowerup()
    {
        if(owner.TryGetComponent(out IMove entity))
        {
            entity.OnSpeedBoost(1f);
        }

        Instance = null;
        base.StopPowerup();
    }



}
