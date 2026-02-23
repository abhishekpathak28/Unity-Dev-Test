using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    [SerializeField] private LayerMask cardHolderLayer;
    private Camera mainCamera;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private GameObject fingerPrefab;
    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector3 inputPos = Pointer.current.position.ReadValue();
            ShowTapFeedBack(inputPos);
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
        if (fingerPrefab == null || uiCanvas == null) return;
        GameObject fingerIcon = Instantiate(fingerPrefab, uiCanvas.transform);

        RectTransform rectTransform = fingerIcon.GetComponent<RectTransform>();
        rectTransform.position = screenPos;

        rectTransform.localScale = Vector3.one * 0.8f;

        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOScale(1.2f, 0.15f).SetEase(Ease.OutBack));
        seq.Join(rectTransform.DOMoveY(screenPos.y + 150f, 0.6f).SetEase(Ease.OutCubic));

        Image fingerImage = fingerIcon.GetComponent<Image>();
        if (fingerImage != null)
        {
            Color startColor = fingerImage.color;
            startColor.a = 1f;
            fingerImage.color = startColor;
            fingerImage.DOFade(0f, 0.5f).SetDelay(0.1f);
        }
        // seq.OnComplete(() =>
        // {
        //     Destroy(fingerIcon);
        // });
    }
}
