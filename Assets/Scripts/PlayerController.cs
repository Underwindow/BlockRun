using UnityEngine;
using DG.Tweening;
using System.Linq;

public class PlayerController : MonoBehaviourSingleton<PlayerController>
{
    [SerializeField] private float movingSpeed;
    [SerializeField] private Transform[] points;

    private Vector3[] wayPoints;
    private Sequence sequence;

    private void Start()
    {
        wayPoints = points.ToList().Select(t => t.position).ToArray();

        var duration = GetPathLength(wayPoints) / movingSpeed;

        sequence = DOTween.Sequence();
        sequence.Append(transform
            .DOPath(wayPoints, duration, PathType.Linear, PathMode.Full3D)
            .SetUpdate(UpdateType.Fixed)
            .SetLookAt(0.01f)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                Debug.Log("Player finished!!");
                GameManager.Instance.Finish();
            })
        );

        GameManager.Instance.OnGameOver += () => sequence.Kill();
    }

    private float GetPathLength(Vector3[] wayPoints)
    {
        float pathLength = 0;

        for (int i = wayPoints.Length - 1; i > 0; i--)
            pathLength += (wayPoints[i] - wayPoints[i - 1]).magnitude;

        return pathLength;
    }
}
