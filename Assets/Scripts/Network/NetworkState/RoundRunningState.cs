using NUnit.Framework.Constraints;
using PurrNet.StateMachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class RoundRunningState : StateNode<List<Player>>
{
    [SerializeField] private int playersAlive;

    public override void Enter(List<Player> data, bool asServer)
    {
        base.Enter(data, asServer);

        if (!asServer) return;

        playersAlive = data.Count;

        foreach(var player in data)
        {
            player.OnDeath_Server += OnPlayerDeath;
        }
    }


    public override void Exit(bool asServer)
    {
        base.Exit(asServer);
    }

    private void OnPlayerDeath(Player daedPlayer)
    {
        daedPlayer.OnDeath_Server -= OnPlayerDeath;
        playersAlive--;
        if(playersAlive <= 1)
        {
            Debug.Log("A PLAYER HAVE WON!!!");
            machine.Next();
        }
    }
}
