using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System.Collections;
using DG.Tweening;
public class ConveyorController : MonoBehaviour
{
    public static ConveyorController Instance;
    [SerializeField] private SplineComputer spline;

    [SerializeField] private Transform entryPoint;
    [SerializeField] private float entrySpeed = 5f;
    [SerializeField] private float beltSpeed = 0.15f;
    [SerializeField] private float slotSpacing = 0.05f;

    private readonly List<Card> cardsOnBelt = new();

    private float beltPercent;

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
     public void RegisterCard(Card card)
    {
        StartCoroutine(EnterBelt(card));
    }

    private IEnumerator EnterBelt(Card card)
    {
        card.transform.parent = null;

        Vector3 start = card.transform.position;
        Vector3 target = entryPoint.position;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * entrySpeed;
            card.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        cardsOnBelt.Add(card);
    }
     private void Update()
    {
        if (spline == null || cardsOnBelt.Count == 0) return;

        beltPercent += beltSpeed * Time.deltaTime;
        beltPercent %= 1f;

        for (int i = 0; i < cardsOnBelt.Count; i++)
        {
            if (!cardsOnBelt[i].OnBelt) continue;
            float p = beltPercent + i * slotSpacing;
            p %= 1f;

            SplineSample sample = spline.Evaluate(p);

            cardsOnBelt[i].transform.SetPositionAndRotation(
                sample.position,
                sample.rotation
            );
        }
    }
    public void RemoveFromBelt(Card card)
    {
        cardsOnBelt.Remove(card);
    }
}
