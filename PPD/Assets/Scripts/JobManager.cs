using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.Serialization;

public class JobManager : MonoBehaviour
{
    [Serializable]
    public struct Sphere
    {
        public float yPosition;
        public float speed;
        public float maxY;
        public float minY;
        public float index;
        public int3 id;
        public int3 groupId;
        public int3 groupIdThread;
    }
    
    // [SerializeField] private Cube[] cubes;
    [SerializeField] private NativeArray<Sphere> sphereList;
    [SerializeField] private List<GameObject> spheresPos;
    private float _maxY = 2f;
    private float _minY = -2f;
    [SerializeField] private List<Material> _materials = new();
    public float minSpeed = 0.1f;
    public float maxSpeed = 0.5f;


    private void Awake()
    {
        sphereList = new NativeArray<Sphere>(spheresPos.Count, Allocator.Persistent);
        
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
            sphereList[i] = sphere;
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
        // Test();
        SphereMoveJob job = new SphereMoveJob()
        {
            spheres = sphereList,
            time = Time.deltaTime
        };
        
        JobHandle jobHandle = job.Schedule(sphereList.Length, 64);
        jobHandle.Complete();
        for(int i = 0; i < sphereList.Length; i++)
        {
            spheresPos[i].transform.position = new Vector3(spheresPos[i].transform.position.x, sphereList[i].yPosition, spheresPos[i].transform.position.z);
        }
        // Debug.Log(cubeList[0].index);
    }
    
    [BurstCompile]
    public struct SphereMoveJob : IJobParallelFor
    {
        public NativeArray<Sphere> spheres;
        public float time;
        
        public void Execute(int index)
        {
            Sphere sphere = spheres[index];
            sphere.index = 0;
            for (int i = 0; i < 10000; i++)
            {
                sphere.index += Mathf.Sqrt(5000+index)/1000;
            }
            if(sphere.yPosition >= sphere.maxY)
            {
                sphere.speed = -3f * (((index)%64)+1)/25;
            }
            else if(sphere.yPosition <= sphere.minY)
            {
                sphere.speed = 3f * (((index)%64)+1)/25;
            }
            sphere.yPosition += spheres[index].speed * time;
            
            spheres[index] = sphere;
        }
    }
}
