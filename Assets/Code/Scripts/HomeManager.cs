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
        settingsScreen.SetActive(false);
    }




    #region Settings logic
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
        // Background fade animation
        settingsBG.DOKill();
        settingsBG.DOFade(0f, 0.3f);

        // Panel animation to the center
        settingsPanel.DOKill();
        settingsPanel.DOAnchorPosY(-canvasRect.rect.height, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                settingsScreen.SetActive(false);
            });
    }
    #endregion Settings logic

}
