using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] GameObject _laser;
    [SerializeField] GameObject _tripleShotLaser;
    [SerializeField] GameObject _missle;
    [SerializeField] GameObject _shield;
    [SerializeField] Transform _laserContainer;

    [SerializeField, Space] GameObject _leftWing;
    [SerializeField] GameObject _rightWing;
    [SerializeField] SpriteRenderer _thruster;
    [SerializeField] GameObject _explosion;

    [SerializeField, Space] Animator _cameraShake;

    [SerializeField, Space, Range(1.0f, 10.0f)] float _speed = 3f;
    [SerializeField, Range(0.01f, 2.0f)] float _fireRate = 0.5f;
    [SerializeField, Range(1f, 3f)] float _thrusterBoostTime;
    [SerializeField, Range(0f, 256)] int _currentAmmo = 15;
    [SerializeField] int _maxLives = 3;
    

    private const float _verticalBoundMin = -3.8f;
    private const float _verticalBoundMax = 0f;
    private const float _horizontalBoundMin = -11.3f;
    private const float _horizontalBoundMax = 11.3f;    

    private SpawnManager _spawnManager;
    private Audio_Manager _audioManager;
    private UI_Manager _uiManager;
    private Animator _anim;

    private float _canFire = -1f;
    private float _canBoost = 3f;
    private int _lives = 3;
    private bool _boostRecharging;
    private float _speedBoostBonus = 1f;
    private float _thrusterSpeedBonus = 1f;
    private int _score = 0;
    private bool _bIsTripleShotActive;
    private bool _bIsMissileShotActive;
    private bool _bIsSpeedBoostActive;
    private IEnumerator TripleShot;
    private IEnumerator SpeedBoost;
    private IEnumerator MissileShot;

    public string laserMask { get ; set ; }

    void Start()
    {
        transform.position = new Vector3(0f, -3.8f, 0f);       
        InitCheck();
        laserMask = "Player Laser";
        _uiManager.OnThrusterUpdate(_canBoost, _thrusterBoostTime);
        _uiManager.OnAmmoUpdate(_currentAmmo);
    }

    private void InitCheck()
    {
        // Managers and component checks
        if (GameObject.FindGameObjectWithTag("Spawn Manager").TryGetComponent(out SpawnManager manager))
            _spawnManager = manager;
        else
            Debug.LogError("Player:: Scene must contain gameobject with 'Spawn Manager' Tag");

        if (GameObject.FindGameObjectWithTag("UI_Manager").TryGetComponent(out UI_Manager UIManager))
            _uiManager = UIManager;
        else
            Debug.LogError("Player:: Scene must contain gameobject with 'UI_Manager' Tag");

        if (GameObject.FindGameObjectWithTag("Audio_Manager").TryGetComponent(out Audio_Manager audioManager))
            _audioManager = audioManager;
        else
            Debug.LogError("Player:: Scene must contain gameobject with 'Audio_Manager' Tag");

        if (TryGetComponent(out Animator anim))
            _anim = anim;
        else
            Debug.LogError("Player must contain an Animator component");
       

        // Basic GameObjects and Transforms etc
        if (!_leftWing)
            Debug.LogError("Player:: Must assign 'LeftWing' reference in inspector.");
        if (!_rightWing)
            Debug.LogError("Player:: Must assign 'RightWing' reference in inspector.");
        if(!_laser)
            Debug.LogError("Player:: Must assign 'Laser' reference in inspector.");
        if(!_tripleShotLaser)
            Debug.LogError("Player:: Must assign 'TripleShotLaser' reference in inspector.");
        if(!_laserContainer)
            Debug.LogError("Player:: Must assign 'LaserContainer' reference in inspector.");
        if (!_shield)
            Debug.LogError("Player:: Must assign 'Shield' reference in inspector.");
        if (!_explosion)
            Debug.LogError("Player:: Must assign 'Explosion' reference in inspector.");
    }

    void Update()
    {             
        if (Input.GetKey(KeyCode.Space) && Time.time > _canFire)
            Fire();        
        if (Input.GetKey(KeyCode.LeftShift) && _boostRecharging == false)
            OnBoost();        
        else if (!Input.GetKey(KeyCode.LeftShift) || _boostRecharging == true)
            OnBoostRecharging();

            CalculateMovement();
    }

    private void CalculateMovement()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
        transform.Translate(direction * _speed * _speedBoostBonus * _thrusterSpeedBonus * Time.deltaTime);

        // Clamp Vertical
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _verticalBoundMin, _verticalBoundMax), transform.position.z);

        // Wrap-around Horiztonal
        if (transform.position.x >= _horizontalBoundMax)
            transform.position = new Vector3(_horizontalBoundMin, transform.position.y, transform.position.z);
        else if (transform.position.x <= -11.3f)
            transform.position = new Vector3(_horizontalBoundMax, transform.position.y, transform.position.z);

        OnMove();        
    }

    private void OnMove()
    {
        _anim.SetFloat("xVel", Input.GetAxis("Horizontal"));
    }   
    

    private void OnBoost()
    {
        // if we can boost, then boost and apply speed bonus.
        if(_canBoost >0 && _boostRecharging == false)
        {
            _canBoost -= 1.0f * Time.deltaTime;
            _thrusterSpeedBonus = 2f;
            _thruster.color = Color.cyan;
        }
        else
        {            
            _boostRecharging = true;
            _thruster.color = Color.white;
            OnBoostDisable();
        }

        _uiManager.OnThrusterUpdate(_canBoost, _thrusterBoostTime);
    }

   
    void OnBoostRecharging()
    {        
        _boostRecharging = true;        
        _canBoost += 1f * Time.deltaTime;
        _canBoost = Mathf.Clamp(_canBoost, 0f, _thrusterBoostTime);

        // disable recharging only when we are fully recharged AND left shift button isn't pressed/held
        if (_canBoost >= (_thrusterBoostTime / _thrusterBoostTime) && !Input.GetKey(KeyCode.LeftShift))
        {
            _boostRecharging = false;
            _thruster.color = Color.white;
        }
        else
            OnBoostDisable();
                    
        _uiManager.OnThrusterUpdate(_canBoost, _thrusterBoostTime);
    }         
        

    private void OnBoostDisable()
    {       
        _thrusterSpeedBonus = 1f;                    
    }

    private void Fire()
    {        
        _canFire = Time.time + _fireRate;                

        // TripleShot is a powerup and therefore exempt from the rules of ammunition
        if (_bIsTripleShotActive)
        {
            OnFireTripleShot();            
        }        
        else if (_bIsMissileShotActive)
        {
            OnMissileShot();
        }
        else if(_currentAmmo > 0)
        {
            OnFireStandardShot();
        }        
        else
        {            
            _uiManager.OnAmmoUpdate(_currentAmmo);
            return;
        }        
    }

    private void OnFireStandardShot()
    {
        Instantiate(_laser, transform.position + (Vector3.up * 1.05f), Quaternion.identity, _laserContainer).name = "Player_Laser";
        _currentAmmo--;
        _uiManager.OnAmmoUpdate(_currentAmmo);
        AudioSource.PlayClipAtPoint(_audioManager.GetLaserSound, transform.position, 0.5f);
    }

    private void OnFireTripleShot()
    {
        Instantiate(_tripleShotLaser, transform.position, Quaternion.identity, _laserContainer).name = "TripleShot";
        AudioSource.PlayClipAtPoint(_audioManager.GetLaserSound, transform.position, 0.5f);
    }

    private void OnMissileShot()
    {
        float closestDistance = 9999;
        Enemy bestTarget = null;

        // Iterate through the scenes list of enemies and check which is the closest to us.
        foreach(Enemy target in GameObject.FindObjectsOfType<Enemy>())
        {
            // check if the target is already tracked
            if (!target.OnCheckMissileTracked())
            {
                Vector2 distance = target.transform.position - transform.position;
                if(distance.magnitude < closestDistance)
                {
                    bestTarget = target;
                }
            }
        }

        if(bestTarget != null)
        {
            bestTarget.OnMissileTracked();
            Instantiate(_missle, transform.position, Quaternion.identity).GetComponent<Missile_Behaviour>().OnSetTarget(bestTarget.transform);
        }
            
    }

    public void OnTakeDamage()
    {
        if (_shield.TryGetComponent(out IShield shield))
        {            
            if(shield.shieldStatus)
                return;
        }

        _lives -= 1;
        _uiManager.OnUpdateLives(_lives);
        _cameraShake.SetTrigger("Shake");
        OnWingDamage();                
                           
        if(_lives <= 0)
            OnDeath();
        
    }

    private void OnWingDamage()
    {
        int randomWingDamage = Random.Range(0, 100);
        if (!_leftWing.activeInHierarchy && randomWingDamage <= 49 || _rightWing.activeInHierarchy)
            _leftWing.SetActive(true);
        else if (!_rightWing.activeInHierarchy && randomWingDamage >= 50 || _leftWing.activeInHierarchy)
            _rightWing.SetActive(true);        
    }

    public void OnDeath()
    {
        _speed = 0f;                      
        _spawnManager.OnPlayerDeath();
        _uiManager.OnGameOver();
        AudioSource.PlayClipAtPoint(_audioManager.GetExplosionSound, transform.position, 0.04f);
        
        foreach(SpriteRenderer renderer in this.transform.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = false;
        }

        GameObject explosion = Instantiate(_explosion, transform.position, Quaternion.identity);
        Destroy(explosion, 3f);
        Destroy(gameObject, 3f);
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

    public void OnTripleShotActive(WaitForSeconds duration)
    {
        _bIsTripleShotActive = true;        
        if (TripleShot !=null)
            StopCoroutine(TripleShot);

        if(MissileShot != null && _bIsMissileShotActive == true)
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
        if (_shield.TryGetComponent(out IShield shield))
            shield.OnInit(life, ShieldColorRange, ownerTag);                
    }
   
    public void OnScoreUpdate(int increment)
    {
        _score += increment;
        _uiManager.UpdateScore(_score);
    }
    
    public void OnAmmoActive(int ammo)
    {
        _currentAmmo += ammo;
        _uiManager.OnAmmoUpdate(_currentAmmo);
    }

    public void OnHealthPickup()
    {       
        _lives++;
        _lives = Mathf.Clamp(_lives, 0, _maxLives);
        _uiManager.OnUpdateLives(_lives);
    }

    public void OnMissileActive(WaitForSeconds duration)
    {
        _bIsMissileShotActive = true;
        if (MissileShot != null)
            StopCoroutine(MissileShot);
        
        if(TripleShot!=null && _bIsTripleShotActive == true)
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
    private IEnumerator MissileTimer(WaitForSeconds duration)
    {
        yield return duration;
        OnMissileDisable();
    }


}


