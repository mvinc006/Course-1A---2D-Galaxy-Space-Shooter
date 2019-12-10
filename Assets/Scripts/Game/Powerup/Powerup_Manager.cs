using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_Manager: MonoBehaviour
{
    [SerializeField] GameObject _tripleShotLaser;
    [SerializeField] GameObject _missle;
    [SerializeField] GameObject _shield;
    [SerializeField] Transform _laserContainer;
    [SerializeField] Audio_Manager _audioManager;
    private Player _player;
    private float _speedBoostBonus = 1f;
    private bool _bIsTripleShotActive;
    private bool _bIsMissileShotActive;
    private bool _bIsSpeedBoostActive;
    private IEnumerator TripleShot;
    private IEnumerator SpeedBoost;
    private IEnumerator MissileShot;

    public float GetSpeedBonus() { return _speedBoostBonus; }    
    public bool GetTripleShotStatus() { return _bIsTripleShotActive; }
    public bool GetMissileShotStatus() { return _bIsMissileShotActive; }
    public GameObject GetShield() { return _shield; }

    private void Start()
    {
        if (TryGetComponent(out Player player))
            _player = player;        
    }

    private void OnEnable()
    {
      /*  Powerup.TripleShotRaised += OnTripleShotActive;
        Powerup.SpeedBoostRaised += OnSpeedBoostActive;
        Powerup.ShieldRaised += OnShieldActive;
        Powerup.MissileRaised += OnMissileActive;
        Powerup.HealthRaised += OnHealthPickup;
        Powerup.AmmoRaised += OnAmmoActive;        */
    }

    public void OnFireTripleShot()
    {
        Instantiate(_tripleShotLaser, transform.position, Quaternion.identity, _laserContainer).name = "TripleShot";
        AudioSource.PlayClipAtPoint(_audioManager.GetLaserSound, transform.position, 0.5f);
    }

   /* public void OnMissileShot()
    {
        float closestDistance = 9999;
        Enemy bestTarget = null;

        // Iterate through the scenes list of enemies and check which is the closest to us.
        foreach (Enemy target in GameObject.FindObjectsOfType<Enemy>())
        {
            // check if the target is already tracked
            if (!target.OnCheckMissileTracked())
            {
                Vector2 distance = target.transform.position - transform.position;
                if (distance.magnitude < closestDistance)
                {
                    bestTarget = target;
                }
            }
        }

        if (bestTarget != null)
        {
            bestTarget.OnMissileTracked();
            Instantiate(_missle, transform.position, Quaternion.identity).GetComponent<Missile_Behaviour>().OnSetTarget(bestTarget.transform);
        }
    }*/

    public void OnTripleShotActive(WaitForSeconds duration)
    {
        _bIsTripleShotActive = true;
        if (TripleShot != null)
            StopCoroutine(TripleShot);

        if (MissileShot != null && _bIsMissileShotActive == true)
        {
            StopCoroutine(MissileShot);
            _bIsMissileShotActive = false;
        }

        TripleShot = TripleShotTimer(duration);
        StartCoroutine(TripleShot);
    }

    private void OnTripleShotDisable()
    {
        _bIsTripleShotActive = false;
    }

    public void OnSpeedBoostActive(WaitForSeconds duration)
    {
        _speedBoostBonus = 2f;
        _bIsSpeedBoostActive = true;
        if (SpeedBoost != null)
            StopCoroutine(SpeedBoost);

        SpeedBoost = SpeedBoostTimer(duration);
        StartCoroutine(SpeedBoost);
    }

    private void OnSpeedBoostDisable()
    {
        _bIsSpeedBoostActive = false;
        _speedBoostBonus = 1f;
    }

    public void OnShieldActive(int life, Color[] ShieldColorRange, string ownerTag)
    {
        if (_shield.TryGetComponent(out Player_Shield shield))
            shield.OnInit(life, ShieldColorRange, ownerTag);
    }

    public void OnMissileActive(WaitForSeconds duration)
    {
        _bIsMissileShotActive = true;
        if (MissileShot != null)
            StopCoroutine(MissileShot);

        if (TripleShot != null && _bIsTripleShotActive == true)
        {
            StopCoroutine(TripleShot);
            _bIsTripleShotActive = false;
        }
        MissileShot = MissileTimer(duration);
        StartCoroutine(MissileShot);
    }

    private void OnMissileDisable()
    {
        _bIsMissileShotActive = false;
    }

    public void OnHealthPickup()
    {
        _player.OnHealthPickup();
    }

    public void OnAmmoActive(int ammo)
    {
        _player.OnAmmoActive(ammo);
    }

    private IEnumerator MissileTimer(WaitForSeconds duration)
    {
        yield return duration;
        OnMissileDisable();
    }

    private IEnumerator TripleShotTimer(WaitForSeconds duration)
    {
        yield return duration;
        OnTripleShotDisable();
    }

    private IEnumerator SpeedBoostTimer(WaitForSeconds duration)
    {
        yield return duration;
        OnSpeedBoostDisable();
    }
}
