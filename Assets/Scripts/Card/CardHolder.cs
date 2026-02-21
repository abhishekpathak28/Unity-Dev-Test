using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CardHolder : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private CardColor holderColor;

    [Header("Materials")]
    [SerializeField] private Material redMat;
    [SerializeField] private Material blueMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private Material yellowMat;

    private List<Card> cardsList = new();
    void Start()
    {
        SpwanCards();
    }
    private void SpwanCards()
    {
        Material mat = GetMaterial(holderColor);
        for(int i=0;i<spawnPoints.Length;i++){
            Card card = Instantiate(cardPrefab,spawnPoints[i].position,spawnPoints[i].rotation,transform);
            card.Intialize(holderColor,mat);
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

    private void OnMouseDown()
    {
        ReleaseCards();
        Debug.Log("Holder tapped");
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
            // card.transform.position+=transform.forward*2f;
            ConveyorController.Instance.RegisterCard(card);
            yield return new WaitForSeconds(0.15f);
            Debug.Log("Releasing " + card.color);
        }
    }
}
