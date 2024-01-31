using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Spheres;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Sphere[] spheres;
    [SerializeField] private List<GameObject> spheresPos;
    [SerializeField] private List<Material> _materials = new();
    public float minSpeed = 0.1f;
    public float maxSpeed = 0.5f;
    
    
    [Header("CUDA")]
    public ComputeShader computeShader;
    ComputeBuffer spheresBuffer;
    public int gridDimX = 16;
    public int gridDimY = 16;
    public int gridDimZ = 1;

    public int blockDimX = 8;
    public int blockDimY = 8;
    public int blockDimZ = 1;
    
    private static readonly int Spheres = Shader.PropertyToID("spheres");
    private static readonly int Time1 = Shader.PropertyToID("time");
    [SerializeField] private GameObject spherePrefab;
    public List<GameObject> teste;
    private static readonly int GridDimX = Shader.PropertyToID("gridDimx");
    private static readonly int GridDimY = Shader.PropertyToID("gridDimy");
    private static readonly int GridDimZ = Shader.PropertyToID("gridDimz");
    
    [Header("HUD")]
    int _currentScene = 0;
    [SerializeField] private TextMeshProUGUI _fpsText;
    [SerializeField] private TextMeshProUGUI _sceneText;
    private float fpsCooldown = 0.5f;
    private float fpsCooldownTimer = 0f;
    
    [Header("Scripts")]
    [SerializeField] private SequentialManager _sequentialManager;
    [SerializeField] private JobManager _jobManager;
    private void Awake()
    {
        int totalSize = blockDimX * blockDimY * blockDimZ * gridDimX * gridDimY * gridDimZ;
        int linha = 0;
        int colunaBloco = 0;
        int linhaBloco = 0;
        for (int coluna = 0; coluna < totalSize; coluna++)
        {
            if (coluna % blockDimX == 0 && coluna != 0)
            {
                linha++;
                if(linha % blockDimY ==  0)
                {
                    colunaBloco++;
                    if (colunaBloco % gridDimX == 0)
                    {
                        linhaBloco++;
                    }
                }
            }
            
            float x = 1.1f * (coluna % blockDimX) + (1.1f * blockDimX + 3.3f) * (colunaBloco % gridDimX);
            float z = -1.1f * (linha % blockDimY) - (1.1f * blockDimY + 3.3f) * (linhaBloco % gridDimY);
            GameObject sphere = Instantiate(spherePrefab, new Vector3(x, 0, z), Quaternion.identity);
            spheresPos.Add(sphere);
        }
        spheres = new Sphere[spheresPos.Count];
        
        // _maxY = transform.position.y + 2f;
        // _minY = transform.position.y - 2f;
        // cubesPos = GameObject.FindGameObjectsWithTag("Cube").ToList();
        int i = 0;
        foreach (GameObject spherePos in spheresPos)
        {
            int materialID = (i / (blockDimX*blockDimY)) % _materials.Count;
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
        _sequentialManager.SetSpheres(this.spheres, this.spheresPos);
        _jobManager.SetSpheres(this.spheres, this.spheresPos);
        SendDataToGPU();
    }

    // Update is called once per frame
    void Update()
    {
        // float fps = 1.0f / Time.deltaTime;
        // _fpsText.text = "FPS: " + (fps).ToString("0");
        // Calculates Average FPs and updates on screen
        fpsCooldownTimer += Time.deltaTime;
        float fps = 1.0f / Time.deltaTime;
        
        if(fpsCooldownTimer >= fpsCooldown)
        {
            fpsCooldownTimer -= fpsCooldown;
            _fpsText.text = "FPS: " + (fps).ToString("0");
            if(fps < 15f)
            {
                _fpsText.color = Color.red;
            }
            else if(fps < 30f)
            {
                _fpsText.color = Color.yellow;
            }
            else
            {
                _fpsText.color = Color.green;
            }
        }
        
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _sceneText.text = "Cena 0 - Sequencial";
            // _sequentialManager.SetSpheres(spheres, spheresPos);
            _currentScene = 0;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            _sceneText.text = "Cena 1 - Paralelo com OpenMP";
            _currentScene = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            _sceneText.text = "Cena 2 - CUDA";
            _currentScene = 2;
        }
        
        if(Input.GetKeyDown(KeyCode.E))
        {
            // Test();
        }

        if (_currentScene == 0)
        {
            //Rodar código Seq;
            _sequentialManager.Sequential();
        }
        else if(_currentScene == 1)
        {
            // ROda OpenMp
            _jobManager.OpenMP();
        }
        else if(_currentScene == 2)
        {
            // Roda CUDA
            CUDA();
        }
        // SphereMoveJob job = new SphereMoveJob()
    }


    private void CUDA()
    {
        computeShader.SetFloat(Time1, Time.deltaTime);
        computeShader.Dispatch(0, gridDimX, gridDimY, gridDimZ);
        
        spheresBuffer.GetData(spheres);
        int index = 0;
        
        foreach (Sphere sphere in spheres)
        {
            spheresPos[index].transform.position = new Vector3(spheresPos[index].transform.position.x, sphere.yPosition, spheresPos[index].transform.position.z);
            index++;
        }
    }

    private void SendDataToGPU()
    {
        int totalSize = sizeof(float) * 4 + sizeof(int) * 10;
        spheresBuffer = new ComputeBuffer(spheres.Length, totalSize);
        spheresBuffer.SetData(spheres);
        computeShader.SetBuffer(0, Spheres, spheresBuffer);
        computeShader.SetInt(GridDimX, gridDimX);
        computeShader.SetInt(GridDimY, gridDimY);
        computeShader.SetInt(GridDimZ, gridDimZ);
    }

    private void OnDestroy()
    {
        spheresBuffer.Dispose();
    }
}
