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
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Box collides " + other.name);
        var crate = other.GetComponent<MatchingCard>();
        if(crate == null) {
            Debug.Log("Crate is null");
        return ;
        }
        if (!crate.CanFill(this)) return;
        ConveyorController.Instance.RemoveFromBelt(this);
        crate.AcceptCard(this);
    }
}
