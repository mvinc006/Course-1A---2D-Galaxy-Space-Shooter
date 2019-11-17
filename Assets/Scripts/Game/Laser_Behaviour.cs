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
            gameObject.tag = "Enemy";
        else
            gameObject.tag = "Laser";
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
        
        if (collision.CompareTag("Player") && collision.TryGetComponent(out Player player) && !_hasCollided && !_allowMultipleCollisions && gameObject.CompareTag("Enemy"))
        {                       
            foreach(Laser_Behaviour laser in transform.parent.GetComponentsInChildren<Laser_Behaviour>())
            {
                laser.OnCollided();
            }
          
            player.OnTakeDamage();
            Destroy(gameObject.transform.parent.gameObject);
        }
        else if (collision.CompareTag("Player") && collision.TryGetComponent(out Player _player) && _allowMultipleCollisions && gameObject.CompareTag("Enemy"))
        {
            _player.OnTakeDamage();
            Destroy(gameObject);                    
        }            
    }

}
