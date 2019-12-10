using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Containers : MonoBehaviour
{
    [SerializeField] GameObject _laser;
    [SerializeField] Transform _laserContainer;
    [SerializeField, Space] GameObject _leftWing;
    [SerializeField] GameObject _rightWing;
    [SerializeField] SpriteRenderer _thruster;
    [SerializeField] GameObject _explosion;
    [SerializeField] UI_Manager _uiManager;
    [SerializeField] SpawnManager _spawnManager;

    public GameObject GetLaser() { return _laser; }
    public Transform GetLaserContainer() { return _laserContainer; }
    public GameObject GetLeftWing() { return _leftWing; }
    public GameObject GetRightWing() { return _rightWing; }
    public SpriteRenderer GetThruster() { return _thruster; }
    public GameObject GetExplosion() { return _explosion; }
    public UI_Manager GetUIManager() { return _uiManager; }
    public SpawnManager GetSpawnManager() { return _spawnManager; }
}
