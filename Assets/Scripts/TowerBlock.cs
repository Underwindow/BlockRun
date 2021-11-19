using UnityEngine;

public class TowerBlock : MonoBehaviour
{
    [SerializeField] private Material neutralMat;

    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetNeutral()
    {
        meshRenderer.material = neutralMat;
    }
}