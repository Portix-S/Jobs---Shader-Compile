using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Spheres;
using TMPro;

public class SequentialManager : MonoBehaviour
{
    [SerializeField] private Sphere[] spheres;
    [SerializeField] private List<GameObject> spheresPos;
    private float _maxY = 2f;
    private float _minY = -2f;
    [SerializeField] private List<Material> _materials = new();
    public float minSpeed = 0.1f;
    public float maxSpeed = 0.5f;
    private int iterations = 1;
    private int calc;
    private TextMeshProUGUI _calculationText;

    private void Awake()
    {
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
        //     spheres[i] = sphere;
        //     i++;
        // }
    }

    // Update is called once per frame
    public void Sequential()
    {
        _calculationText.text = spheres[0].index.ToString();
        for(int i = 0; i < spheres.Length; i++)
        {
            spheres[i].index = i;
            //Make long calculations later
            for (int j = 0; j < iterations; j++)
            {
                spheres[i].index += (int)(Mathf.Sqrt(5000) * (spheres[i].yPosition));
            }
            if(spheres[i].yPosition >= spheres[i].maxY)
            {
                spheres[i].speed = (((i)%64)+1)* -0.2f;
            }
            else if(spheres[i].yPosition <= spheres[i].minY)
            {
                spheres[i].speed = (((i)%64)+1)* 0.2f;
            }
            spheres[i].yPosition += spheres[i].speed * Time.deltaTime;
            spheresPos[i].transform.position = new Vector3(spheresPos[i].transform.position.x, spheres[i].yPosition, spheresPos[i].transform.position.z);
        }
    }
    
    public void SetSpheres(Sphere[] spheres, List<GameObject> spheresPos)
    {
        this.spheres = new Sphere[spheres.Length];
        // this.spheres = spheres;
        for (int i = 0; i < spheres.Length; i++)
        {
            this.spheres[i] = spheres[i];
        }
        this.spheresPos = spheresPos;
    }
    
    public void SetCalcText(TextMeshProUGUI text)
    {
        _calculationText = text;
    }
    
    public void ChangeIterations(int value)
    {
        iterations = value;
    }
    
    public int GetIteration()
    {
        return iterations;
    }
    // public void SetSpheres(Sphere[] spheres)
    // {
    //     
    // }
}
