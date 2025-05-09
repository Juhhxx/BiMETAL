using System.Collections.Generic;
using System.Linq;
using AI.FSMs.UnityIntegration;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaCheats : MonoBehaviour
{
    private ArenaController _arenaController;
    private string _currentScene;

    private void Start()
    {
        _arenaController = FindAnyObjectByType<ArenaController>();

        _currentScene = SceneManager.GetActiveScene().name;
    }
    private void Update()
    {
        Cheats();
    }

    private void Cheats()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            if (Input.GetKeyDown(KeyCode.W)) WinBattle();
            if (Input.GetKeyDown(KeyCode.R)) ResetScene();
            if (Input.GetKeyDown(KeyCode.E)) ResetEnemies();
        }
    }
    private void WinBattle() => _arenaController.WinBattle();
    private void ResetScene() => SceneManager.LoadScene(_currentScene);
    private void ResetEnemies()
    {
        foreach (GameObject e in _arenaController.EnemyPool.EnemiesList)
        {
            e.GetComponent<StateMachineRunner>().Reset();
        }
    }
    
}
