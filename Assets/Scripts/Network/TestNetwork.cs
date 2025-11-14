using NUnit.Framework.Internal.Commands;
using PurrNet;
using System;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class TestNetwork : NetworkBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private List<PlayerID> _players = new();
    [SerializeField] private int _targetedPlayer = 1;

    protected override void OnSpawned()
    {
        base.OnSpawned();
    }

    protected override void OnObserverAdded(PlayerID player)
    {
        base.OnObserverAdded(player);

        _players.Add(player);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            SetPauseStateServer();
    }

    [TargetRpc]
    private void SetPauseStateTarget(PlayerID id)
    {

        GameManager.Instance.GameStateMachine.ChangeState(EGameState.Paused);
    }

    [ServerRpc(requireOwnership: false)]
    private void SetPauseStateServer()
    {
        SetPauseStateTarget(_players[_targetedPlayer]);
    }
}
