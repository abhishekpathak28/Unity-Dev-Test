using UnityEngine;
using DG.Tweening;
using System.Collections;
public class MatchingCard : MonoBehaviour
{
    private CardColor cardHolderColor;
    [SerializeField] private ParticleSystem sparkle;
    private int capacity = 6;
    [SerializeField] private Transform[] landPos;
    [SerializeField] private float YoffSet=0.02f;
    private int filled;
    private Vector3 startPos;
    private Vector3 startScale;
    [SerializeField] private MeshRenderer crateRenderer;
    [SerializeField] private Material redMat;
    [SerializeField] private Material blueMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private Material yellowMat;
    private void Start()
    {
        startPos = transform.position;
        startScale = transform.localScale;
        SetRandomColorAndMateril();
    }
    void OnEnable()
    {
        SetRandomColorAndMateril();
    }
    private void SetRandomColorAndMateril()
    {
        cardHolderColor = (CardColor)Random.Range(0, 4);
        if (crateRenderer != null)
        {
            crateRenderer.sharedMaterial = GetMaterial(cardHolderColor);
        }
        Debug.Log($"Matching color: {cardHolderColor}");
    }
    public bool CanFill(Card card)
    {
        // Debug.Log($"filled {filled} vs capacity {capacity}");
        return card.color == cardHolderColor && filled <capacity;
    }
    private void OnTriggerEnter(Collider other)
    {
        Card passingCard = other.GetComponent<Card>();
        if(passingCard!=null && passingCard.OnBelt)
        {
            if (CanFill(passingCard))
            {
                ConveyorController.Instance.RemoveFromBelt(passingCard);
                AcceptCard(passingCard);
            }
        }
    }
    public void AcceptCard(Card card)
    {
        card.OnBelt = false;
        int slotIndex = filled;
        filled++;

        Transform slot = landPos[slotIndex];
        DOTween.Kill(card, true);
        Sequence seq = DOTween.Sequence().SetId(card);

        float flyTime = 0.35f;
        Vector3 originalScale = card.transform.localScale;
        seq.Append(card.transform.DOJump(slot.position, 1.5f, 1, flyTime).SetEase(Ease.OutQuad));
        seq.Join(card.transform.DORotateQuaternion(slot.rotation, flyTime).SetEase(Ease.OutQuad));
        seq.Join(card.transform.DOScale(new Vector3(originalScale.x * 0.8f, originalScale.y, originalScale.z * 1.3f), flyTime / 2).SetEase(Ease.OutSine));
        seq.Insert(flyTime / 2f, card.transform.DOScale(originalScale, flyTime / 2).SetEase(Ease.InSine));
        seq.OnComplete(() =>
        {
            card.transform.SetParent(slot);
            card.transform.localPosition = Vector3.zero;
            card.transform.localRotation = Quaternion.identity;
            transform.DOPunchScale(new Vector3(0.08f, -0.08f, 0.08f), 0.15f, 1, 1f);
            if (filled >= capacity)
            {
                FilledCrate();
            }
        });
    }
    private void FilledCrate()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveY(transform.position.y + 1.5f, 0.5f)).SetEase(Ease.OutCubic);

        seq.JoinCallback(() =>
        {
            if (sparkle) sparkle.Play();
        });
                seq.AppendInterval(0.5f);
        seq.Append(transform.DOScale(0f, 0.25f));

        seq.OnComplete(() =>
        {
            // Destroy(gameObject);
            StartCoroutine(RespawnHolder());
        });
    }
    private IEnumerator RespawnHolder()
    {
        SetRandomColorAndMateril();
        yield return new WaitForSeconds(0.5f);
        foreach (Transform slot in landPos)
        {
            foreach (Transform childCard in slot)
            {
                Destroy(childCard.gameObject); 
            }
        }
        filled = 0;
        transform.position = startPos;
        transform.DOScale(startScale, 0.4f).SetEase(Ease.OutBack);
    }
    private Material GetMaterial(CardColor col)
    {
        return col switch
        {
            CardColor.Red => redMat,
            CardColor.Blue => blueMat,
            CardColor.Green => greenMat,
            CardColor.Yellow => yellowMat,
            _ => redMat
        };
    }
}
