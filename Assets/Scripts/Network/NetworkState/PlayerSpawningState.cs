using PurrNet;
using PurrNet.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


// First State
public class PlayerSpawningState : StateNode
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private List<Transform> spawnPoints = new();

    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

        if (!asServer) return;

        DespawnAllPlayers();

        var spawnedPlayers = new List<Player>();

        SpawnPlayers(spawnedPlayers);
        machine.Next(spawnedPlayers);
    }

    private void SpawnPlayers(List<Player> spawnedPlayers)
    {
        int currentSpawnIndex = 0;
        foreach (var player in networkManager.players)
        {
            var spawnPoint = spawnPoints[currentSpawnIndex];
            var newPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            newPlayer.GiveOwnership(player);
            spawnedPlayers.Add(newPlayer);
            currentSpawnIndex++;

            if (currentSpawnIndex >= spawnPoints.Count)
            {
                currentSpawnIndex = 0;
            }
        }
    }

    //destroy all player objects before spawning
    private void DespawnAllPlayers()
    {
        var allPlayers = FindObjectsByType<Player>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var player in allPlayers)
        {
            Destroy(player.gameObject);
        }
    }

    public override void Exit(bool asServer)
    {
        base.Exit();
    }
}
