using UnityEngine;

public class RoadTextureScroller : MonoBehaviour
{
    [SerializeField] private Material roadMaterial;
    [SerializeField] private float speed =1f;
    Vector2 offset ;
    void Update()
    {
        offset.y +=speed * Time.deltaTime;
        roadMaterial.SetTextureOffset("_BaseMap",offset);
    }
}
