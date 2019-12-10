using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shield_Base : MonoBehaviour, IDamageable
{
    public Color[] shieldColorRange { get; set; }
    public int shieldColorRangeIndex { get; set; }
    public bool shieldStatus { get; set; }
    public string LaserMask { get ; set ; }
    public int health;

    public virtual void OnInit(int health, Color[] shieldColorRange, string ownerTag)
    {
        this.health = health;
        this.shieldColorRange = shieldColorRange;
        shieldStatus = true;
        LaserMask = ownerTag;
        shieldColorRangeIndex = shieldColorRange.Length - 1;
        gameObject.SetActive(true); 
    }

    public virtual void OnDeath()
    {
        shieldStatus = false;
        gameObject.SetActive(false); 
    }

    public void OnTakeDamage()
    {
        health--;

        if (health <= 0)
        {
            OnDeath();
            return;
        }

        GetComponent<SpriteRenderer>().color = shieldColorRange[shieldColorRangeIndex];
        shieldColorRangeIndex--;
    }

}
