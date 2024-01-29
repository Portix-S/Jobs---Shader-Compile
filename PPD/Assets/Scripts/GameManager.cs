using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Serializable]
    public struct Cube
    {
        public float yPosition;
        public float speed;
        public float maxY;
        public float minY;
        public int index;
        public int3 id;
        public int3 groupId;
        public int3 groupIdThread;
    }
    
    [SerializeField] private Cube[] cubes;
    [SerializeField] private List<GameObject> cubesPos;
    private float _maxY = 2f;
    private float _minY = -2f;
    [SerializeField] private List<Material> _materials = new();
    public float minSpeed = 0.1f;
    public float maxSpeed = 0.5f;

    [Header("CUDA")]
    public ComputeShader computeShader;

    private static readonly int Cubes = Shader.PropertyToID("cubes");
    private static readonly int Time1 = Shader.PropertyToID("time");

    private void Awake()
    {
        // _maxY = transform.position.y + 2f;
        // _minY = transform.position.y - 2f;
        // cubesPos = GameObject.FindGameObjectsWithTag("Cube").ToList();
        int i = 0;
        foreach (GameObject cubePos in cubesPos)
        {
            int materialID = (i / 64) % 4;
            cubePos.GetComponent<MeshRenderer>().material = _materials[materialID];
            Cube cube = new Cube();
            cube.yPosition = cubePos.transform.position.y;
            cube.speed = 1f;
            // cube.speed = Random.Range(minSpeed, maxSpeed);
            cube.maxY = cube.yPosition + 4f;
            cube.minY = cube.yPosition - 4f;
            cubes[i] = cube;
            i++;
        }
        // foreach (Cube cube in cubes)
        // {
        //     var cube1 = cube;
        //     cube1.pos = cubesPos[i].transform.position;
        //     i++;
        //     // cube.pos.GetComponent<MeshRenderer>().material = _materials[new System.Random().Next(0, _materials.Count)];
        //     cube1.speed = Random.Range(minSpeed, maxSpeed);
        //     var position = cube1.pos;
        //     cube1.maxY = position.y + 2f;
        //     cube1.minY = position.y - 2f;
        //     cube = cube1;
        //     // cube.movingUp = new System.Random().Next(0, 2) == 1;
        //
        // }
        // _speed = Random.Range(0.1f, 1f);
        
    }

    // Update is called once per frame
    void Update()
    {   
        if(Input.GetKeyDown(KeyCode.E))
        {
            // Test();
        }
        Test();

    
        // foreach (Cube cube in cubes)
           // {
        //     // float speed = Random.Range(0.5f, 2f);
        //     if(cube.pos.position.y >= cube.maxY)
        //     {
        //         cube.speed = Random.Range(-minSpeed, -maxSpeed);
        //     }
        //     else if(cube.pos.position.y <= cube.minY)
        //     {
        //         cube.speed = Random.Range(minSpeed, maxSpeed);
        //     }
        //     MoveTo(cube.pos, cube.speed);
        // }
    }


    private void Test()
    {
        int totalSize = sizeof(float) * 4 + sizeof(int) * 10; 
        ComputeBuffer cubesBuffer = new ComputeBuffer(cubes.Length, totalSize);
        cubesBuffer.SetData(cubes);
        computeShader.SetBuffer(0, Cubes, cubesBuffer);
        computeShader.SetFloat(Time1, Time.deltaTime);
        computeShader.Dispatch(0, 8, 8, 1);
        
        cubesBuffer.GetData(cubes);
        int index = 0;
        
        foreach (Cube cube in cubes)
        {
            // if(index < 900)
            //     Debug.Log("id: " + cube.id + " gid: "+ cube.groupId + " + gidthread " + cube.groupIdThread);
            // cubesPos[index].transform.position = cube.pos;
            cubesPos[index].transform.position = new Vector3(cubesPos[index].transform.position.x, cube.yPosition, cubesPos[index].transform.position.z);
            index++;
            // Debug.Log(cube.pos.y);
        }


        // Maybe need a list of Objects for keeping track of the cubes?
        // foreach (Cube cube in cubes)
        // {
        // }
        cubesBuffer.Dispose();
    }
    
    private void MoveTo(Transform transform, float speed)
    {
        transform.position += Vector3.up * (speed * Time.deltaTime);
    }

    
}
