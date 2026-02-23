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
    private CardColor previousHolderColor;
    private bool isReleasing = false;
    private bool isInitialized = false;
    void Start()
    {
        if (!isInitialized)
        {
            
        RandomizeColor();
        SpwanCards();
        isInitialized = true;
        }
    }
    private void RandomizeColor()
    {
        CardColor previousCardColor = cardColor;
        do
        {
            cardColor = (CardColor)Random.Range(0, 4); 
        } 
        while (cardColor == previousCardColor);

        CardColor newHolderColor;
        do
        {
            newHolderColor = (CardColor)Random.Range(0, 4); 
        } 
        while (newHolderColor == previousHolderColor);

        previousHolderColor = newHolderColor;

        if (holderMeshRenderer != null)
        {
            holderMeshRenderer.sharedMaterial = GetMaterial(newHolderColor);
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
        if (isReleasing || cardsList.Count == 0) return;
        isReleasing = false;
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
            if (GameManager.Instance != null) GameManager.Instance.PlaySFX(GameManager.Instance.cardPopSound);
        }

        cardsList.Clear();
        isReleasing = false;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReplaceEmptyHolder(gameObject, spawnIndex);
        }
    }
    public void RegenrateCards()
    {
        isInitialized = true;
        if(cardsList.Count>0)
        {
            foreach(var card in cardsList) if(card != null) Destroy(card.gameObject);
            cardsList.Clear();
        }
        SpwanCards();
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < cardsList.Count; i++)
        {
            Transform cardTransform = cardsList[i].transform;
            cardTransform.localScale = Vector3.zero;
            seq.Insert(i * 0.04f, cardTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        }
    }

}
