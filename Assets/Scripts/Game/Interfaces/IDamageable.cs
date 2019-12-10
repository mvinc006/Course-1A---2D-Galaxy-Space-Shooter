using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    string LaserMask { get; set; }
    void OnTakeDamage();
    void OnDeath();
}
