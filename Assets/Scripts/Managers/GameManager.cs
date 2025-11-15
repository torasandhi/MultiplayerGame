using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public State_Machine GameStateMachine { get; private set; }

    public static event Action<EGameState> OnGameStateChanged;
    private bool IsPaused = false;

    protected override void Awake()
    {
        base.Awake();
        GameStateMachine = new State_Machine();

        if (PlayerController.Instance != null)
        {
            OnGameStateChanged += PlayerController.Instance.InputActivate;
        }
        else
        {
            Debug.LogError("GameManager couldn't find PlayerController.Instance on Awake!");
        }

    }

    private void Start()
    {
        // Register all states with the state machine
        GameStateMachine.RegisterState(EGameState.MainMenu, new State_MainMenu());
        GameStateMachine.RegisterState(EGameState.Gameplay, new State_Gameplay());
        GameStateMachine.RegisterState(EGameState.Paused, new State_Pause());
        GameStateMachine.RegisterState(EGameState.Loading, new State_Loading());
        GameStateMachine.RegisterState(EGameState.GameOver, new State_GameOver());

        //Initialize Essential UI
        UIManager.Instance.SpawnUIByString("ui-mainmenu");
        UIManager.Instance.SpawnUIByString("ui-gameplay");
        UIManager.Instance.SpawnUIByString("ui-loading");
        UIManager.Instance.SpawnUIByString("ui-pause");

        UIManager.Instance.GetUI("ui-mainmenu").gameObject.SetActive(false);
        UIManager.Instance.GetUI("ui-gameplay").gameObject.SetActive(false);
        UIManager.Instance.GetUI("ui-loading").gameObject.SetActive(false);
        UIManager.Instance.GetUI("ui-pause").gameObject.SetActive(false);

        // Start the game to Authenticate
        //SceneManager.Instance.LoadLevel("MainMenu", EGameState.MainMenu);
        GameStateMachine.ChangeState(EGameState.Gameplay);
    }

    private void Update()
    {
        GameStateMachine.Update();
    }

    public void TriggerGameStateChange(EGameState newState)
    {
        OnGameStateChanged?.Invoke(newState);
    }

    public void PauseGame()
    {
        IsPaused = true;

        // 1. Freeze game time
        //Time.timeScale = 0f;

        // 2. Pause all audio
        //AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        IsPaused = false;

        // 1. Restore normal game time
        //Time.timeScale = 1f;

        // 2. Resume all audio
        //AudioListener.pause = false;
    }
}
