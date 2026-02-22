using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System.Collections;
using DG.Tweening;
using Dreamteck.Splines.Primitives;
public class ConveyorController : MonoBehaviour
{
    public static ConveyorController Instance;
    [SerializeField] private SplineComputer spline;

    [SerializeField] private Transform entryPoint;
    [SerializeField] private float entrySpeed = 5f;

    [SerializeField] private float beltSpeed = 5f;
    [SerializeField] private float slotSpacing = 0.15f;
    [SerializeField] private float YoffSet = 0.15f;

    private readonly List<Card> cardsOnBelt = new();

    private float currentDistance;
    private float splineLength;
    private float startOffsetDistance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (spline != null)
        {
            splineLength =spline.CalculateLength();
            SplineSample sample = spline.Project(entryPoint.position);
            startOffsetDistance = spline.CalculateLength(0.0,sample.percent);
        }
    }
    public void RegisterCard(Card card)
    {
        StartCoroutine(EnterBelt(card));
    }

    private IEnumerator EnterBelt(Card card)
    {
        card.transform.parent = null;
        Vector3 start = card.transform.position;
        Vector3 target = entryPoint.transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * entrySpeed;
            card.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        cardsOnBelt.Add(card);
    }
    void Update()
    {
        if (spline == null || cardsOnBelt.Count == 0) return;
        currentDistance -= beltSpeed * Time.deltaTime;
        if (currentDistance < 0f)
        {
            currentDistance += splineLength;
        }

        currentDistance %= splineLength;
        for(int i = 0; i < cardsOnBelt.Count; i++)
        {
            if(!cardsOnBelt[i].OnBelt) continue;
            float cardDistance= currentDistance +startOffsetDistance + (i*slotSpacing);
            cardDistance %=splineLength;
            double percent = spline.Travel(0.0,cardDistance);
            SplineSample sample = spline.Evaluate(percent);
            Quaternion offset = Quaternion.Euler(0f,90f,0f);
            Vector3 finalPosition = sample.position + (Vector3.up * YoffSet);
            cardsOnBelt[i].transform.SetPositionAndRotation(finalPosition,sample.rotation*offset);
        }
    }
    public void RemoveFromBelt(Card card)
    {
        cardsOnBelt.Remove(card);
    }
}
