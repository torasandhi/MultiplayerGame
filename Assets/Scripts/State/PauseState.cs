using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.UI;

public class State_Pause : IState
{
    private Button btn_resume;
    private Button btn_exit;

    public void OnEnter()
    {
        Debug.Log("Entering Pause State.");
        
        Transform parent = UIManager.Instance.GetUI("ui-pause");
        parent.gameObject.SetActive(true);

        btn_resume = parent.Find("background")
                 .transform.Find("button-container")
                 .transform.Find("btn-resume").GetComponent<Button>();
        btn_resume.onClick.AddListener(Resume);


        btn_exit = parent.Find("background")
                 .transform.Find("button-container")
                 .transform.Find("btn-exit").GetComponent<Button>();
        btn_exit.onClick.AddListener(Exit);

        GameManager.Instance.PauseGame();
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Resume();
        }
    }

    public void OnExit()
    {
        Debug.Log("Exiting Pause State.");

        if (btn_resume != null)
            btn_resume.onClick.RemoveAllListeners();

        if (btn_exit != null)
            btn_exit.onClick.RemoveAllListeners();

        GameManager.Instance.ResumeGame();
        UIManager.Instance.GetUI("ui-pause").gameObject.SetActive(false);
    }

    private void Resume()
    {
        GameManager.Instance.GameStateMachine.ChangeState(EGameState.Gameplay);
    }

    private void Exit()
    {
        SceneManager.Instance.LoadLevel("MainMenu", EGameState.MainMenu);
    }
}