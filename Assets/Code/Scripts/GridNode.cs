using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridNode : MonoBehaviour, IPointerClickHandler
{
    public static event Action OnGridRotate;

    [Space(20)]
    [Header("Grid Settings")]
    [SerializeField] private GridType gridType;
    [SerializeField] private int gridNumber;
    [SerializeField] private GridOrientation orientation;

    private int rotationSteps; // 0, 1, 2, 3


    private void Awake()
    {
        // Initialize rotationSteps based on current Z rotation
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
    }
    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }



    #region Rotation Logic
    public void OnPointerClick(PointerEventData eventData)
    {
        Rotate();
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


    private void CheckIfCanGlow(int gridNum) 
    {
        /*if (this.gridNumber == gridNum && this.orientation == gridOrientation) 
        {
            Debug.Log($"CAN GLOW {gridNumber} : {orientation}");
        }*/
    }

    public int GetGridNumber() 
    {
        return gridNumber;
    }
    public GridOrientation GetGridOrientation()
    {
        return orientation;
    }
}