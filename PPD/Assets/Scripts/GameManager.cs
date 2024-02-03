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
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Sphere[] spheres;
    [SerializeField] private List<GameObject> spheresPos;
    [SerializeField] private List<Material> _materials = new();
    [SerializeField] private int[] cudaCalculations;
    private int calc;
    public int iterations = 1;


    [Header("CUDA")] 
    public int cudaIterations = 1;
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
    private static readonly int GridDimX = Shader.PropertyToID("gridDimx");
    private static readonly int GridDimY = Shader.PropertyToID("gridDimy");
    private static readonly int GridDimZ = Shader.PropertyToID("gridDimz");
    
    [Header("HUD")]
    int _currentScene = 0;
    int _lastScene = 0;
    [SerializeField] private TextMeshProUGUI _fpsText;
    [SerializeField] private TextMeshProUGUI _sceneText;
    [SerializeField] private TextMeshProUGUI _iterationText;
    [SerializeField] private TextMeshProUGUI _calculationText;

    [SerializeField] Slider slider;
    private float fpsCooldown = 0.5f;
    private float fpsCooldownTimer = 0f;
    
    [Header("Scripts")]
    [SerializeField] private SequentialManager _sequentialManager;
    [SerializeField] private JobManager _jobManager;
    private static readonly int Iterations = Shader.PropertyToID("iterations");

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
        cudaCalculations = new int[spheresPos.Count];
        
        
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
            sphere.calc = 0;
            sphere.speed = 1f;
            // cube.speed = Random.Range(minSpeed, maxSpeed);
            sphere.maxY = sphere.yPosition + 2f;
            sphere.minY = sphere.yPosition - 2f;
            spheres[i] = sphere;
            i++;
        }
        _sequentialManager.SetSpheres(this.spheres, this.spheresPos);
        _sequentialManager.SetCalcText(_calculationText);
        _jobManager.SetSpheres(this.spheres, this.spheresPos);
        _jobManager.SetCalcText(_calculationText);
        SendDataToGPU();
    }

    public void ChangeIterations(Slider slider)
    {
        iterations = (int)slider.value;
        _iterationText.text = "Iterations: " + iterations;
        if (_currentScene == 0)
        {
            int sequentialIterations = _sequentialManager.GetIteration();
            // if(iterations == 1000)
            //     slider.value = sequentialIterations;
            // else
            _sequentialManager.ChangeIterations(iterations);
            if(_jobManager.GetIteration() <= iterations)
            {
                _jobManager.ChangeIterations(iterations);
                cudaIterations = iterations;
                computeShader.SetInt(Iterations, iterations);
            }
        }
        else if (_currentScene == 1)
        {
            _jobManager.ChangeIterations(iterations);
            if(cudaIterations <= iterations)
            {
                cudaIterations = iterations;
                computeShader.SetInt(Iterations, cudaIterations);
            }
        }
        else if (_currentScene == 2)
        {
            cudaIterations = iterations;
            computeShader.SetInt(Iterations, cudaIterations);
        }
        
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
            _sceneText.text = "Case 0 - Sequential";
            _currentScene = 0;
            Debug.Log(_sequentialManager.GetIteration());
            slider.value = _sequentialManager.GetIteration();
            slider.maxValue = 1000;
            _lastScene = 0;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            _sceneText.text = "Case 1 - Jobs";
            _currentScene = 1;
            if (_lastScene <= 1)
            {
                slider.maxValue = 10000;
                slider.value = _jobManager.GetIteration();
            }
            else
            {
                slider.value = _jobManager.GetIteration();
                slider.maxValue = 10000;
            }
            _lastScene = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            _sceneText.text = "Case 2 - Compute Shader";
            _currentScene = 2;
            slider.maxValue = 1000000;
            slider.value = cudaIterations;
            _lastScene = 2;
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
            // Roda OpenMp
            _jobManager.OpenMP();
        }
        else if(_currentScene == 2)
        {
            // Roda CUDA
            CUDA();
        }
    }


    private void CUDA()
    {
        computeShader.SetFloat(Time1, Time.deltaTime); // Envia dados de tempo para a GPU 
        computeShader.Dispatch(0, gridDimX, gridDimY, gridDimZ); // Inicia o calculo pela GPU
        
        spheresBuffer.GetData(spheres); // Recebe os dados da GPU
        int index = 0;
        foreach (Sphere sphere in spheres)
        {
            cudaCalculations[index] = sphere.calc;
            _calculationText.text = spheres[index].calc.ToString();
            spheresPos[index].transform.position = new Vector3(spheresPos[index].transform.position.x, sphere.yPosition, spheresPos[index].transform.position.z);
            index++;
        }
    }

    private void SendDataToGPU()
    {
        int totalSize = sizeof(float) * 4 + sizeof(int) * 11;
        spheresBuffer = new ComputeBuffer(spheres.Length, totalSize);
        spheresBuffer.SetData(spheres);
        computeShader.SetBuffer(0, Spheres, spheresBuffer);
        computeShader.SetInt(GridDimX, gridDimX);
        computeShader.SetInt(GridDimY, gridDimY);
        computeShader.SetInt(GridDimZ, gridDimZ);
        computeShader.SetInt(Iterations, iterations);
    }

    private void OnDestroy()
    {
        spheresBuffer.Dispose();
    }
}
