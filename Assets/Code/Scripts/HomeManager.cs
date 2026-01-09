using DG.Tweening;
using UnityEngine;

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

    [Space(20)]
    [Header("Settings Screen")]
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private RectTransform settingsPanel;
    [SerializeField] private LogicButton settingsCloseBtn;



    private void Start()
    {
        InitialScreenSetup();
        InitialAudioSetup();
    }



    private void OnEnable()
    {
        settingsBtn.OnClick += ShowSettings;
        settingsCloseBtn.OnClick += HideSettings;
        EnableLevelBtn();
    }
    private void OnDisable()
    {
        settingsBtn.OnClick -= ShowSettings;
        settingsCloseBtn.OnClick -= HideSettings;
    }



    private void InitialScreenSetup()
    {
        // Level screen
        levelScreen_CG.alpha = 0;
        levelScreen.SetActive(true);
        levelScreen_CG.DOFade(1, 0.5f);

        // Settings screen
        settingsScreen.SetActive(false);

        // level screen
        for (int i = 1; i <= SaveSystem.TotalLevel; i++)
        {
            levelLockVisual[i - 1].SetActive(!SaveSystem.Instance.IsLevelUnlocked(i));

            levelCompleteVisual[i - 1].SetActive(SaveSystem.Instance.IsLevelCompleted(i));
        }
    }
    private void InitialAudioSetup() 
    {
        // AudioSystem.Instance.PlayMenuMusic();
    }


    #region Settings Screen
    private void ShowSettings()
    {
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
        Debug.Log($"Current Selected Level: {selectedLevel}");
        SaveSystem.Instance.SetCurrentLevel(selectedLevel);
        SceneLoader.Instance.LoadScene(SceneLoader.LEVEL_SCENE + selectedLevel);
    }
    #endregion Level Screen
}
