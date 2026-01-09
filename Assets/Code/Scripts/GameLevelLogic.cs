using DG.Tweening;
using System;
using UnityEngine;

public class GameLevelLogic : MonoBehaviour
{
    public static event Action OnLevelComplete;

    [Space(20)]
    [Header("Level Screen")]
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private CanvasGroup gameScreen_CG;


    private void Start()
    {
        IntialScreenSetup();
    }


    private void OnEnable()
    {
        LevelManager.OnLevelPause += HideGame;
        LevelManager.OnLevelResume += ShowGame;
    }
    private void OnDisable()
    {
        LevelManager.OnLevelPause -= HideGame;
        LevelManager.OnLevelResume -= ShowGame;
    }


    private void IntialScreenSetup()
    {
        // Level screen
        gameScreen_CG.alpha = 0;
        gameScreen.SetActive(true);
        gameScreen_CG.DOFade(1, 0.5f);
    }


    public void ShowGame()
    {
        gameScreen.SetActive(true);
        gameScreen_CG.DOKill();
        gameScreen_CG.DOFade(1f, 0.5f);
    }
    public void HideGame()
    {
        // Game screen hide animation
        gameScreen_CG.alpha = 0f;
        gameScreen.SetActive(false);
    }
}
