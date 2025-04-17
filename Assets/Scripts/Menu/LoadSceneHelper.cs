using UnityEngine;

public class LoadSceneHelper : MonoBehaviour
{
    [SerializeField] private string _scene;
    public void LoadScene()
    {
        // SceneManager.LoadScene(_loadScene);
        SceneLoader.Load(_scene);
    }
}