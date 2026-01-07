using DG.Tweening;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance { get; private set; }

    [SerializeField] private CanvasGroup coverScreen;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowLoadingScreen() 
    {
        coverScreen.DOKill();
        coverScreen.DOFade(1, 0.3f);
    }

    public void HideLoadingScreen()
    {
        coverScreen.DOKill();
        coverScreen.DOFade(0, 0.3f);
    }
}
