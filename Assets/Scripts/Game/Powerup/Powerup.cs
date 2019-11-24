﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] float _powerupMoveSpeed = 4f;
    [SerializeField] int _shieldLife;
    [SerializeField] int _ammo = 100;
    [SerializeField] float _duration = 10f;
    [SerializeField] string _ownerTag;
    [SerializeField] PowerUpType _powerupType;
    [SerializeField] Color[] _shieldHitColorRange;

    private Audio_Manager _audioManager;
    private WaitForSeconds _expireTime;

    // define a delegate
    // define an event
    // raise the event


    public delegate void OnTripleShotEventHandler(WaitForSeconds duration);
    public delegate void OnSpeedBoostEventHandler(WaitForSeconds duration);
    public delegate void OnShieldEventHandler(int life, Color[] ShieldColorRange, string ownerTag);
    public delegate void OnAmmoEventHandler(int ammo);
    public delegate void OnHealthEventHandler();
    public delegate void OnMissileEventHandler(WaitForSeconds duration);

    public static event OnTripleShotEventHandler TripleShotRaised;
    public static event OnSpeedBoostEventHandler SpeedBoostRaised;
    public static event OnShieldEventHandler ShieldRaised;
    public static event OnAmmoEventHandler AmmoRaised;
    public static event OnHealthEventHandler HealthRaised;
    public static event OnMissileEventHandler MissileRaised;   

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
        transform.Translate(Vector3.down * _powerupMoveSpeed * Time.deltaTime);
        if (transform.position.y <= -5.5f)
            Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out Powerup_Manager powerupManager))
            {
                switch (_powerupType)
                {
                    case PowerUpType.TripleShot:
                       // powerupManager.OnTripleShotActive(_expireTime);
                        // event
                        TripleShotRaised(_expireTime);
                        break;
                    case PowerUpType.SpeedBoost:
                    //    powerupManager.OnSpeedBoostActive(_expireTime);
                        // event
                        SpeedBoostRaised(_expireTime);
                        break;
                    case PowerUpType.Shield:
                   //     powerupManager.OnShieldActive(_shieldLife, _shieldHitColorRange, _ownerTag);
                        // event
                        ShieldRaised(_shieldLife, _shieldHitColorRange, _ownerTag);
                        break;
                    case PowerUpType.Ammo:
                   //     powerupManager.OnAmmoActive(_ammo);
                        // event
                        AmmoRaised(_ammo);
                        break;
                    case PowerUpType.Health:
                     //   powerupManager.OnHealthPickup();
                        // event
                        HealthRaised();
                        break;
                    case PowerUpType.Missile:
                   //     powerupManager.OnMissileActive(_expireTime);
                        // event
                        MissileRaised(_expireTime);
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
