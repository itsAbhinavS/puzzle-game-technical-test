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
    [SerializeField] private GameObject[] levelLockVisual;
    [SerializeField] private GameObject[] levelCompleteVisual;

    [Space(20)]
    [Header("Settings Screen")]
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private CanvasGroup settingsBG;
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
        // settings screen
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

        // Background fade animation
        settingsBG.DOKill();
        settingsBG.DOFade(1f, 0.3f);

        // Panel animation to the center
        settingsPanel.DOKill();
        settingsPanel.DOAnchorPosY(0f, 0.5f).SetEase(Ease.OutBack);
    }
    private void HideSettings()
    {
        // Panel animation to the center
        settingsPanel.DOKill();
        settingsPanel.DOAnchorPosY(-canvasRect.rect.height, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                // Background fade animation
                settingsBG.DOKill();
                settingsBG.DOFade(0f, 0.3f).OnComplete(() =>
                {
                    settingsScreen.SetActive(false);
                });
            });
    }
    #endregion Settings

}
