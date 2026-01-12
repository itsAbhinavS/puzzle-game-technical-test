using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridNode : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnGridRotate;

    [Space(20)]
    [Header("Grid Settings")]
    [SerializeField] private GridType gridType;
    [SerializeField] private int gridNumber;
    [SerializeField] private GridOrientation orientation;

    [Space(20)]
    [Header("Grid Settings")]
    [SerializeField] private List<Image> gridImg;
    private string glowHex = "#FFFFFF";
    private string noGlowHex = "#4C4C4C";
    private Color noGlowColor;
    private Color glowColor;

    private int rotationSteps; // 0, 1, 2, 3


    private void Awake()
    {
        IntializeGlowColors();
        InitializeRotationFromTransform();
        UpdateOrientation();
    }


    private void OnEnable()
    {
        GameLevelLogic.OnGridCorrect += CheckIfCanGlow;
    }
    private void OnDisable()
    {
        GameLevelLogic.OnGridCorrect -= CheckIfCanGlow;
    }


    #region Orientation Logic
    private void InitializeRotationFromTransform()
    {
        // Get current Z rotation
        float currentZ = transform.localEulerAngles.z;

        // Normalize angle to 0-360 range
        currentZ = NormalizeAngle(currentZ);

        // Convert to rotation steps (0, 1, 2, 3)
        // Since we use negative rotation: -0, -90, -180, -270
        // Which Unity stores as: 0, 270, 180, 90

        if (Mathf.Abs(currentZ - 0f) < 5f)
        {
            rotationSteps = 0; // 0° = Up
        }
        else if (Mathf.Abs(currentZ - 270f) < 5f)
        {
            rotationSteps = 1; // -90° (270°) = Right
        }
        else if (Mathf.Abs(currentZ - 180f) < 5f)
        {
            rotationSteps = 2; // -180° (180°) = Down
        }
        else if (Mathf.Abs(currentZ - 90f) < 5f)
        {
            rotationSteps = 3; // -270° (90°) = Left
        }
        else
        {
            rotationSteps = Mathf.RoundToInt(currentZ / 90f) % 4;
        }

        Rotate();
    }
    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }
    #endregion Orientation Logic


    #region Rotation Logic
    public void OnPointerClick(PointerEventData eventData)
    {
        Rotate();

        // play sound
        AudioManager.Instance.PlaySwitchSound();
    }
    public void Rotate()
    {
        if (gridType == GridType.Line)
        {
            // Toggle between 0 (Up/Vertical) and 1 (Right/Horizontal)
            rotationSteps = rotationSteps == 0 ? 1 : 0;
        }
        else
        {
            // Normal 4-way rotation for other types
            rotationSteps = (rotationSteps + 1) % 4;
        }

        float targetRotation = rotationSteps * 90f;

        // Rotation animation on this grid
        transform.DOKill();
        transform.DORotate(new Vector3(0, 0, -targetRotation), 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => 
            {
                UpdateOrientation();
                OnGridRotate?.Invoke();
            });
    }
    private void UpdateOrientation()
    {
        GridOrientation rawOrientation = (GridOrientation)rotationSteps;

        // For Line, normalize to only Up or Right
        if (gridType == GridType.Line)
        {
            orientation = (rotationSteps == 0 || rotationSteps == 2)
                ? GridOrientation.Up
                : GridOrientation.Right;
        }
        else
        {
            orientation = rawOrientation;
        }
    }
    #endregion Rotation Logic


    #region Glow Logic
    private void IntializeGlowColors()
    {
        if (!ColorUtility.TryParseHtmlString(noGlowHex, out noGlowColor))
        {
            Debug.LogError("Invalid noGlowHex string format!");
        }
        if (!ColorUtility.TryParseHtmlString(glowHex, out glowColor))
        {
            Debug.LogError("Invalid glowHex string format!");
        }
    }
    private void CheckIfCanGlow(List<bool> gridCorrect, List<int> gridNumber) 
    {
        for (int i = 0; i < gridCorrect.Count; i++)
        {
            if (gridCorrect[i] == true && gridNumber[i] == this.gridNumber) 
            {
                SetAllToGlowColor();
                return;
            }
        }

        SetAllToNoGlowColor();
    }
    public void SetAllToGlowColor()
    {
        foreach (Image img in gridImg)
        {
            img.color = glowColor;
        }
    }

    public void SetAllToNoGlowColor()
    {
        foreach (Image img in gridImg)
        {
            img.color = noGlowColor;
        }
    }
    #endregion Glow Logic


    #region Get Functions
    public int GetGridNumber() 
    {
        return gridNumber;
    }
    public GridOrientation GetGridOrientation()
    {
        return orientation;
    }
    #endregion Get Functions
}