using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShield
{
    Color[] shieldColorRange { get; set; }
    int shieldColorRangeIndex { get; set; }
    bool shieldStatus { get; set; }
    string laserMask { get; set; }
    void OnInit(int health, Color[] shieldColorRange, string ownerTag);
    void OnShieldDamage();
    
}
