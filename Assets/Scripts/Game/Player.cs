using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour, IDamageable, IShoot, IMove
{

    [SerializeField, Space] Animator _cameraShake;
    [SerializeField, Space, Range(1.0f, 10.0f)] float _speed = 3f;
    [SerializeField, Range(0.01f, 2.0f)] float _fireRate = 0.5f;
    [SerializeField, Range(1f, 3f)] float _thrusterBoostTime;
    [SerializeField, Range(0f, 256)] int _currentAmmo = 15;
    [SerializeField] int _maxLives = 3;    

    private const float _verticalBoundMin = -3.8f;
    private const float _verticalBoundMax = 0f;
    private const float _horizontalBoundMin = -10f;
    private const float _horizontalBoundMax = 10f;    
    
    private Animator _anim;
    private Player_Containers _playerContainer;
    private Rigidbody2D playerRigidbodyComponent;

    private float _canFire = -1f;
    private float _canBoost = 3f;
    private int _lives = 3;
    private bool _boostRecharging;
    private float _thrusterSpeedBonus = 1f;
    private int _score = 0;

    public event Action OnFire;    
    private float SpeedMultiplier = 1.0f;
    public string laserMask { get ; set ; }

    private Vector3 playerInput;
    void Start()
    {
        transform.position = new Vector3(0f, -3.8f, 0f);
        playerRigidbodyComponent = GetComponent<Rigidbody2D>();
        InitCheck();
        laserMask = "Player Laser";
        _playerContainer.GetUIManager().OnThrusterUpdate(_canBoost, _thrusterBoostTime);
        _playerContainer.GetUIManager().OnAmmoUpdate(_currentAmmo);
    //    Powerup_Base.PrimaryWeaponChange += OnWeaponChanged;
        /*Powerup_Base.AddWeaponChangeListener(OnWeaponChanged);*/
    }

    private void InitCheck()
    {
        // Managers and component checks           

        if (TryGetComponent(out Animator anim))
            _anim = anim;
        else
            Debug.LogError("Player must contain an Animator component");        

        if (TryGetComponent(out Player_Containers pContainer))
            _playerContainer = pContainer;

        // Basic GameObjects and Transforms etc
        if (!_playerContainer.GetLeftWing())
            Debug.LogError("Player:: Must assign 'LeftWing' reference in inspector.");
        if (!_playerContainer.GetRightWing())
            Debug.LogError("Player:: Must assign 'RightWing' reference in inspector.");
        
    }

    void Update()
    {             
        if (Input.GetKey(KeyCode.Space))
            Fire();        
        if (Input.GetKey(KeyCode.LeftShift) && _boostRecharging == false)
            OnBoost();        
        else if (!Input.GetKey(KeyCode.LeftShift) || _boostRecharging == true)
            OnBoostRecharging();

        playerInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
        OnMove();        
    }

    private void FixedUpdate()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        Vector3 destination = transform.position + (playerInput * _speed * SpeedMultiplier * _thrusterSpeedBonus * Time.fixedDeltaTime);
                
        destination.y = Mathf.Clamp(destination.y, _verticalBoundMin, _verticalBoundMax); // Clamp Y
        // Wrap around
        if (destination.x < _horizontalBoundMin)
            destination.x = _horizontalBoundMax;
        if (destination.x > _horizontalBoundMax)
            destination.x = _horizontalBoundMin;

        playerRigidbodyComponent.MovePosition(destination);                              
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
            _playerContainer.GetThruster().color = Color.cyan;
        }
        else
        {            
            _boostRecharging = true;
            _playerContainer.GetThruster().color = Color.white;
            OnBoostDisable();
        }

        _playerContainer.GetUIManager().OnThrusterUpdate(_canBoost, _thrusterBoostTime);
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
            _playerContainer.GetThruster().color = Color.white;
        }
        else
            OnBoostDisable();

        _playerContainer.GetUIManager().OnThrusterUpdate(_canBoost, _thrusterBoostTime);
    }         
        

    private void OnBoostDisable()
    {       
        _thrusterSpeedBonus = 1f;                    
    }

    private void Fire()
    {                
        // TripleShot is a powerup and therefore exempt from the rules of ammunition
        if (OnFire != null)
        {            
            OnFire();
            return;
        }
            
        if (_currentAmmo > 0 && Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            OnFireStandardShot();
        }        
        else
        {
            _playerContainer.GetUIManager().OnAmmoUpdate(_currentAmmo);
            return;
        }        
    }

    private void OnFireStandardShot()
    {
        Instantiate(_playerContainer.GetLaser(), transform.position + (Vector3.up * 1.05f), Quaternion.identity, _playerContainer.GetLaserContainer()).name = "Player_Laser";
        _currentAmmo--;
        _playerContainer.GetUIManager().OnAmmoUpdate(_currentAmmo);
       // AudioSource.PlayClipAtPoint(_playerContainer.GetLaserSound(), transform.position, 0.5f);
    }

    public void OnTakeDamage()
    {
      /*  if (_powerupManager.GetShield().TryGetComponent(out Player_Shield shield))
        {            
            if(shield.shieldStatus)
                return;
        }*/

        _lives -= 1;
        _playerContainer.GetUIManager().OnUpdateLives(_lives);
        _cameraShake.SetTrigger("Shake");
        OnWingDamage();                
                           
        if(_lives <= 0)
            OnDeath();  
    }

    private void OnWingDamage()
    {
        int randomWingDamage = UnityEngine.Random.Range(0, 100);
        if (!_playerContainer.GetLeftWing().activeInHierarchy && randomWingDamage <= 49 || _playerContainer.GetRightWing().activeInHierarchy)
            _playerContainer.GetLeftWing().SetActive(true);
        else if (!_playerContainer.GetRightWing().activeInHierarchy && randomWingDamage >= 50 || _playerContainer.GetLeftWing().activeInHierarchy)
            _playerContainer.GetRightWing().SetActive(true);        
    }

    public void OnDeath()
    {
        _speed = 0f;                      
        _playerContainer.GetSpawnManager().OnPlayerDeath();
        _playerContainer.GetUIManager().OnGameOver();
       // AudioSource.PlayClipAtPoint(_playerContainer.GetExplosionSound(), transform.position, 0.04f);
        
        foreach(SpriteRenderer renderer in this.transform.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = false;
        }

        GameObject explosion = Instantiate(_playerContainer.GetExplosion(), transform.position, Quaternion.identity);
        Destroy(explosion, 3f);
        Destroy(gameObject, 3f);
    }
 
    public void OnScoreUpdate(int increment)
    {
        _score += increment;
        _playerContainer.GetUIManager().UpdateScore(_score);
    }
    
    public void OnAmmoActive(int ammo)
    {
        _currentAmmo += ammo;
        _playerContainer.GetUIManager().OnAmmoUpdate(_currentAmmo);
    }

    public void OnHealthPickup()
    {       
        _lives++;        
        _lives = Mathf.Clamp(_lives, 0, _maxLives);
        _playerContainer.GetUIManager().OnUpdateLives(_lives);
    }

    public void OnSpeedBoost(float speed)
    {
        SpeedMultiplier = speed;
    }
}


