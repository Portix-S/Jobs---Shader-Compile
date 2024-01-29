using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MoveJob : MonoBehaviour
{
    private float _maxY = 2f;
    private float _minY = -2f;
    [SerializeField] private List<Material> _materials = new();
    bool movingUp = true;
    private bool canChange = true;
    private float _speed;
    private NativeArray<Vector3> _positionResult;
    private NativeArray<bool> _movingUpResult;
    private NativeArray<bool> _canChangeResult;
    private JobHandle _jobHandle;
    private List<Vector3> _positions = new();
    private void Awake()
    {
        var position = transform.position;
        _maxY = position.y + 2f;
        _minY = position.y - 2f;
        movingUp = new System.Random().Next(0, 2) == 1;
        this.GetComponent<MeshRenderer>().material = _materials[new System.Random().Next(0, _materials.Count)];
        // _speed = Random.Range(0.1f, 1f);
        _speed = Random.Range(0.5f, 2f);
        _positionResult = new NativeArray<Vector3>(1, Allocator.Persistent);
        _movingUpResult = new NativeArray<bool>(1, Allocator.Persistent);
        _canChangeResult = new NativeArray<bool>(1, Allocator.Persistent);
    }

    // Update is called once per frame
    void Update()
    {
        Job job = new Job(_maxY, _minY, movingUp, canChange, _speed, Time.deltaTime, transform.position, _positionResult, _canChangeResult, _movingUpResult);
        _jobHandle = job.Schedule(1, 1, _jobHandle);
    }

    private void LateUpdate()
    {
        _jobHandle.Complete();
        transform.position = _positionResult[0];
        movingUp = _movingUpResult[0];
        canChange = _canChangeResult[0];
    }

    private void OnDestroy()
    {
        _positionResult.Dispose();
    }
}
