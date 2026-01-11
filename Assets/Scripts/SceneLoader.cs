using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Loading UI")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private CanvasGroup loadingScreen;


    // All scenes
    public static readonly string HOME_SCENE = "Home";
    public static readonly string LEVEL_SCENE = "Level";


    private void Awake() 
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Canvas canvas = GetComponent<Canvas>();
        DontDestroyOnLoad(canvas.gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Setup the intial load screen state
        loadingScreen.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(false);
        progressBar.value = 0;

        // Screen fade in animation
        loadingScreen.DOKill();
        yield return loadingScreen.DOFade(1, 0.3f)
            .SetLink(loadingScreen.gameObject)
            .WaitForCompletion();

        progressBar.gameObject.SetActive(true);

        // Loading feature (load paused until animation is over)
        float startTime = Time.time;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Progress animation
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";

            if (asyncLoad.progress >= 0.9f)
            {
                float elapsedTime = Time.time - startTime;
                if (elapsedTime < 1f)
                {
                    yield return new WaitForSeconds(1f - elapsedTime);
                }

                progressBar.value = 1f;
                progressText.text = "100%";
                yield return new WaitForSeconds(0.2f);

                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // Screen fade out animation
        loadingScreen.DOKill();
        if (loadingScreen != null)
        {
            yield return loadingScreen.DOFade(0, 0.3f)
                .SetLink(loadingScreen.gameObject)
                .WaitForCompletion();
            loadingScreen.gameObject.SetActive(false);
        }
    }
}
