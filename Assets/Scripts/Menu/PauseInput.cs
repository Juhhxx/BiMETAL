using UnityEngine;

public class PauseInput : MonoBehaviour
{
    private void OnEnable()
    {
        InputManager.PauseCount++;
        Debug.Log("Adding pause count: " + InputManager.PauseCount + " at GO: " + gameObject);
    }

    private void OnDisable()
    {
        InputManager.PauseCount--;
        Debug.Log("disable removing pause count: " + InputManager.PauseCount + " at GO: " + gameObject);
    }

    private void OnDestroy()
    {
        if ( gameObject != null && gameObject.activeSelf )
        {
            // InputManager.PauseCount--;
            Debug.Log("destroy removing pause count: " + InputManager.PauseCount + " at GO: " + gameObject);
        }
    }
}
