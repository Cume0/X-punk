using UnityEngine;

public class Tile : MonoBehaviour //class to change tile properties in inspector
{
    [SerializeField] public bool blockNode = false;
    [SerializeField] public int costToMove = 10;

    [SerializeField] Material whenBlockedMaterial;

    void Awake()
    {
        RecolorNode();
    }
    public void RecolorNode()
    {
        MeshRenderer meshRenderer = transform.Find("Visuals").transform.Find("Base").GetComponent<MeshRenderer>();

        if (blockNode)
        {
            meshRenderer.material = whenBlockedMaterial;
        }
    }
}