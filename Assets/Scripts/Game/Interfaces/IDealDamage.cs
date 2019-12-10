using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IDealDamage
{   
    event Action OnEntityFireWeapon;
    void OnDealDamage();
    
}
