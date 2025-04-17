using UnityEngine;

public class Main : Menu
{
    [SerializeField] private string _overworld;

    /// <summary>
    /// Loads the previous game files and starts the game off from where the player left off.
    /// </summary>
    public override void Continue()
    {
        // load previous save
    }

    /// <summary>
    /// Saves any progress and closes the game window.
    /// </summary>
    public override void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Clears the game files and loads a fresh game.
    /// </summary>
    public void NewGame()
    {
        SceneLoader.Load(_overworld);
    }
}
