using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Powerup_Base : MonoBehaviour
{
    [Header("Base Class Settings")]
    [SerializeField] protected float _powerupDuration;
    [SerializeField] protected string _collectingEntityTag;
    [SerializeField] protected GameObject _powerupPrefab;
    [SerializeField] protected AudioClip _powerupClip;

    protected GameObject _owner;    
    private bool _isActive;

    protected abstract void AddToListener();    // Not all powerups will register to a listener, implementing class to decide how this is handled. 
    protected abstract void RemoveFromListener();   

    public virtual void Activate(GameObject owner)  // Called by the OnTriggerEnter2D event
    {
        this._owner = owner;        
        _isActive = true;
        _powerupDuration += Time.time;
        if(_powerupClip)
            AudioSource.PlayClipAtPoint(_powerupClip, transform.position);

        AddToListener();
        Debug.Log(this + " Powerup Activated for " + _powerupDuration + " second(s)");
    }

    protected virtual void OnEntityFiredWeapon()
    {
        if (_powerupPrefab != null)
            Instantiate(_powerupPrefab, _owner.transform.position, Quaternion.identity);
    }

    protected virtual void Update()
    {
        if (_isActive)
        {            
            if (Time.time > _powerupDuration)
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

    public virtual void SetCollectionMask(string mask) { _collectingEntityTag = mask; }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {      
        if (collision.TryGetComponent(out Entity_Base entity))
        {
            if (!collision.CompareTag(_collectingEntityTag))
                return;

            if(TryGetComponent(out SpriteRenderer spriteRenderer))
                spriteRenderer.enabled = false;
            if(TryGetComponent(out Collider2D collider))
                collider.enabled = false;

            SpriteRenderer[] spriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].enabled = false;     // Health Powerup contains more than one sprite to disable
            }

            Activate(entity.gameObject);
        }            
    }
}
