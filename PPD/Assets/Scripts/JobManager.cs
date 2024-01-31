using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.Serialization;
using Spheres;
using TMPro;

public class JobManager : MonoBehaviour
{
    [SerializeField] private float[] indexes;
    [SerializeField] private NativeArray<Sphere> sphereList;
    
    [SerializeField] private List<GameObject> spheresPos;
    private float _maxY = 2f;
    private float _minY = -2f;
    [SerializeField] private List<Material> _materials = new();
    public float minSpeed = 0.1f;
    public float maxSpeed = 0.5f;
    private int iterations = 1;
    private TextMeshProUGUI _calculationText;

    private SphereMoveJob job;

    private void Awake()
    {
        //
        // int i = 0;
        // foreach (GameObject spherePos in spheresPos)
        // {
        //     int materialID = (i / 64) % 4;
        //     spherePos.GetComponent<MeshRenderer>().material = _materials[materialID];
        //     Sphere sphere = new Sphere();
        //     sphere.yPosition = spherePos.transform.position.y;
        //     sphere.speed = 1f;
        //     // cube.speed = Random.Range(minSpeed, maxSpeed);
        //     sphere.maxY = sphere.yPosition + 2f;
        //     sphere.minY = sphere.yPosition - 2f;
        //     sphereList[i] = sphere;
        //     i++;
        // }
    }
    
    public void SetSpheres(Sphere[] spheres, List<GameObject> spheresPos)
    {
        indexes = new float[spheres.Length];
        sphereList = new NativeArray<Sphere>(spheresPos.Count, Allocator.Persistent);
        sphereList.CopyFrom(spheres);
        this.spheresPos = spheresPos;
    }
    
    public void SetCalcText(TextMeshProUGUI text)
    {
        _calculationText = text;
    }
    
    public void ChangeIterations(int value)
    {
        iterations = value;
        try
        {
            job.iterations = value;
        }
        catch (Exception e)
        {
            Console.WriteLine("Job not initialized");
            throw;
        }
    }
    
    public int GetIteration()
    {
        return iterations;
    }


    // Update is called once per frame
    public void OpenMP()
    {   
        
        job = new SphereMoveJob()
        {
            spheres = sphereList,
            time = Time.deltaTime,
            iterations = iterations
        };
        
        JobHandle jobHandle = job.Schedule(sphereList.Length, 64);
        jobHandle.Complete();
        
        _calculationText.text = sphereList[0].calc.ToString();

        for(int i = 0; i < sphereList.Length; i++)
        {
            indexes[i] = sphereList[i].index;
            spheresPos[i].transform.position = new Vector3(spheresPos[i].transform.position.x, sphereList[i].yPosition, spheresPos[i].transform.position.z);
        }
        // Debug.Log(cubeList[0].index);
    }
    
    // [BurstCompile]
    public struct SphereMoveJob : IJobParallelFor
    {
        public NativeArray<Sphere> spheres;
        public float time;
        public int iterations;
        
        public void Execute(int index)
        {
            Sphere sphere = spheres[index];
            sphere.calc = index;
            // sphere.index = 0;
            for (int i = 0; i < iterations; i++)
            {
                sphere.calc += (int)(Mathf.Sqrt(5000) * sphere.yPosition);
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
