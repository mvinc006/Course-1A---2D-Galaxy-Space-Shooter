using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Powerup_Base : MonoBehaviour
{
    [Header("Base Class Settings")]
    [SerializeField] protected float powerupDuration;
    [SerializeField] protected GameObject powerupPrefab;
    [SerializeField] protected AudioClip powerupClip;

    protected GameObject owner;
    private bool isActive;

    protected abstract void AddToListener();    // Not all powerups will register to a listener, implementing class to decide how this is handled. 
    protected abstract void RemoveFromListener();   
    public virtual void Activate(GameObject owner)  // Called by the OnTriggerEnter2D event
    {
        this.owner = owner;        
        isActive = true;
        powerupDuration += Time.time;
        if(powerupClip)
            AudioSource.PlayClipAtPoint(powerupClip, transform.position);

        AddToListener();
        Debug.Log(this + " Powerup Activated for " + powerupDuration + " second(s)");
    }

    protected virtual void OwnerFired()
    {
        if (powerupPrefab != null)
            Instantiate(powerupPrefab, owner.transform.position, Quaternion.identity);
    }

    protected virtual void Update()
    {
        if (isActive)
        {            
            if (Time.time > powerupDuration)
                StopPowerup();
        }
        else
        {
            transform.Translate(Vector3.down * 1.5f * Time.deltaTime);
            if (transform.position.y <= -5.5f)
                Destroy(this.gameObject);
        }
    }

    public virtual void StopPowerup()
               {
        Debug.Log(this + "Powerup Deactivated");
        RemoveFromListener();
        Destroy(this.gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {            
            if(TryGetComponent(out SpriteRenderer spriteRenderer))
                spriteRenderer.enabled = false;
            if(TryGetComponent(out Collider2D collider))
                collider.enabled = false;

            SpriteRenderer[] spriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].enabled = false;     // Health Powerup contains more than one sprite to disable
            }

            Activate(player.gameObject);
        }            
    }
}
