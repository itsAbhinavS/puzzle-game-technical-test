using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LogicButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    // Any external class can subscribe to OnClick
    public event Action OnClick;

    [Header("Animation Settings")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float downScale = 0.93f;
    [SerializeField] private float UpScale = 1.1f;

    private void OnEnable() => visualRoot.localScale = Vector3.one;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(visualRoot == null) return;

        // Visual scale down animation
        visualRoot.DOKill();
        visualRoot.DOScale(downScale, 0.2f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (visualRoot == null) return;

        // Visual scale up, then to normal scale animation
        visualRoot.DOKill();
        visualRoot.DOScale(UpScale, 0.1f).OnComplete(() =>
        {
            visualRoot.DOScale(1, 0.1f);
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // The delay is added to make other animation complete before executing button function
        DOVirtual.DelayedCall(0.1f, () =>
        {
            OnClick?.Invoke();

        }, ignoreTimeScale: true);
    }
}