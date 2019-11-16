using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField, Range(1.0f, 10.0f)] float speed = 4f;
    [SerializeField] Vector2 _fireSpeed;
    [SerializeField, Range(1, 100)] int _pointsOnKill = 20;
    [SerializeField,Space] GameObject _explosionPrefab;
    [SerializeField, Space] GameObject _laserPrefab;

    private Player _player;
    private Collider2D _collider;
    private float _canFire;
    private void Start()
    {
        InitCheck();                
    }

    private void InitCheck()
    {
        if (GameObject.FindGameObjectWithTag("Player").TryGetComponent(out Player player))
            _player = player;
        else
            Debug.LogError("Enemy:: Scene must contain a Player with tag 'Player'.");

        if (TryGetComponent(out Collider2D collider))
            _collider = collider;
        else
            Debug.LogError("Enemy:: Enemy must contain a 2D collider.");

        if (!_explosionPrefab)
            Debug.LogError("Enemy:: Missing reference 'ExplosionPrefab' must be assinged in inspector.");
        if (!_laserPrefab)
            Debug.LogError("Enemy:: Missing reference 'LaserPrefab' must be assinged in inspector.");
    }

    private void Fire()
    {
        if (Time.time >= _canFire)
        {
            _canFire = Time.time + Random.Range(_fireSpeed.x, _fireSpeed.y);
            Instantiate(_laserPrefab, transform.position, Quaternion.identity);            
        }                    
    }

    private void Update()
    {
        Movement();
        Fire();
    }

    private void Movement()
    {
        if (transform.position.y <= -5.5f && _collider.enabled == true)
            transform.position = new Vector3(Random.Range(-8f, 8f), 9f, 0f);

        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    private void OnDeath()
    {
        _player.OnScoreUpdate(_pointsOnKill);
        _collider.enabled = false;
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 3f);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
        
            Destroy(other.gameObject);
            OnDeath();
        }
        else if (other.CompareTag("Player") || other.CompareTag("Shield"))
        {
            _player.OnTakeDamage();
            OnDeath();
        }        
    }
}
