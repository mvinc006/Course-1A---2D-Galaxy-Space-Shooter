using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] Sprite[] _Lives;
    [SerializeField] float _flickerDelaySeconds;
    
    private TMP_Text _scoreText;
    private GameObject _GameOverText;
    private Image _livesIMG;    
    private WaitForSeconds _flickerDelay;
    private GameManager _gameManager;

    private void Start()
    {
        _scoreText = transform.Find("Score").GetComponent<TMP_Text>();
        _livesIMG = transform.Find("Lives_Display").GetComponent<Image>();
        _GameOverText = transform.Find("GAME_OVER").gameObject;
        _gameManager = GameObject.FindGameObjectWithTag("Game_Manager").GetComponent<GameManager>();
        _flickerDelay = new WaitForSeconds(_flickerDelaySeconds);
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
