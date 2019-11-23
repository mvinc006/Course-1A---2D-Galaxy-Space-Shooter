using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Shield : Shield_Base
{
    public override void OnInit(int health, Color[] shieldColorRange, string ownerTag)
    {
        base.OnInit(health, shieldColorRange, ownerTag);
    }
}
