using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using DG.Tweening;
public class CardHolder : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform[] spawnPoints;
    private CardColor cardColor;

    [Header("Materials")]
    [SerializeField] private Material redMat;
    [SerializeField] private Material blueMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private Material yellowMat;

    [SerializeField] private MeshRenderer holderMeshRenderer;

    private List<Card> cardsList = new();
    public int spawnIndex;
    void Start()
    {
        RandomizeColor();
        SpwanCards();
    }
    private void RandomizeColor()
    {
        cardColor = (CardColor)Random.Range(0, 4); 
        CardColor holderColor = (CardColor)Random.Range(0, 4); 
        if (holderMeshRenderer != null)
        {
            holderMeshRenderer.material = GetMaterial(holderColor);
        }
    }
    private void SpwanCards()
    {
        Material mat = GetMaterial(cardColor);
        for(int i=0;i<spawnPoints.Length;i++){
            Card card = Instantiate(cardPrefab,spawnPoints[i].position,spawnPoints[i].rotation,transform);
            card.Intialize(cardColor,mat);
            cardsList.Add(card);
        }
    }
    private Material GetMaterial(CardColor col)
    {
        return col switch
        {
            CardColor.Red => redMat,
            CardColor.Blue => blueMat,
            CardColor.Green => greenMat,
            CardColor.Yellow => yellowMat,
            _=>redMat
        };
    }
    public void ReleaseCards()
    {
        StartCoroutine(ReleaseCoRO());
    }
    private IEnumerator ReleaseCoRO()
    {
        foreach (var card in cardsList)
        {
            card.transform.parent=null;
            ConveyorController.Instance.RegisterCard(card);
            yield return new WaitForSeconds(0.15f);
            // Debug.Log("Releasing " + card.color);
        }
        cardsList.Clear();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReplaceEmptyHolder(gameObject, spawnIndex);
        }
    }
    public void RegenrateCards()
    {
        if(cardsList.Count == 0)
        {
            SpwanCards();
        }
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < cardsList.Count; i++)
        {
            Transform cardTransform = cardsList[i].transform;
            cardTransform.localScale = Vector3.zero;
            seq.Insert(i * 0.1f, cardTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
        }
    }

}
