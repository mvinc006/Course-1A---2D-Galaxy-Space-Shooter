using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] GameObject[] _powerups;
    [SerializeField, Space] Transform _enemyContainer;  
    [SerializeField] Vector2 _enemySpawnDelay;
    [SerializeField] Vector2 _powerupSpawnDelay;

    private IEnumerator EnemySpawn;
    private IEnumerator SpawnPowerups;
    private bool _bShouldSpawn;

    private void Start()
    {        
        _bShouldSpawn = false;
        EnemySpawn = Enemy_Spawn();
        SpawnPowerups = Spawn_Powerups();
    }

    public void OnLevelStart()
    {
        _bShouldSpawn = true;
        StartCoroutine(EnemySpawn);
        StartCoroutine(SpawnPowerups);
    }
    private IEnumerator Enemy_Spawn()
    {
        while (_bShouldSpawn)
        {
            yield return new WaitForSeconds(Random.Range(_enemySpawnDelay.x, _enemySpawnDelay.y));

            Vector3 randomPos = new Vector3(Random.Range(-8f, 8f), 9f, 0f);
            Instantiate(_enemyPrefab, randomPos, Quaternion.identity, _enemyContainer);            
        }        
    }

    private IEnumerator Spawn_Powerups()
    {
        while (_bShouldSpawn)
        {
            yield return new WaitForSeconds(Random.Range(_powerupSpawnDelay.x, _powerupSpawnDelay.y));

            int randomPowerup = Random.Range(0, _powerups.Length);
            Vector3 randomSpawn = new Vector3(Random.Range(-8f, 8f), 9f, 0f);
            Instantiate(_powerups[randomPowerup], randomSpawn, Quaternion.identity);            
        }        
    }
    public void OnPlayerDeath()
    {
        _bShouldSpawn = false;        
        StopCoroutine(EnemySpawn);
        StopCoroutine(SpawnPowerups);
        ClearEnemies();
        ClearPowerUps();
    }

    private void ClearEnemies()
    {
        GameObject[] enemyPool = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in enemyPool)
        {
            Destroy(obj);
        }
    }

    private void ClearPowerUps()
    {
        GameObject[] powerupPool = GameObject.FindGameObjectsWithTag("Powerup");
        foreach (GameObject obj in powerupPool)
        {
            Destroy(obj);
        }
    }
}
