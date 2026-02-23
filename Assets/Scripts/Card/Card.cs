using System.Text.RegularExpressions;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private MeshRenderer cardMeshRenderer;
    public CardColor color {get;private set;}

    public bool OnBelt = true;
    public void Intialize(CardColor col,Material mat)
    {
        color = col;
        cardMeshRenderer.material = mat;
    }
}
