﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Powerup : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 4f;
    [SerializeField] float _duration = 10f;
    [SerializeField] PowerUpType _powerupType;

    private Audio_Manager _audioManager;
    private WaitForSeconds _expireTime;

    private void Start()
    {
        _expireTime = new WaitForSeconds(_duration);
        if (GameObject.FindGameObjectWithTag("Audio_Manager").TryGetComponent(out Audio_Manager audioManager))
            _audioManager = audioManager;
        else
            Debug.LogError("Powerup:: Needs an AudioManager gameobject in the scene tagged 'Audio_Manager'.");
    }
    void Update()
    {
        transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
        if (transform.position.y <= -5.5f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out Player player))
            {
                switch (_powerupType)
                {
                    case PowerUpType.TripleShot:
                        player.OnTripleShotActive(_expireTime);
                        break;
                    case PowerUpType.SpeedBoost:
                        player.OnSpeedBoostActive(_expireTime);
                        break;
                    case PowerUpType.Shield:
                        player.OnShieldActive();                        
                        break;
                }

                AudioSource.PlayClipAtPoint(_audioManager.GetPowerupSound, transform.position, 1.0f);
                Destroy(gameObject);            
            }
            else
            {
                Debug.LogError("Powerup:OnTriggerEnter2D -> collision object didn't have a 'Player' script attached");
            }                                
        }                        
    }

}