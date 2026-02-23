using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameObject filledCardholderPrefab;
    [SerializeField] private GameObject emptyCratePrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] boardSpawnPoints; //11
    [SerializeField] private Transform[] crateSpawnPoints; //4

    [SerializeField] private int totalHoldersToPool = 16;
    [SerializeField] private GameObject[] activeHoldersOnBoard;
    [SerializeField] private int[] cascadeGroupIndices = new int[] {0,1,2};
    [SerializeField] private float slidingDistance = 1f,slidingTime =0.5f;
    private Queue<GameObject> holderPool = new Queue<GameObject>();

   void Awake()
   {
    if(Instance==null){
        Instance=this;
    }
    else{
        Destroy(gameObject);
    }
   }
    void Start()
    {
        activeHoldersOnBoard = new GameObject[boardSpawnPoints.Length];
        SpawnMatchingCrates();
        SpawnFilledCrates();
    }
    private void SpawnMatchingCrates()
    {
        for (int i = 0; i < crateSpawnPoints.Length; i++)
        {
            Instantiate(emptyCratePrefab, crateSpawnPoints[i].position, crateSpawnPoints[i].rotation);
        }
    }
    private void SpawnFilledCrates()
    {
        for(int i = 0; i < totalHoldersToPool; i++)
        {
            GameObject newHolder = Instantiate(filledCardholderPrefab);
            if (i < boardSpawnPoints.Length) 
            {
                newHolder.transform.position = boardSpawnPoints[i].position;
                newHolder.transform.rotation = boardSpawnPoints[i].rotation;
                newHolder.GetComponent<CardHolder>().spawnIndex=i;
                newHolder.SetActive(true);
                activeHoldersOnBoard[i] = newHolder;
            }
            else
            {
                newHolder.SetActive(false);
                holderPool.Enqueue(newHolder);
            }
        }
    }
    public void ReplaceEmptyHolder(GameObject emptyHolder,int spwanIndex)
    {
        emptyHolder.transform.DOMoveZ(emptyHolder.transform.position.z + slidingDistance, slidingTime).OnComplete(()=>
        {
            emptyHolder.SetActive(false);
            holderPool.Enqueue(emptyHolder);
        });
        int groupPos = System.Array.IndexOf(cascadeGroupIndices, spwanIndex);

        if (groupPos != -1)
        {
            for(int i = groupPos; i < cascadeGroupIndices.Length - 1; i++)
            {
                int currentIndex = cascadeGroupIndices[i];
                int nextIndex = cascadeGroupIndices[i + 1];
                GameObject holderToMove = activeHoldersOnBoard[nextIndex];
                if(holderToMove != null)
                {
                    activeHoldersOnBoard[currentIndex] = holderToMove;
                    holderToMove.GetComponent<CardHolder>().spawnIndex = currentIndex;
                    Transform targetSpot = boardSpawnPoints[currentIndex];
                    holderToMove.transform.DOMove(targetSpot.position, 0.3f).SetEase(Ease.OutQuad);
                    holderToMove.transform.DORotateQuaternion(targetSpot.rotation, 0.3f).SetEase(Ease.OutQuad);
                }
            }
            int backOfGroupIndex = cascadeGroupIndices[cascadeGroupIndices.Length - 1];
            SpawnNewHolderAt(backOfGroupIndex);
        }
        else
        {
            SpawnNewHolderAt(spwanIndex);
        }
    }
    private void SpawnNewHolderAt(int targetIndex)
    {
        if (holderPool.Count > 0)
        {
            GameObject newHolder = holderPool.Dequeue();
            activeHoldersOnBoard[targetIndex] = newHolder; 
            newHolder.GetComponent<CardHolder>().spawnIndex = targetIndex;

            Transform targetSpot = boardSpawnPoints[targetIndex];
            newHolder.transform.position = targetSpot.position - new Vector3(0, 0, 2f);
            newHolder.transform.rotation = targetSpot.rotation;
            newHolder.SetActive(true);

            newHolder.transform.DOMove(targetSpot.position, 0.4f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                PopCardsIntoHolder(newHolder);
            });
        }
        else
        {
            Debug.LogWarning("Pool is empty");
        }
    }
    private void PopCardsIntoHolder(GameObject holder)
    {
        CardHolder cardHolderScript = holder.GetComponent<CardHolder>();
        
        if (cardHolderScript != null)
        {
            cardHolderScript.RegenrateCards();
            Debug.Log("New Cards spaned");
        }
    }
}
