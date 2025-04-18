using UnityEngine;

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
        if ( InputManager.Pause() )
        {
            if ( _pause.activeSelf )
                Continue();
            else
            {
                _pause.SetActive( true );
                Time.timeScale = 0f;
                InputManager.Paused = true;
            }
        }
    }

    /// <summary>
    /// This button will save the current game’s data and go back to the main menu.
    /// </summary>
    public override void Quit()
    {
        // preform save actions

        SceneLoader.Load(_mainMenu);
    }

    /// <summary>
    /// This button will resume the game from where the player left off.
    /// </summary>
    public override void Continue()
    {
        _settings.TurnOffSettings();
        _pause.SetActive(false);
        Time.timeScale = 1f;
        InputManager.Paused = false;
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
        Continue();        
    }
}
