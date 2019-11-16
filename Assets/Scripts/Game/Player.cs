using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    [SerializeField] GameObject _laser;
    [SerializeField] GameObject _tripleShotLaser;
    [SerializeField] GameObject _shield;
    [SerializeField] Transform _laserContainer;

    [SerializeField, Space] GameObject _leftWing;
    [SerializeField] GameObject _rightWing;

    
    [SerializeField] GameObject _explosion;

    [SerializeField, Space, Range(1.0f, 10.0f)] float _speed = 3f;
    [SerializeField, Range(0.01f, 2.0f)] float _fireRate = 0.5f;
    [SerializeField] int _lives = 3;

    private const float _verticalBoundMin = -3.8f;
    private const float _verticalBoundMax = 0f;
    private const float _horizontalBoundMin = -11.3f;
    private const float _horizontalBoundMax = 11.3f;    

    private SpawnManager _spawnManager;
    private Audio_Manager _audioManager;
    private UI_Manager _uiManager;
    private Animator _anim;

    private float _canFire = -1f;
    private float _speedBoostBonus = 1f;
    private int _score = 0;
    private bool _bIsTripleShotActive;
    private bool _bIsSpeedBoostActive;
    private bool _bIsShieldActive;
    private IEnumerator TripleShot;
    private IEnumerator SpeedBoost;

    void Start()
    {
        transform.position = new Vector3(0f, -3.8f, 0f);
        InitCheck();
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
        CalculateMovement();
     
        if (Input.GetKey(KeyCode.Space) && Time.time > _canFire)
            Fire();
    }

    private void CalculateMovement()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
        transform.Translate(direction * _speed * _speedBoostBonus * Time.deltaTime);

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

    private void Fire()
    {        
        _canFire = Time.time + _fireRate;
        
        if (_bIsTripleShotActive)
        {
            Instantiate(_tripleShotLaser, transform.position, Quaternion.identity, _laserContainer).name = "TripleShot";
        }
        else
        {
            Instantiate(_laser, transform.position + (Vector3.up * 1.05f), Quaternion.identity, _laserContainer).name = "Player_Laser";
        }

        AudioSource.PlayClipAtPoint(_audioManager.GetLaserSound, transform.position, 0.5f);                
    }

    public void OnTakeDamage()
    {
        if (_bIsShieldActive)
        {
            OnShieldDisable();
            return;
        }

        _lives -= 1;
        _uiManager.OnUpdateLives(_lives);
        OnWingDamage();                
                           
        if(_lives <= 0)
            OnGameOver();
        
    }

    private void OnWingDamage()
    {
        int randomWingDamage = Random.Range(0, 100);
        if (!_leftWing.activeInHierarchy && randomWingDamage <= 49 || _rightWing.activeInHierarchy)
            _leftWing.SetActive(true);
        else if (!_rightWing.activeInHierarchy && randomWingDamage >= 50 || _leftWing.activeInHierarchy)
            _rightWing.SetActive(true);        
    }

    private void OnGameOver()
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

        TripleShot = TripleShotTimer(duration);
        StartCoroutine(TripleShot);
    }

    public void OnTripleShotDisable()
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

    public void OnSpeedBoostDisable()
    {
        _bIsSpeedBoostActive = false;
        _speedBoostBonus = 1f;
    }

    public void OnShieldActive()
    {        
        _bIsShieldActive = true;
        _shield.SetActive(true);
    }

    public void OnShieldDisable()
    {
        _bIsShieldActive = false;
        _shield.SetActive(false);
    }
   
    public void OnScoreUpdate(int increment)
    {
        _score += increment;
        _uiManager.UpdateScore(_score);
    }
    
}


