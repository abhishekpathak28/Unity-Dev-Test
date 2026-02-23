using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    [SerializeField] private LayerMask cardHolderLayer;
    private Camera mainCamera;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private Image fingerImage;
    private RectTransform fingerRect;
    void Start()
    {
        mainCamera = Camera.main;
        {
            fingerRect = fingerImage.GetComponent<RectTransform>();
            Color startColor = fingerImage.color;
            startColor.a = 0f;
            fingerImage.color = startColor;
        }
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector3 inputPos = Pointer.current.position.ReadValue();
            // ShowTapFeedBack(inputPos);
            Ray ray = mainCamera.ScreenPointToRay(inputPos);
            if(Physics.Raycast(ray,out RaycastHit hit, 100f, cardHolderLayer))
            {
                CardHolder tappedHolder = hit.collider.GetComponentInParent<CardHolder>();
                if (tappedHolder != null)
                {
                    tappedHolder.ReleaseCards();
                }
            }
        }
    }
    private void ShowTapFeedBack(Vector2 screenPos)
    {
        if (fingerRect == null || fingerImage == null) return;

        DOTween.Kill(fingerRect);
        DOTween.Kill(fingerImage);

        fingerRect.position = screenPos;

        fingerRect.localScale = Vector3.one * 0.8f;
        Color resetColor = fingerImage.color;
        resetColor.a = 1f;
        fingerImage.color = resetColor;

        Sequence seq = DOTween.Sequence();
        seq.SetTarget(fingerRect); 
        seq.Append(fingerRect.DOScale(1.2f, 0.15f).SetEase(Ease.OutBack));
        // seq.Join(fingerRect.DOMoveY(screenPos.y + 150f, 0.6f).SetEase(Ease.OutCubic));

        fingerImage.DOFade(0f, 0.5f).SetDelay(0.1f);
    }
}
