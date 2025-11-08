using UnityEngine;

public class State_Gameplay : IState
{
    public void OnEnter()
    {
        Debug.Log("Entering Gameplay State.");
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
     }
}

