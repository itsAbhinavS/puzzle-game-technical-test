using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameLevelLogic : MonoBehaviour
{
    public static event Action OnLevelComplete;
    public static event Action<List<bool>, List<int>> OnGridCorrect;

    [Space(20)]
    [Header("Level Screen")]
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private CanvasGroup gameScreen_CG;

    [Space(20)]
    [Header("Grid Sequence")]
    [SerializeField] private List<GridNode> gridItem;

    [Space(20)]
    [Header("Win Sequence")]
    [SerializeField] private List<GridSequence> gridSequence;
    [SerializeField] private List<bool> gridCorrect;
    [SerializeField] private List<int> gridNumbers;

    [Header("Score Settings")]
    [SerializeField] private int baseScore = 1000;
    [SerializeField] private int rotationPenalty = 10;
    [SerializeField] private float timeLimit = 100f; 
    private int currentScore = 0;
    private float startTime = 0;
    private float elapsedTime = 0;

    private void Awake()
    {
        DOTween.Init(true, true, LogBehaviour.ErrorsOnly).SetCapacity(500, 50);
    }

    private void Start()
    {
        IntialScreenSetup();
        InitialScoreSetup();
    }


    private void OnEnable()
    {
        LevelManager.OnLevelPause += HideGame;
        LevelManager.OnLevelResume += ShowGame;

        GridNode.OnGridRotate += CheckGridCorrect;
    }
    private void OnDisable()
    {
        LevelManager.OnLevelPause -= HideGame;
        LevelManager.OnLevelResume -= ShowGame;

        GridNode.OnGridRotate -= CheckGridCorrect;
    }


    #region UI Setup
    private void IntialScreenSetup()
    {
        // Level screen
        gameScreen_CG.alpha = 0;
        gameScreen.SetActive(true);
        gameScreen_CG.DOFade(1, 0.5f);

        // 0 score intiially
        SaveSystem.Instance.SetCurrentLevelScore(0);

        // This is runned to simply initialize the grid glow effect
        gridCorrect[0] = true;
        gridNumbers[0] = gridSequence[0].Sequence[0].GridNumber;
        OnGridCorrect?.Invoke(gridCorrect, gridNumbers);
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
    #endregion UI Setup


    #region Grid Manager
    private void CheckGridCorrect()
    {
        for (int i = 0; i < gridCorrect.Count; i++)
        {
            gridCorrect[i] = false;
        }

        gridCorrect[0] = true;

        foreach (GridSequence gridSeq in gridSequence)
        {
            int index = 0;

            // Go through each WinSequence in the current GridSequence
            foreach (WinSequence winSeq in gridSeq.Sequence)
            {
                if (winSeq.GridNumber == gridItem[index].GetGridNumber() &&
                    winSeq.Orientation == gridItem[index].GetGridOrientation())
                {
                    gridCorrect[index] = true;
                    gridNumbers[index] = winSeq.GridNumber;
                }
                else 
                {
                    break;
                }

                index = index + 1;
            }
        }

        OnGridCorrect?.Invoke(gridCorrect, gridNumbers);

        SetScore();

        CheckWinSequence();
    }
    private void CheckWinSequence()
    {
        for (int i = 0; i < gridCorrect.Count; i++)
        {
            if (!gridCorrect[i])
                return;
        }

        // Win sequence
        gameScreen_CG.blocksRaycasts = false;
        CalculateFinalScore();
        ShakeGameScreen();
        GameFinishState();
    }
    private void GameFinishState()
    {
        // Publish/raise delegates to let other know
        OnLevelComplete?.Invoke();

        // Slowly fade away animation after a delay
        DOVirtual.DelayedCall(2f, () =>
        {
            gameScreen_CG.DOFade(0, 0.5f);
        });
    }
    #endregion Grid Manager


    #region Score System
    private void InitialScoreSetup()
    {
        currentScore = baseScore;
        startTime = Time.time;
        elapsedTime = 0;
    }
    private void SetScore()
    {
        currentScore = Mathf.Max(0, currentScore - rotationPenalty);
    }
    private void CalculateFinalScore()
    {
        elapsedTime = Time.time - startTime;
        
        float remainingTime = Mathf.Max(0, timeLimit - elapsedTime);

        int tenthsRemaining = Mathf.FloorToInt(remainingTime * 10f);

        int timeScore = tenthsRemaining * 2;

        currentScore += timeScore;

        SaveSystem.Instance.SetCurrentLevelScore(currentScore);
    }
    #endregion Score System


    private void ShakeGameScreen()
    {
        gameScreen.transform.DOShakePosition(
            duration: 4f,
            strength: new Vector3(10f, 10f, 0f), // Only X and Y
            vibrato: 50,
            randomness: 90,
            snapping: false,
            fadeOut: true
        ).SetDelay(0.5f);
    }
}



#region Grid Win Sequence
[System.Serializable]
public class GridSequence
{
    public List<WinSequence> Sequence;
}

[System.Serializable]
public class WinSequence
{
    public int GridNumber;
    public GridOrientation Orientation;
}
#endregion Grid Win Sequence


public enum GridOrientation
{
    Up,
    Right,
    Down,
    Left
}

public enum GridType
{
    PointLine,
    Line,
    Curve,
}
