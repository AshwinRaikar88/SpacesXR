using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;

public class SimpleObjectSpawner : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField]
    private Slider scaleSlider;

     [Header("Scaling Settings")]
    [SerializeField]
    private int rotationSpeed = 500;
    [SerializeField]
    private int rotationSpeedAuto = 10;
    [SerializeField]
    private int scaleDivisions = 100;    
    [SerializeField]
    public float minScale = 0.1f;
    [SerializeField]
    public float maxScale = 5f;       
    private float scaleStep;
    private bool autoRotate = true;

    [Header("Spawn Settings")]
    // [Tooltip("The name of the prefab to load from the Resources folder.")]
    // [SerializeField] private string prefabName;

    [Tooltip("Distance in front of the camera to spawn the object.")]
    [SerializeField] private float spawnDistance = -30f;

    [Tooltip("Camera the object should spawn in front of. Defaults to main camera.")]
    [SerializeField] private Camera cameraToFace;

    [Tooltip("Optional visual effect prefab (loaded by name from Resources).")]
    [SerializeField] private string spawnEffectName;

    [Tooltip("Apply random Y rotation around forward vector.")]
    [SerializeField] private bool randomYRotation = true;

    [Tooltip("Y rotation range in degrees.")]
    [SerializeField] private float yRotationRange = 45f;


    public GameObject spawnedObject;

    private string lastSpawnedPrefabName;


    /// <summary>
    /// Event invoked after an object is spawned.
    /// </summary>
    public event Action<GameObject> OnObjectSpawned;

    void Awake()
    {
        if (cameraToFace == null)
            cameraToFace = Camera.main;

        scaleStep = (maxScale - minScale) / scaleDivisions;
        if (scaleSlider != null)
        {
            scaleSlider.minValue = minScale;
            scaleSlider.maxValue = maxScale;
            scaleSlider.value = 0.001f;
        }
    }

    // void Start()
    // {
    //     if (scaleSlider != null)
    //         scaleSlider.onValueChanged.AddListener(OnSliderChanged);   
    // }


    public void Spawn(string prefabName)
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogWarning("Prefab name is not set.");
            return;
        }

        Vector3 offsetRotatedDirection = Quaternion.AngleAxis(yRotationRange, Vector3.up) * cameraToFace.transform.forward;

        Vector3 spawnPoint = cameraToFace.transform.position + offsetRotatedDirection * spawnDistance;
        Vector3 spawnNormal = -offsetRotatedDirection;

        // If the same prefab is already spawned, just move and rotate it
        if (spawnedObject != null && prefabName == lastSpawnedPrefabName)
        {
            spawnedObject.transform.position = spawnPoint;
            spawnedObject.transform.rotation = Quaternion.LookRotation(cameraToFace.transform.forward, Vector3.up);

            if (randomYRotation)
            {
                float randomY = UnityEngine.Random.Range(-yRotationRange, yRotationRange);
                spawnedObject.transform.Rotate(Vector3.up, randomY);
            }

            Debug.Log($"Moved existing prefab '{prefabName}' to new position.");
            return;
        }

        // Load the prefab
        GameObject prefabToSpawn = Resources.Load<GameObject>($"Prefabs/{prefabName}");
        if (prefabToSpawn == null)
        {
            Debug.LogError($"Prefab '{prefabName}' not found in Resources.");
            return;
        }

        // Destroy old object if different
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }

        GameObject spawned = Instantiate(prefabToSpawn, spawnPoint, Quaternion.LookRotation(cameraToFace.transform.forward, Vector3.up));

        if (randomYRotation)
        {
            float randomY = UnityEngine.Random.Range(-yRotationRange, yRotationRange);
            spawned.transform.Rotate(Vector3.up, randomY);
        }

        // Load and spawn visualization effect if set
        if (!string.IsNullOrEmpty(spawnEffectName))
        {
            GameObject effectPrefab = Resources.Load<GameObject>(spawnEffectName);
            if (effectPrefab != null)
                Instantiate(effectPrefab, spawnPoint, spawned.transform.rotation);
            else
                Debug.LogWarning($"Spawn effect '{spawnEffectName}' not found in Resources.");
        }

        OnObjectSpawned?.Invoke(spawned);
        spawnedObject = spawned;
        lastSpawnedPrefabName = prefabName;
    }


    public void ScaleUp()
    {
        if (spawnedObject != null)
        {
            XRGrabInteractable interactable = spawnedObject.GetComponent<XRGrabInteractable>();
            Vector3 scale = interactable.GetTargetLocalScale();
            scale += Vector3.one * scaleStep;
            scale = Vector3.Min(scale, Vector3.one * maxScale);
            interactable.SetTargetLocalScale(scale);

            if (scaleSlider != null)
            {
                scaleSlider.value = scale.x;
            }
        }    
    }

    public void ScaleDown()
    {
        if (spawnedObject != null)
        {
            XRGrabInteractable interactable = spawnedObject.GetComponent<XRGrabInteractable>();
            Vector3 scale = interactable.GetTargetLocalScale();
            scale -= Vector3.one * scaleStep;
            scale = Vector3.Max(scale, Vector3.one * minScale);
            interactable.SetTargetLocalScale(scale);
            
            if (scaleSlider != null)
            {
                scaleSlider.value = scale.x;
            }
        }    
    }

    public void Despawn()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
        }
        else
        {
            Debug.LogWarning("No object to despawn.");
        }
    }
}
