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
    private int[] _powerupWeightTable = 
        {
            179, // Ammo = 51.28%
            48, // Triple Shoot  = 13.67%
            42, // Speed boost = 11.96%
            37, // Shield = 10.54%
            30, // Health = 8.54%
            15 // Missile = 4.27%
        }; // total 351

    private int _powerupTotalWeight;

    private void Start()
    {     
        _bShouldSpawn = false;
        EnemySpawn = Enemy_Spawn();
        SpawnPowerups = Spawn_Powerups();
        OnInitPowerUps();
    }

    private void OnInitPowerUps()
    {
        // initalize the weighting for loot spawns
        foreach (int item in _powerupWeightTable){ _powerupTotalWeight += item; }
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
            
            Vector3 randomSpawn = new Vector3(Random.Range(-8f, 8f), 9f, 0f);
            Instantiate(_powerups[WeightPowerUp()], randomSpawn, Quaternion.identity);            
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

    private int WeightPowerUp()
    {
        int randomWeight = Random.Range(0, _powerupTotalWeight);
        Debug.Log("Generated Number: " + randomWeight);
        for (int i = 0; i < _powerupWeightTable.Length; i++)
        {
            if (randomWeight <= _powerupWeightTable[i])
            {

                Debug.Log("Weight Value: " + randomWeight + " : Matching Weight Value: " + _powerupWeightTable[i] + " : Powerup ID: " + i);
                return i;
                
            }

            else
            {
                randomWeight -= _powerupWeightTable[i];
            }
                
        }

        return 0;
    }
   
}
