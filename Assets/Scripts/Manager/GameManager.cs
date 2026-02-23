using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System;
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

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip cardPopSound;
    public AudioClip crateMatchSound;
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
        
        Transform targetSpot = boardSpawnPoints[spwanIndex];
        Vector3 exitPosition = targetSpot.position + (targetSpot.right * slidingDistance);
        emptyHolder.transform.DOMove(exitPosition, slidingTime).OnComplete(() =>
                {
                    emptyHolder.SetActive(false);
                    holderPool.Enqueue(emptyHolder);
                });
        int groupPos = Array.IndexOf(cascadeGroupIndices, spwanIndex);

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
                    Transform stepTargetSpot = boardSpawnPoints[currentIndex];
                    holderToMove.transform.DOMove(stepTargetSpot.position, 0.1f).SetEase(Ease.OutQuad);
                    holderToMove.transform.DORotateQuaternion(stepTargetSpot.rotation, 0.1f).SetEase(Ease.OutQuad);
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
            Vector3 startPos = targetSpot.position + (targetSpot.right * slidingDistance);
            
            newHolder.transform.position = startPos;
            newHolder.transform.rotation = targetSpot.rotation;
            newHolder.SetActive(true);

            newHolder.transform.DOMove(targetSpot.position, 0.1f).SetEase(Ease.OutBack);

            PopCardsIntoHolder(newHolder);
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
    public void PlaySFX(AudioClip clip)
    {
        
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
