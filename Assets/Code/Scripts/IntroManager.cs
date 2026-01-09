using DG.Tweening;
using UnityEngine;

public class IntroManager : MonoBehaviour
{

    [Space(20)]
    [Header("Intro Screen")]
    [SerializeField] private CanvasGroup introTxt_CG;
    [SerializeField] private LogicButton playBtn;
    [SerializeField] private CanvasGroup playBtn_CG;
    [SerializeField] private Transform playTxt;


    private void Start()
    {
        ApplicationFrameSetup();
        IntialScreenSetup();
        InitialAudioSetup();
    }


    private void OnEnable()
    {
        playBtn.OnClick += StartGame;
    }
    private void OnDisable()
    {
        playBtn.OnClick -= StartGame;
    }
    private void OnDestroy()
    {
        introTxt_CG?.DOKill();
        playBtn_CG?.DOKill();
        playTxt?.DOKill();
    }


    private void ApplicationFrameSetup()
    {
        QualitySettings.vSyncCount = 0;
        int screenRefreshRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
        Application.targetFrameRate = screenRefreshRate;
    }
    private void IntialScreenSetup()
    {
        // Intro screen animation
        introTxt_CG.alpha = 0;
        playBtn_CG.alpha = 0;
        playBtn.gameObject.SetActive(false);
        introTxt_CG.DOFade(1, 2).OnComplete(() =>
        {
            playBtn.gameObject.SetActive(true);
            playBtn_CG.DOFade(0.3f, 1).SetDelay(1f).OnComplete(() => 
            {
                playBtn_CG.DOFade(1f, 1.2f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .SetLink(playBtn_CG.gameObject, LinkBehaviour.KillOnDestroy);

                playTxt.transform.DOScale(1.05f, 1.2f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .SetLink(playTxt.gameObject, LinkBehaviour.KillOnDestroy);
            });
        });
    }
    private void InitialAudioSetup()
    {
        AudioManager.Instance.PlayMenuMusic();
    }


    private void StartGame()
    {
        AudioManager.Instance.PlayButtonClick();
        SceneLoader.Instance.LoadScene(SceneLoader.HOME_SCENE);
    }
}
