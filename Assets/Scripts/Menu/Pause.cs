using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : Menu
{
    [SerializeField] private GameObject _pause;
    [SerializeField] private string _mainMenu;

    private void Start()
    {
        Continue();
    }

    /// <summary>
    /// Runs on late update incase any inputs with the same keys are ran at the same time.
    /// </summary>
    private void LateUpdate()
    {
        if (  SceneManager.GetActiveScene().name != _mainMenu && InputManager.Pause() )
        {
            if ( _pause.activeSelf )
                Continue();
            else
            {
                _pause.SetActive( true );
                Time.timeScale = 0f;
                // InputManager.Paused = true;
            }
        }
        // Debug.Log("time scale?" + Time.timeScale);
    }

    /// <summary>
    /// This button will save the current game’s data and go back to the main menu.
    /// </summary>
    public override void Quit()
    {
        // preform save actions

        StartCoroutine(QuitRoutine());
    }

    private IEnumerator QuitRoutine()
    {
        Continue();

        yield return new WaitUntil( () => ! _pause.activeSelf && ! _settings.GetActive() );

        Debug.Log("loading main");
        SceneLoader.Load(_mainMenu);
    }

    /// <summary>
    /// This button will resume the game from where the player left off.
    /// </summary>
    public override void Continue()
    {
        Time.timeScale = 1f;
        // InputManager.Paused = false;

        _settings.TurnOffSettings();
        _pause.SetActive(false);

        // Debug.Log("unloading");
    }

    /// <summary>
    /// Saves the current progress.
    /// </summary>
    public void Save()
    {

    }

    /// <summary>
    /// Loads the last saved progress.
    /// </summary>
    public void Load()
    {

    }

    public void OnDestroy()
    {
        Debug.Log("Destroying pause");
        Continue();        
    }
}
