using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelLogic : MonoBehaviour
{
    public static event Action OnLevelComplete;
    public static event Action<int> OnGridCorrect;

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



    private void Start()
    {
        IntialScreenSetup();
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
                }
                else 
                {
                    break;
                }

                index = index + 1;
            }
        }

        CheckWinSequence();
    }

    private void CheckWinSequence()
    {
        for (int i = 0; i < gridCorrect.Count; i++)
        {
            if (!gridCorrect[i])
                return;
        }

        // Stop game interaction
        gameScreen_CG.blocksRaycasts = false;
        gameScreen_CG.DOFade(0, 2f);

        OnLevelComplete?.Invoke();
    }
    #endregion Grid Manager
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
