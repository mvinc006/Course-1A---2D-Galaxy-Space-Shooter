using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IDamageable, IShield
{
    public int health;
    public Color[] shieldColorRange { get ; set ; }
    public int shieldColorRangeIndex { get; set; }
    public bool shieldStatus { get ; set; }
    public string laserMask { get ; set ; }


    public void OnDeath()
    {
        this.shieldStatus = false;
        this.gameObject.SetActive(false);
    }

    public void OnTakeDamage()
    {       
        OnShieldDamage();
    }

    public void OnInit(int health, Color[] colorRange, string owner)
    {
        this.health = health;
        this.shieldColorRange = colorRange;
        this.shieldStatus = true;
        this.laserMask = owner;
        this.shieldColorRangeIndex = colorRange.Length -1;
        this.gameObject.SetActive(true);
    }

    public void OnShieldDamage()
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

   /* public void OnTriggerEnter2D(Collider2D collision)
    {
        // only player has a shield for the time being.
        if (collision.CompareTag(ownerTag) || collision.CompareTag("Powerup"))
            return;
        else
            OnTakeDamage();
    }*/
}
