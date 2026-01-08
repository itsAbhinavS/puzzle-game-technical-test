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
    [SerializeField] private GameObject[] levelLockVisual;
    [SerializeField] private GameObject[] levelCompleteVisual;

    [Space(20)]
    [Header("Settings Screen")]
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private RectTransform settingsPanel;
    [SerializeField] private LogicButton settingsCloseBtn;



    private void Start()
    {
        IntialScreenSetup();
    }



    private void OnEnable()
    {
        settingsBtn.OnClick += ShowSettings;
        settingsCloseBtn.OnClick += HideSettings;
    }
    private void OnDisable()
    {
        settingsBtn.OnClick -= ShowSettings;
        settingsCloseBtn.OnClick -= HideSettings;
    }



    private void IntialScreenSetup()
    {
        // Level screen
        levelScreen_CG.alpha = 0;
        levelScreen.SetActive(true);
        levelScreen_CG.DOFade(1, 0.5f);

        // Settings screen
        settingsScreen.SetActive(false);

        // level screen
        for (int i = 1; i <= SaveSystem.Instance.GetTotalLevel(); i++) 
        {
            levelLockVisual[i-1].SetActive(!SaveSystem.Instance.IsLevelUnlocked(i));

            levelCompleteVisual[i-1].SetActive(SaveSystem.Instance.IsLevelCompleted(i));
        }
    }


    #region Settings
    private void ShowSettings() 
    {
        // Initially the panel will start from down
        settingsPanel.anchoredPosition = new Vector2(
            settingsPanel.anchoredPosition.x,
            -canvasRect.rect.height
        );
        settingsScreen.SetActive(true);

        settingsBtn.gameObject.SetActive(false);

        // Close level screen
        levelScreen_CG.alpha = 0f;
        levelScreen.SetActive(false);

        // Panel animation to the center
        settingsPanel.DOKill();
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
                settingsBtn.gameObject.SetActive(true);

                // level screen fade animation
                levelScreen.SetActive(true);
                levelScreen_CG.DOKill();
                levelScreen_CG.DOFade(1f, 0.5f).OnComplete(() =>
                {
                    settingsScreen.SetActive(false);
                });
            });
    }
    #endregion Settings

}
