using PurrNet.StateMachine;
using Unity.Services.Matchmaker.Models;
using UnityEditor.Rendering;
using UnityEngine;

public class RoundEndState : StateNode
{
    [SerializeField] private int amountOfRounds = 3;
    [SerializeField] private StateNode spawningState;
    [SerializeField] private StateNode gameEndState;

    private int round = 1; 
    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

        if (!asServer) return;

        round++;
        machine.SetState(spawningState);

        //if(_roundCount >= amountOfRounds)
        //    machine.SetState(gameEndState);
    }

    public override void Exit(bool asServer)
    {
        base.Exit(asServer);
    }
}
