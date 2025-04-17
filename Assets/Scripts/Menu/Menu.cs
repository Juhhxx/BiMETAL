using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    protected Settings _settings;

    private void Awake()
    {
        _settings = FindFirstObjectByType<Settings>();
    }

    /// <summary>
    /// Opens the Settings menu, these changes will be applied everywhere in all scenes of the game.
    /// </summary>
    public void Settings()
    {
        if ( _settings == null )
            _settings = FindFirstObjectByType<Settings>();
            
        _settings.TurnOnSettings();
    }

    public abstract void Continue();
    public abstract void Quit();
}
