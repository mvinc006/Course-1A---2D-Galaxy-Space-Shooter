using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Laser_Behaviour : MonoBehaviour
{
    [SerializeField] float _laserSpeed = 5.0f;
    [SerializeField] bool _allowMultipleCollisions;
    [SerializeField] private Vector3 _direction;

    private bool _hasCollided;

    private void Start()
    {
        if (_direction == Vector3.down)
            gameObject.tag = "Enemy Laser";
        else
            gameObject.tag = "Player Laser";
    }

    void Update()
    {      
        transform.Translate(_direction * _laserSpeed * Time.deltaTime);
        
        if (_direction == Vector3.up) 
            OnPlayerLaserMove();
        else
            OnEnemyLaserMove();        
    }

    private void OnEnemyLaserMove()
    {        
        if (transform.position.y <= -5f)
            Destroy(gameObject.transform.parent.gameObject);
    }

    public void OnCollided()
    {
        _hasCollided = true;
    }
    private void OnPlayerLaserMove()
    {
        if (transform.position.y >= 8f && transform.parent.name == "TripleShot")
            Destroy(transform.parent.gameObject);
        else if (transform.position.y >= 8f)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (transform.parent.name == "EnemyLaser" && transform.parent.childCount-1 <= 0)
            Destroy(transform.parent.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if(collision.TryGetComponent(out IDamageable hitInfo))
        {
            // check if we are attacking our a shield
            if (collision.TryGetComponent(out IShield shieldInfo))
            {
                if (gameObject.CompareTag(shieldInfo.laserMask)) 
                {
                    return;
                }
                else
                {
                    MultipleCollisionCheck(hitInfo);
                }
            }
            else if (gameObject.CompareTag(hitInfo.laserMask))
            {
                return;                
            }
            else
            {
                MultipleCollisionCheck(hitInfo);
                Debug.Log("Laser with tag " + gameObject.tag + " is being destroyed by " + collision.name + " with tag " + collision.tag);                
            }
        }       
    }

    void MultipleCollisionCheck(IDamageable hitInfo)
    {
        if (!_hasCollided && !_allowMultipleCollisions)
        {
            foreach (Laser_Behaviour laser in transform.parent.GetComponentsInChildren<Laser_Behaviour>())
            {
                laser.OnCollided();
                Destroy(laser.gameObject);
            }

            hitInfo.OnTakeDamage();                       
        }
        else if (_allowMultipleCollisions)
        {
            hitInfo.OnTakeDamage();
            Destroy(gameObject);
        }
    }

}
