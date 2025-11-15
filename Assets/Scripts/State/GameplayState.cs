using UnityEngine;
public class State_Gameplay : IState
{
    public void OnEnter()
    {
        Debug.Log("Entering Gameplay State.");
        Transform ui = UIManager.Instance.GetUI("ui-gameplay");
        ui.gameObject.SetActive(true);
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.GameStateMachine.ChangeState(EGameState.Paused);
        }
    }

    public void OnExit()
    {
        Debug.Log("Exiting Gameplay State.");
        UIManager.Instance.GetUI("ui-gameplay").gameObject.SetActive(false);
    }
}

