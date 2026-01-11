using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Space(20)]
    [Header("Win Screen")]
    [SerializeField] private TextMeshProUGUI levelCompleteTMP;
    [SerializeField] private ParticleSystem winParticle;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private RectTransform winPanel;
    [SerializeField] private LogicButton winHomeBtn;
    [SerializeField] private LogicButton nextBtn;
    [Header("Score Settings")]
    [SerializeField] private TextMeshProUGUI auraScore;
    public RectTransform auraLayout;


    private void Start()
    {
        AudioManager.Instance.PlayGameplayMusic();

        // Initial screen setup
        pauseBtn.gameObject.SetActive(true);
        pauseScreen.SetActive(false);
        winScreen.SetActive(false);
    }

    private void OnEnable()
    {
        // Pause screen
        pauseBtn.OnClick += PauseLevel;
        pauseCloseBtn.OnClick += ResumeLevel;
        homeBtn.OnClick += GoHome;
        retryBtn.OnClick += ReteyLevel;

        // Win screen
        winHomeBtn.OnClick += GoHome;
        nextBtn.OnClick += NextLevel;

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

        // Win screen
        winHomeBtn.OnClick -= GoHome;
        nextBtn.OnClick -= NextLevel;

        // Game state
        GameLevelLogic.OnLevelComplete -= WonGame;
    }


    #region Game State Function
    private void PauseLevel()
    {
        // play click sound
        AudioManager.Instance.PlayButtonClick();

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
        pausePanel.DOAnchorPosY(-100f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            pauseCloseBtn.gameObject.SetActive(true);
        });
    }

    private void ResumeLevel()
    {
        // play click sound
        AudioManager.Instance.PlayButtonClick();

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
        AudioManager.Instance.PlayWinMusic(0.1f);
        levelCompleteTMP.text = $"Level {SaveSystem.CurrentLevel} Completed";

        SaveScoreLogic();

        // Win Panel show animation

        pauseBtn.gameObject.SetActive(false);

        DOVirtual.DelayedCall(3f, () =>
        {
            // Initially the pause panel will start from down
            winPanel.anchoredPosition = new Vector2(pausePanel.anchoredPosition.x, -canvasRect.rect.height);

            winScreen.SetActive(true);

            // Pause panel animation to the center
            winPanel.DOAnchorPosY(-100f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                // Score animation
                int score = 0;
                int finalScore = SaveSystem.Instance.GetLevelScore(SaveSystem.CurrentLevel);

                DOTween.To(() => score, x => score = x, finalScore, 1f)
                    .OnUpdate(() =>
                    {
                        auraScore.text = score.ToString();
                    })
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        LayoutRebuilder.ForceRebuildLayoutImmediate(auraLayout);
                    });
            });
        });


        DOVirtual.DelayedCall(1f, () =>
        {
            winParticle.Play();
        });
    }
    private void GoHome()
    {
        // play click sound
        AudioManager.Instance.PlayButtonClick();

        SceneLoader.Instance.LoadScene(SceneLoader.HOME_SCENE);
    }
    private void ReteyLevel()
    {
        // play click sound
        AudioManager.Instance.PlayButtonClick();

        SceneLoader.Instance.LoadScene(SceneLoader.LEVEL_SCENE + SaveSystem.CurrentLevel);
    }
    private void NextLevel()
    {
        // play click sound
        AudioManager.Instance.PlayButtonClick();

        if (SaveSystem.CurrentLevel == SaveSystem.TotalLevel)
        {
            GoHome();
        }
        else 
        {
            int newLevel = SaveSystem.CurrentLevel + 1;
            SaveSystem.Instance.SetCurrentLevel(newLevel);
            SceneLoader.Instance.LoadScene(SceneLoader.LEVEL_SCENE + SaveSystem.CurrentLevel);
        }
    }
    #endregion Game State Function


    #region Logic Function
    private void SaveScoreLogic()
    {
        if (SaveSystem.Instance.IsLevelCompleted(SaveSystem.CurrentLevel))
        {
            if (SaveSystem.LevelScore > SaveSystem.Instance.GetLevelScore(SaveSystem.CurrentLevel))
            {
                SaveSystem.Instance.CompletedLevel(SaveSystem.CurrentLevel, SaveSystem.LevelScore);
            }
        }
        else
        {
            SaveSystem.Instance.CompletedLevel(SaveSystem.CurrentLevel, SaveSystem.LevelScore);
        }
    }
    #endregion Logic Function
}
