using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Move : MonoBehaviour
{
    private float _maxY = 2f;
    private float _minY = -2f;
    [SerializeField] private List<Material> _materials = new();
    bool movingUp = true;
    private bool canChange = true;
    private float _speed;

    private void Awake()
    {
        _maxY = transform.position.y + 2f;
        _minY = transform.position.y - 2f;
        movingUp = new System.Random().Next(0, 2) == 1;
        this.GetComponent<MeshRenderer>().material = _materials[new System.Random().Next(0, _materials.Count)];
        // _speed = Random.Range(0.1f, 1f);
        _speed = Random.Range(0.5f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y >= _maxY -0.1f || transform.position.y <= _minY + 0.1f)
        {
            // Debug.Log("Changin");
            ChangeDirection();
        }
        if(transform.position.y < _maxY -0.1f && transform.position.y > _minY + 0.1f)
        {
            canChange = true;
        }
        MoveTo(transform.position);
    }

    private void ChangeDirection()
    {
        if (canChange)
        {
            canChange = false;
            // _speed = Random.Range(0.5f, 2f);
            movingUp = !movingUp;
        }
    }
    
    private void MoveTo(Vector3 position)
    {
        float towards = movingUp ? _maxY : _minY;
        transform.position = Vector3.Slerp(transform.position, new Vector3(position.x, towards, position.z), _speed * Time.deltaTime) ;
    }
}
