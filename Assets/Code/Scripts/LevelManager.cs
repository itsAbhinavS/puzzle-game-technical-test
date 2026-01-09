using DG.Tweening;
using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static event Action OnLevelPause;
    public static event Action OnLevelResume;

    [Space(20)]
    [Header("Canvas")]
    [SerializeField] private RectTransform canvasRect;

    [Space(20)]
    [Header("Default Screen")]
    [SerializeField] private LogicButton pauseBtn;

    [Space(20)]
    [Header("Pause Screen")]
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private RectTransform pausePanel;
    [SerializeField] private LogicButton homeBtn;
    [SerializeField] private LogicButton retryBtn;
    [SerializeField] private LogicButton pauseCloseBtn;

    private void Start()
    {
        AudioManager.Instance.PlayGameplayMusic();
    }

    private void OnEnable()
    {
        // Pause screen
        pauseBtn.OnClick += PauseLevel;
        pauseCloseBtn.OnClick += ResumeLevel;
        homeBtn.OnClick += GoHome;
        retryBtn.OnClick += ReteyLevel;

        // Game state
        GameLevelLogic.OnLevelComplete += WonGame;
    }

    private void OnDisable()
    {
        // Pause screen
        pauseBtn.OnClick -= PauseLevel;
        pauseCloseBtn.OnClick -= ResumeLevel;
        homeBtn.OnClick -= GoHome;
        retryBtn.OnClick -= ReteyLevel;

        // Game state
        GameLevelLogic.OnLevelComplete -= WonGame;
    }


    #region Game State Function
    private void PauseLevel()
    {
        pauseBtn.gameObject.SetActive(false);

        // Initially the pause panel will start from down
        pausePanel.DOKill();
        pausePanel.anchoredPosition = new Vector2(
            pausePanel.anchoredPosition.x,
            -canvasRect.rect.height
        );
        pauseScreen.SetActive(true);

        OnLevelPause?.Invoke();

        // Pause panel animation to the center
        pausePanel.DOAnchorPosY(0f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            pauseCloseBtn.gameObject.SetActive(true);
        });
    }

    private void ResumeLevel()
    {
        pauseCloseBtn.gameObject.SetActive(false);

        // Panel animation to the center
        pausePanel.DOKill();
        pausePanel.DOAnchorPosY(-canvasRect.rect.height, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                OnLevelResume?.Invoke();
                pauseBtn.gameObject.SetActive(true);
                pauseScreen.SetActive(false);
            });
    }

    private void WonGame() 
    {
        
    }
    private void GoHome()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.HOME_SCENE);
    }
    private void ReteyLevel()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.LEVEL_SCENE + SaveSystem.CurrentLevel);
    }
    #endregion Game State Function
}
