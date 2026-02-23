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
    [SerializeField] private double startSplinePercent = 0.0;
    private Dictionary<Card, double> cardDistances = new Dictionary<Card, double>();
    private float splineLength;

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
        }
    }
    public void RegisterCard(Card card)
    {
        // StartCoroutine(EnterBelt(card));
        EnterBelt(card);
    }

        private void EnterBelt(Card card)
    {
        SplineSample startSample = spline.Evaluate(startSplinePercent);
        Vector3 targetPos = startSample.position + (Vector3.up * YoffSet);

        Sequence seq = DOTween.Sequence();
        seq.Append(card.transform.DOJump(targetPos, 5f, 1, 0.6f).SetEase(Ease.OutSine));
        seq.Join(card.transform.DORotate(new Vector3(0, 180, 0), 0.6f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad));
        seq.OnComplete(() =>
        {
            double startDist = spline.CalculateLength(0.0, startSplinePercent);
            cardDistances[card] = startDist;
        });
    }
    void Update()
    {
        if (spline == null || cardDistances.Count == 0) return;
            List<Card> activeCards = new List<Card>(cardDistances.Keys);

        foreach (Card card in activeCards)
        {
            if (!card.OnBelt) continue;
            double dist = cardDistances[card];
            dist -= beltSpeed * Time.deltaTime; 
            if (dist < 0f)
            {
                dist += splineLength;
            }
            dist %= splineLength;

            cardDistances[card] = dist;
            double percent = spline.Travel(0.0, (float)dist);
            SplineSample sample = spline.Evaluate(percent);
            
            Quaternion offset = Quaternion.Euler(0f, 90f, 0f);
            Vector3 finalPosition = sample.position + (Vector3.up * YoffSet);
            
            card.transform.SetPositionAndRotation(finalPosition, sample.rotation * offset);
        }
    }
    public void RemoveFromBelt(Card card)
    {
        if (cardDistances.ContainsKey(card))
        {
            cardDistances.Remove(card);
        }
    }
}
