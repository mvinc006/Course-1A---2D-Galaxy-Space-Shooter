using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    [SerializeField, Space] AudioClip _laserSound;
    [SerializeField] AudioClip _explosionSound;
    [SerializeField] AudioClip _powerupSound;
    [SerializeField] AudioSource _gameMusic;
    public AudioClip GetLaserSound { get => _laserSound;}
    public AudioClip GetExplosionSound { get => _explosionSound; }    
    public AudioSource GetGameMusic { get => _gameMusic; }
    public AudioClip GetPowerupSound { get => _powerupSound; }

    private void Start()
    {
        if (_laserSound == null)
            Debug.LogError("Audio Manager is missing LaserSound reference in inspector.");
        if (_explosionSound == null)
            Debug.LogError("Audio Manager is missing ExplosionSound reference in inspector.");
        if (_gameMusic == null)
            Debug.LogError("Audio Manager is missing GameMusic reference in inspector.");
        if (_powerupSound == null)
            Debug.LogError("Audio Manager is missing PowerUp reference in inspector.");
    }
}
