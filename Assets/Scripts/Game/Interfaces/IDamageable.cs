using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    string laserMask { get; set; }
    void OnTakeDamage();
    void OnDeath();
}
