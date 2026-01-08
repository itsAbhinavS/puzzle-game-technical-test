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
        IntialScreenSetup();
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


    private void IntialScreenSetup()
    {
        // Intro screen animation
        introTxt_CG.alpha = 0;
        playBtn_CG.alpha = 0;
        playBtn.gameObject.SetActive(false);
        introTxt_CG.DOFade(1, 3).OnComplete(() =>
        {
            playBtn.gameObject.SetActive(true);
            playBtn_CG.DOFade(1, 1).SetDelay(2f).OnComplete(() => 
            {
                playBtn_CG.DOFade(0.3f, 1f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetLink(playBtn_CG.gameObject, LinkBehaviour.KillOnDestroy);

                playTxt.transform.DOScale(1.05f, 1f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .SetLink(playTxt.gameObject, LinkBehaviour.KillOnDestroy);
            });
        });
    }


    private void StartGame()
    {
        SceneLoader sceneLoader = SceneLoader.Instance;

        if (sceneLoader != null)
        {
            sceneLoader.LoadScene(sceneLoader.GetHomeScene());
        }
        else 
        {
            Debug.LogWarning("SceneLoader missing");
        }
    }
}
