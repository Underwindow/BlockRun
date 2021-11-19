using UnityEngine;

public class TowerBlock : MonoBehaviour
{
    [SerializeField] Material neutralMat;

    public void SetNeutral()
    {
        GetComponent<MeshRenderer>().material = neutralMat;
    }
}