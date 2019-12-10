using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Player : Entity_Base
{
    [Header("Implementing Class Settings")]
    [SerializeField, Space] Animator _cameraShake;
    [SerializeField, Range(0.01f, 2.0f)] float _fireRate = 0.5f;
    [SerializeField, Range(1f, 3f)] float _thrusterBoostTime;
    [SerializeField, Range(0f, 256)] int _ammoAmount = 15;
    [SerializeField] int _maxLives = 3;    

    private Animator _anim;
    private Player_Containers _playerContainer;
    
    private float _canFire = -1f;    
    private int _lives = 3;
    private int _score = 0;
    private float _canBoost = 3f;
    private bool _boostRecharging;
    private float _thrusterSpeedBonus = 1f;
    private float _speedMultiplier = 1.0f;
    private Vector3 _keyboardInput;    

    void Start()
    {
        transform.position = new Vector3(0f, -3.8f, 0f);
        _entityRigidBody = GetComponent<Rigidbody2D>();
        InitCheck();
        LaserMask = "Player Laser";
        _playerContainer.GetUIManager().OnThrusterUpdate(_canBoost, _thrusterBoostTime);
        _playerContainer.GetUIManager().OnAmmoUpdate(_ammoAmount);
    }

    private void InitCheck()
    {       
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
        CheckFireInput();
        CheckMovementInput();        
    }

    private void FixedUpdate()
    {
        CalculateMovement();
    }

    private void CheckFireInput()
    {
        if (Input.GetKey(KeyCode.Space))
            OnDealDamage();
    }

    private void CheckMovementInput()
    {
        _keyboardInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
        if (_keyboardInput.sqrMagnitude > 0f)
            OnMove();
        if (Input.GetKey(KeyCode.LeftShift) && _boostRecharging == false)
            OnBoost();
        else if (!Input.GetKey(KeyCode.LeftShift) || _boostRecharging == true)
            OnBoostRecharging();
    }    

    private void CalculateMovement()
    {
        Vector3 _destination = transform.position + (_keyboardInput * _speed * _speedMultiplier * _thrusterSpeedBonus * Time.fixedDeltaTime);
        _destination.y = Mathf.Clamp(_destination.y, _verticalBoundMin, _verticalBoundMax); // Clamp Y
        // Wrap around
        if (_destination.x < _horizontalBoundMin)
            _destination.x = _horizontalBoundMax;
        if (_destination.x > _horizontalBoundMax)
            _destination.x = _horizontalBoundMin;

        _entityRigidBody.MovePosition(_destination);
    }

    public void OnMove()
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

    public override void OnDealDamage()
    {                
        // TripleShot is a powerup and therefore exempt from the rules of ammunition
        if (ShouldFireSpecialWeapon())
        {                        
            // ShouldFireSpecialWeapon calls the OnFire action from base class for us if true. We don't want to execute ammo related code, so we return.
            return;
        }
            
        if (_ammoAmount > 0 && Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            OnFireStandardShot();
        }        
        else
        {
            _playerContainer.GetUIManager().OnAmmoUpdate(_ammoAmount);
            return;
        }        
    }

    private void OnFireStandardShot()
    {
        Instantiate(_playerContainer.GetLaser(), transform.position + (Vector3.up * 1.05f), Quaternion.identity, _playerContainer.GetLaserContainer()).name = "Player_Laser";
        _ammoAmount--;
        _playerContainer.GetUIManager().OnAmmoUpdate(_ammoAmount);
       // AudioSource.PlayClipAtPoint(_playerContainer.GetLaserSound(), transform.position, 0.5f);
    }

    public override void OnTakeDamage()
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

    public override void OnDeath()
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
        _ammoAmount += ammo;
        _playerContainer.GetUIManager().OnAmmoUpdate(_ammoAmount);
    }

    public void OnHealthPickup()
    {       
        _lives++;        
        _lives = Mathf.Clamp(_lives, 0, _maxLives);
        _playerContainer.GetUIManager().OnUpdateLives(_lives);

        if (_playerContainer.GetLeftWing().activeInHierarchy)
            _playerContainer.GetLeftWing().SetActive(false);
        else if (_playerContainer.GetRightWing().activeInHierarchy)
            _playerContainer.GetRightWing().SetActive(false);
    }

    public override void OnSpeedBoost(float speed)
    {
        _speedMultiplier = speed;
    }
}


