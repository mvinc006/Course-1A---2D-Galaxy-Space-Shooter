﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField, Range(1.0f, 10.0f)] float speed = 4f;
    [SerializeField] Vector2 _fireSpeed;
    [SerializeField, Range(1, 100)] int _pointsOnKill = 20;
    [SerializeField,Space] GameObject _explosionPrefab;
    [SerializeField, Space] GameObject _laserPrefab;

    private Player _player;
    private Collider2D _collider;
    private bool _missileTracked;
    private float _canFire;

    public string laserMask { get; set; }

    private void Start()
    {        
        InitCheck();
        laserMask = "Enemy Laser";
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

    public void OnMissileTracked()
    {
        _missileTracked = true;
    }

    public bool OnCheckMissileTracked()
    {
        return _missileTracked;
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

    public void OnTakeDamage()
    {
        OnDeath();
    }
    public void OnDeath()
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
    


}
