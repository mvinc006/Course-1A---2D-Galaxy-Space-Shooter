using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] Sprite[] _Lives;
    [SerializeField] private Image _thrusterImage;
    [SerializeField] TMP_Text _thrusterTime;
    [SerializeField] TMP_Text _ammoCount;
    [SerializeField] TMP_Text _ammoWarning;
    [SerializeField] float _flickerDelaySeconds;
    
    private TMP_Text _scoreText;    
    private Image _livesIMG;
    private GameObject _GameOverText;
    private WaitForSeconds _flickerDelay;
    private GameManager _gameManager;

    private IEnumerator AmmoFlicker;
    private bool _AmmoFlickerRunning;

    private void Start()
    {
        _scoreText = transform.Find("Score").GetComponent<TMP_Text>();
        _livesIMG = transform.Find("Lives_Display").GetComponent<Image>();
        _GameOverText = transform.Find("GAME_OVER").gameObject;
        _gameManager = GameObject.FindGameObjectWithTag("Game_Manager").GetComponent<GameManager>();
        _flickerDelay = new WaitForSeconds(_flickerDelaySeconds);
        AmmoFlicker = OnAmmoEmpty();
    }

    public void UpdateScore(int score)
    {
        _scoreText.SetText("Score: " + score);
    }

    public void OnUpdateLives(int currentlives)
    {
        if (currentlives > _Lives.Length || currentlives < 0)
            return;

        _livesIMG.sprite = _Lives[currentlives];
    }

    public void OnThrusterUpdate(float currentPos, float endPos)
    {        
        _thrusterImage.fillAmount = (currentPos / endPos);
        _thrusterTime.SetText(currentPos.ToString("F0"));
    }

    public void OnAmmoUpdate(int ammo)
    {
        _ammoCount.SetText(ammo.ToString());
       
        if (ammo <= 0 && !_AmmoFlickerRunning)
        {
            StartCoroutine(AmmoFlicker);
            _AmmoFlickerRunning = true;
        }            
        else if (ammo !=0 && _AmmoFlickerRunning)
        {
            StopCoroutine(AmmoFlicker);
            _AmmoFlickerRunning = false;
        }            
    }

    private IEnumerator OnAmmoEmpty()
    {
        while (true)
        {
            _ammoWarning.enabled = !_ammoWarning.enabled;
            yield return _flickerDelay;
        }
        
    }
    public void OnGameOver()
    {
        _gameManager.OnGameOver();
        StartCoroutine(Flicker(_GameOverText));
    }

    private IEnumerator Flicker(GameObject flickerObject)
    {
        while (true)
        {                                
            _GameOverText.SetActive(!_GameOverText.activeInHierarchy);
            yield return _flickerDelay;
        }        
    }
}
