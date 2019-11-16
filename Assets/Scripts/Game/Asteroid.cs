using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider2D))]
public class Asteroid : MonoBehaviour
{
    [SerializeField] float _rotateSpeed;
    [SerializeField] GameObject _explosionPrefab;
    private SpawnManager _spawnManager;
    private Collider2D _collider;

    private void Start()
    {
        _spawnManager = GameObject.FindGameObjectWithTag("Spawn Manager").GetComponent<SpawnManager>();
        _collider = GetComponent<Collider2D>();
    }
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, 1f) * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Laser"))
        {
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _collider.enabled = false;
            _spawnManager.OnLevelStart();
            Destroy(collision.gameObject);
            Destroy(explosion, 3f);
            Destroy(gameObject);
        }

    }
}
