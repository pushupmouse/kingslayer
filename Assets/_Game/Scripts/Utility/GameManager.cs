using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private SceneAsset _gameScene;
    [SerializeField] private SceneAsset _boostCenterScene;

    public void GoToBoostCenter()
    {
        SceneManager.LoadScene(_boostCenterScene.name);
    }

    public void GoToNextRound()
    {
        SceneManager.LoadScene(_gameScene.name);
        
        RoundTimer.Instance.EnterNextRound();
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
    }
}
