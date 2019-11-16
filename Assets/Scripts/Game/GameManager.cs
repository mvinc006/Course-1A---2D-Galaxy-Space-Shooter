using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool _isGameOver = false;    
    
    public void OnGameOver()
    {
        _isGameOver = true;
    }    

    private void GameOver()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene(1);
        }
    }    

    private void Update()
    {
        if (_isGameOver)
            GameOver();
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }
}
