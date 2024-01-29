using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SequentialManager : MonoBehaviour
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


    private void Awake()
    {
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
        // Test();

        for(int i = 0; i < spheres.Length; i++)
        {
            //Make long calculations later
            for (int j = 0; j < 1000; j++)
            {
                Mathf.Sqrt(5000);
            }
            if(spheres[i].yPosition >= spheres[i].maxY)
            {
                spheres[i].speed = Random.Range(-minSpeed, -maxSpeed) * (((i)%64)+1)/10;
            }
            else if(spheres[i].yPosition <= spheres[i].minY)
            {
                spheres[i].speed = Random.Range(minSpeed, maxSpeed) * (((i)%64)+1)/10;
            }
            spheres[i].yPosition += spheres[i].speed * Time.deltaTime;
            spheresPos[i].transform.position = new Vector3(spheresPos[i].transform.position.x, spheres[i].yPosition, spheresPos[i].transform.position.z);
        }
    }
    
}
