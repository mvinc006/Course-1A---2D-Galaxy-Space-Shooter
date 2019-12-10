using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Enemy : Entity_Base, IDamageable
{
    [Header("Implementing Class Settings")]
    [SerializeField] Vector2 _fireSpeed;
    [SerializeField, Range(1, 100)] int _pointsOnKill = 20;
    [SerializeField,Space] GameObject _explosionPrefab;
    [SerializeField, Space] GameObject _laserPrefab;

    private Player _player;
    private Collider2D _collider;
    private bool _missileTracked;
    private float _canFire;


    private void Start()
    {        
        InitCheck();
        _entityRigidBody = GetComponent<Rigidbody2D>();
        LaserMask = "Enemy Laser";
        SpawnManager.ActiveEnemies.Add(this);
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

    public void OnMissileTracked(bool value)
    {
        _missileTracked = value;
    }

    public bool bIsMissleTracked()
    {
        return _missileTracked;
    }

    public override void OnDealDamage()
    {
        if (Time.time >= _canFire && !ShouldFireSpecialWeapon())
        {
            _canFire = Time.time + Random.Range(_fireSpeed.x, _fireSpeed.y);
            Instantiate(_laserPrefab, transform.position, Quaternion.identity);            
        }                    
    }

    private void Update()
    {        
        OnDealDamage();        
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        if (transform.position.y <= -5.5f && _collider.enabled == true)
            _entityRigidBody.MovePosition(transform.position + new Vector3(Random.Range(-8f, 8f), 9f, 0f));

        _entityRigidBody.MovePosition(transform.position + (Vector3.down * _speed * Time.fixedDeltaTime));        
    }

    public override void OnTakeDamage()
    {
        OnDeath();
    }
    public override void OnDeath()
    {
        _player.OnScoreUpdate(_pointsOnKill);
        _collider.enabled = false;
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 3f);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Missile"))
        {            
            Destroy(other.gameObject);            
            OnDeath();
        }
        else if (other.TryGetComponent(out IDamageable entity) && !other.CompareTag(gameObject.tag))
        {                        
            entity.OnTakeDamage();
            Debug.Log(other.name + " with tag: " + other.tag + " caused " + gameObject.name + " with tag: " + gameObject.tag + " to call OnDeath()");
            OnDeath();
        }                                    
    }

    private void OnDestroy()
    {
        SpawnManager.ActiveEnemies.Remove(this);
    }

    public override void OnSpeedBoost(float speed)
    {
        throw new System.NotImplementedException();
    }
}
