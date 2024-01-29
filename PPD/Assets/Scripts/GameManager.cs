using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Serializable]
    public struct Sphere
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
    
    [SerializeField] private Sphere[] spheres;
    [SerializeField] private List<GameObject> spheresPos;
    private float _maxY = 2f;
    private float _minY = -2f;
    [SerializeField] private List<Material> _materials = new();
    public float minSpeed = 0.1f;
    public float maxSpeed = 0.5f;

    [Header("CUDA")]
    public ComputeShader computeShader;

    private static readonly int Spheres = Shader.PropertyToID("spheres");
    private static readonly int Time1 = Shader.PropertyToID("time");

    private void Awake()
    {
        // _maxY = transform.position.y + 2f;
        // _minY = transform.position.y - 2f;
        // cubesPos = GameObject.FindGameObjectsWithTag("Cube").ToList();
        int i = 0;
        foreach (GameObject spherePos in spheresPos)
        {
            int materialID = (i / 64) % 4;
            spherePos.GetComponent<MeshRenderer>().material = _materials[materialID];
            Sphere sphere = new Sphere();
            sphere.yPosition = spherePos.transform.position.y;
            sphere.speed = 1f;
            // cube.speed = Random.Range(minSpeed, maxSpeed);
            sphere.maxY = sphere.yPosition + 2f;
            sphere.minY = sphere.yPosition - 2f;
            spheres[i] = sphere;
            i++;
        }
        
    }

    // Update is called once per frame
    void Update()
    {   
        if(Input.GetKeyDown(KeyCode.E))
        {
            // Test();
        }
        Test();
        
    }


    private void Test()
    {
        int totalSize = sizeof(float) * 4 + sizeof(int) * 10; 
        ComputeBuffer spheresBuffer = new ComputeBuffer(spheres.Length, totalSize);
        spheresBuffer.SetData(spheres);
        computeShader.SetBuffer(0, Spheres, spheresBuffer);
        computeShader.SetFloat(Time1, Time.deltaTime);
        computeShader.Dispatch(0, 16, 8, 1);
        
        spheresBuffer.GetData(spheres);
        int index = 0;
        
        foreach (Sphere sphere in spheres)
        {
            spheresPos[index].transform.position = new Vector3(spheresPos[index].transform.position.x, sphere.yPosition, spheresPos[index].transform.position.z);
            index++;
        }
        
        spheresBuffer.Dispose();
    }
    
    private void MoveTo(Transform transform, float speed)
    {
        transform.position += Vector3.up * (speed * Time.deltaTime);
    }

    
}
