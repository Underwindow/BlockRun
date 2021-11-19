using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public UnityEvent OnPlay;

    public delegate void GameOverEvent();
    public event GameOverEvent OnGameOver;

    public delegate void FinishEvent();
    public event FinishEvent OnFinish;

    private void Awake()
    {
        DOTween.Init(false, false, LogBehaviour.Default);
    }

    public void Play()
    {
        OnPlay?.Invoke();
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void Finish()
    {
        OnFinish?.Invoke();
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        OnPlay?.RemoveAllListeners();
    }
}
