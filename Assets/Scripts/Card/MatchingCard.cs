using UnityEngine;
using DG.Tweening;
public class MatchingCard : MonoBehaviour
{
    [SerializeField] private CardColor cardHolderColor;
    [SerializeField] private ParticleSystem sparkle;
    private int capacity = 6;
    [SerializeField] private Transform[] landPos;
    [SerializeField] private float YoffSet=0.02f;
    private int filled;

    public bool CanFill(Card card)
    {
        // Debug.Log($"filled {filled} vs capacity {capacity}");
        return card.color == cardHolderColor && filled <capacity;
    }
    public void AcceptCard(Card card)
    {
        card.follower.enabled = false;
        card.OnBelt = false;
        int slotIndex = filled;
        filled++;

        Transform slot = landPos[slotIndex];
        DOTween.Kill(card, true);
        Sequence seq = DOTween.Sequence().SetId(card);

        seq.AppendInterval(0.1f);

        seq.Append(
            card.transform
                .DOJump(slot.position, 0.8f, 1, 0.32f)
                .SetEase(Ease.OutQuad)
                .SetId(card)
        );

        seq.Join(
            card.transform
                .DORotateQuaternion(slot.rotation, 0.32f)
                .SetId(card)
        );

        seq.OnComplete(() =>
        {
            card.transform.SetParent(slot);
            card.transform.localPosition = Vector3.zero;
            card.transform.localRotation = Quaternion.identity;

            if (filled >= capacity)
                FilledCrate();
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
            Destroy(gameObject);
        });
    }
}
