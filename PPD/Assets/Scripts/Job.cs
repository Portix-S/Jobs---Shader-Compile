using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;

[BurstCompile]
public struct Job : IJobParallelFor
{
    private float _maxY;
    private float _minY;
    bool movingUp;
    private bool canChange;
    private float _speed;
    private float _time;
    private Vector3 _position;
    private NativeArray<Vector3> _positionResult;
    private NativeArray<bool> _movingUpResult;
    private NativeArray<bool> _canChangeResult;

    public Job(float maxY, float minY, bool movingUp, bool canChange, float speed, float time, Vector3 position, 
        NativeArray<Vector3> positionResult, NativeArray<bool> canChangeResult, NativeArray<bool> movingUpResult)
    {
        _maxY = maxY;
        _minY = minY;
        this.movingUp = movingUp;
        this.canChange = canChange;
        _speed = speed;
        _time = time;
        _position = position;
        _positionResult = positionResult;
        _canChangeResult = canChangeResult;
        _movingUpResult = movingUpResult;
    }

    
    // Update is called once per frame
    public void Execute(int index)
    {
        if(_position.y >= _maxY - 0.1f || _position.y <= _minY + 0.1f)
        {
            // Debug.Log("Changin");
            ChangeDirection();
        }
        if(_position.y < _maxY - 0.1f && _position.y > _minY + 0.1f)
        {
            canChange = true;
            _canChangeResult[0] = canChange;
        }
        MoveTo(_position);
    }

    private void ChangeDirection()
    {
        if (canChange)
        {
            canChange = false;
            // _speed = Random.Range(0.5f, 2f);
            movingUp = !movingUp;
            _movingUpResult[0] = movingUp;
            _canChangeResult[0] = canChange;
        }
    }

    private void MoveTo(Vector3 position)
    {
        float towards = movingUp ? _maxY : _minY;
        _position = Vector3.Slerp(_position, new Vector3(_position.x, towards, _position.z), _speed * _time);
        // _position += Vector3.up * _speed * _time * (movingUp ? 1 : -1); 
        _positionResult[0] = _position;
    }
}
