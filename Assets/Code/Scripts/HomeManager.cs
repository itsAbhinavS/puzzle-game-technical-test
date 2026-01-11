using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [Space(20)]
    [Header("Canvas")]
    [SerializeField] private RectTransform canvasRect;

    [Space(20)]
    [Header("Default Screen")]
    [SerializeField] private LogicButton settingsBtn;

    [Space(20)]
    [Header("Level Screen")]
    [SerializeField] private GameObject levelScreen;
    [SerializeField] private CanvasGroup levelScreen_CG;
    [SerializeField] private LogicButton[] levelBtn;
    [SerializeField] private GameObject[] levelLockVisual;
    [SerializeField] private GameObject[] levelCompleteVisual;

    [Header("Score Settings")]
    [SerializeField] private TextMeshProUGUI auraScore; 
    public RectTransform auraLayout;

    [Space(20)]
    [Header("Settings Screen")]
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private RectTransform settingsPanel;
    [SerializeField] private LogicButton settingsCloseBtn;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;



    private void Start()
    {
        InitialScreenSetup();
    }



    private void OnEnable()
    {
        // Settings screen
        settingsBtn.OnClick += ShowSettings;
        settingsCloseBtn.OnClick += HideSettings;
        musicSlider.onValueChanged.AddListener(OnMusicSliderChange);
        sfxSlider.onValueChanged.AddListener(OnSFXVolSliderChange);

        // Level screen
        EnableLevelBtn();
    }
    private void OnDisable()
    {
        // Settings screen
        settingsBtn.OnClick -= ShowSettings;
        settingsCloseBtn.OnClick -= HideSettings;
    }



    private void InitialScreenSetup()
    {
        AudioManager.Instance.PlayMenuMusic(0.5f);

        // Level screen
        levelScreen_CG.alpha = 0;
        levelScreen.SetActive(true);
        levelScreen_CG.DOFade(1, 0.5f);

        // Settings screen
        settingsScreen.SetActive(false);
        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();

        // level screen
        for (int i = 1; i <= SaveSystem.TotalLevel; i++)
        {
            levelLockVisual[i - 1].SetActive(!SaveSystem.Instance.IsLevelUnlocked(i));
            levelCompleteVisual[i - 1].SetActive(SaveSystem.Instance.IsLevelCompleted(i));
        }

        // Prepare score
        int score = 0;
        DOTween.To(() => score, x => score = x, SaveSystem.Instance.GetTotalScore(), 1f)
            .OnUpdate(() => {
                
                // Score animation
                auraScore.text = score.ToString();

                // Rebuild layout to fix the horizonatal layout compenent
                LayoutRebuilder.ForceRebuildLayoutImmediate(auraLayout);
            })
            .SetEase(Ease.Linear);

    }



    #region Settings Screen
    private void ShowSettings()
    {
        AudioManager.Instance.PlayButtonClick();

        settingsBtn.gameObject.SetActive(false);

        // Initially the panel will start from down
        settingsPanel.DOKill();
        settingsPanel.anchoredPosition = new Vector2(
            settingsPanel.anchoredPosition.x,
            -canvasRect.rect.height
        );
        settingsScreen.SetActive(true);


        // Close level screen
        levelScreen_CG.alpha = 0f;
        levelScreen.SetActive(false);

        // Panel animation to the center
        settingsPanel.DOAnchorPosY(0f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            settingsCloseBtn.gameObject.SetActive(true);
        });
    }
    private void HideSettings()
    {
        AudioManager.Instance.PlayButtonClick();

        settingsCloseBtn.gameObject.SetActive(false);

        // Panel animation to the center
        settingsPanel.DOKill();
        settingsPanel.DOAnchorPosY(-canvasRect.rect.height, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                // level screen fade animation
                levelScreen.SetActive(true);
                levelScreen_CG.DOKill();
                levelScreen_CG.DOFade(1f, 0.5f).OnComplete(() =>
                {
                    settingsBtn.gameObject.SetActive(true);
                    settingsScreen.SetActive(false);
                });
            });
    }
    private void OnMusicSliderChange(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }
    private void OnSFXVolSliderChange(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }
    #endregion Settings Screen


    #region Level Screen
    private void EnableLevelBtn() 
    {
        for (int i = 0; i < SaveSystem.TotalLevel; i++) 
        {
            int level = i + 1;
            levelBtn[i].OnClick += () => InitializeLevelLoad(level);
        }
    }
    private void InitializeLevelLoad(int selectedLevel)
    {
        if (!SaveSystem.Instance.IsLevelUnlocked(selectedLevel))
        {
            // play deny sound
            AudioManager.Instance.PlayDeniedSound();
            return;
        }

        // play click sound
        AudioManager.Instance.PlayButtonClick();

        Debug.Log($"Current Selected Level: {selectedLevel}");
        SaveSystem.Instance.SetCurrentLevel(selectedLevel);
        SceneLoader.Instance.LoadScene(SceneLoader.LEVEL_SCENE + selectedLevel);
    }
    #endregion Level Screen
}
