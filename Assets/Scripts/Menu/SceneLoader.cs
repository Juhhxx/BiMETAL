using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private const string _loadScene = "LoadScene";
    public static string SceneToLoad { get; private set; }

    // TODO: I would add tabletop - battle persistence here too, round, points, saving is still good,
    // but it would be nice if we didn't have to reload everything all the time, so we just load from existing variables instead of fetching from files

    [SerializeField] private float prePostWaitTime = 0.5f;
    [SerializeField] private float minLoadTime = 1;
    [SerializeField] private Slider _loadSlider;

    public static bool IsLoading { get; private set; } = false;

    public static RestoreFlag CurrentRestoreFlag { get; private set; }

    public static void Load(string scene, RestoreFlag restoreFlag = null )
    {
        Debug.Log("DESTROYED: Loading new");
        if (IsLoading) return;
        IsLoading = true;

        CurrentRestoreFlag = restoreFlag;
        if ( CurrentRestoreFlag != null )
            restoreFlag.IsRestored = false;

        SceneToLoad = scene;
        SceneManager.LoadSceneAsync(_loadScene, LoadSceneMode.Additive);
    }
    
    /// <summary>
    /// When starting up the load scene, then it starts unloading previous scenes and loads the new scene
    /// </summary>
    private IEnumerator Start()
    {
        InputManager.Paused = true;
        // test SceneToLoad ??= "LoadScene";

        // wait prePostWaitTime before trying to load
        yield return new WaitForSeconds(prePostWaitTime);

        AsyncOperation op;

        // unload all scenes except load scene
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != _loadScene)
            {
                Debug.Log("unloading async: " + scene.name);
                op = SceneManager.UnloadSceneAsync(scene.name);
                yield return new WaitWhile(() => !op.isDone);
            }
        }

        //  start counting load time
        float startTime = Time.time;
        // start loading target scene without enabling it
        op = SceneManager.LoadSceneAsync(SceneToLoad, LoadSceneMode.Additive);
        op.allowSceneActivation = false;

        float progress;

        // progress visualization
        Debug.Log("one ");
        while ( !op.isDone )
        {
            // this line remaps 0-0.9 (AsyncOperation.progress returns a value in this range) into a value between 0-1
            progress = Mathf.InverseLerp(0, 0.9f, op.progress);
            _loadSlider.value = Mathf.Lerp(_loadSlider.minValue, _loadSlider.maxValue * 0.8f, progress);
            
            if( progress >= 1)
            {
                Debug.Log("progress activated");
                op.allowSceneActivation = true;
            }
            yield return null;
        }

        // SceneManager.SetActiveScene(SceneManager.GetSceneByName(_loadScene));

        Debug.Log("two ");
        if ( CurrentRestoreFlag != null )
            yield return new WaitUntil( () => CurrentRestoreFlag.IsRestored );

        // give it a min loading time, as loading immediately apparently gives a "pop" effect
        float leftTime = minLoadTime - (Time.time - startTime);
        leftTime = Mathf.Max(0, leftTime);

        float timer = 0;

        while (timer < leftTime)
        {
            timer += Time.deltaTime;
            float finalProgress = Mathf.Clamp01(timer / leftTime);
            _loadSlider.value = Mathf.Lerp(_loadSlider.maxValue * 0.8f, _loadSlider.maxValue, finalProgress);
            Debug.Log("timer? " + timer);
            yield return null;
        }

        // SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneToLoad));

        //unload loading scene
        // onFinishLoad.Invoke();
        yield return new WaitForSeconds(prePostWaitTime);
        
        // why does unload scene async's async object not correctly return completed when using it in a yield return or loop?
        SceneManager.UnloadSceneAsync(_loadScene);

        IsLoading = false;
        CurrentRestoreFlag = null;
        InputManager.Paused = false;
    }
}