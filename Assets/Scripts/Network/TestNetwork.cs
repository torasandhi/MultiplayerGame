using NUnit.Framework.Internal.Commands;
using PurrNet;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TestNetwork : NetworkIdentity
{
    [SerializeField] private List<TestStruct> _testStruct;
    [SerializeField] private Renderer _renderer;

    protected override void OnSpawned()
    {
        base.OnSpawned();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            SetColorObserver(_testStruct[0].color);
        }
    }

    [ObserversRpc]
    private void SetColorObserver(Color color)
    {
        _renderer.material.color = color;
    }
}

[Serializable]
public struct TestStruct
{
    public Color color;
}

