using System;
using UnityEngine;
using UnityEngine.UI;

public class NewObjectSpawner : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField]
    private Slider scaleSlider;
    public bool isMagicScaleEnabled = false;

    [Header("Scaling Settings")]
    [SerializeField]    
    private int scaleDivisions = 100;
    [SerializeField]
    public float minScale = 0.1f;
    [SerializeField]
    public float maxScale = 5f;
    private float scaleStep;    

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

    [SerializeField] private Transform fixedSpawnPoint;

    public MagicScaler magicScaler;
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

        Vector3 spawnPoint = fixedSpawnPoint.position;
        Quaternion spawnRotation = Quaternion.LookRotation(fixedSpawnPoint.forward, Vector3.up);


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
        if (magicScaler != null)
            magicScaler.SetTargetObject(spawnedObject);

        lastSpawnedPrefabName = prefabName;

    }


    public void ScaleUp()
    {
        if (spawnedObject != null)
        {
            
            Vector3 scale = spawnedObject.transform.localScale;
            scale += Vector3.one * scaleStep;
            spawnedObject.transform.localScale = Vector3.Min(scale, Vector3.one * maxScale);

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
            Vector3 scale = spawnedObject.transform.localScale;
            scale -= Vector3.one * scaleStep;
            spawnedObject.transform.localScale = Vector3.Max(scale, Vector3.one * minScale);

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
    
    // public void ToggleChildCollider(bool enable)
    // {
    //     isMagicScaleEnabled = !enable;

    //     if (spawnedObject == null)
    //     {
    //         Debug.LogWarning("No object has been spawned.");
    //         return;
    //     }

    //     // Find the first Collider in children (excluding the root)
    //     Collider childCollider = spawnedObject.GetComponentInChildren<Collider>();

    //     if (childCollider != null && childCollider.gameObject != spawnedObject)
    //     {
    //         childCollider.enabled = enable;
    //         Debug.Log($"Child collider on '{childCollider.gameObject.name}' has been {(enable ? "enabled" : "disabled")}.");
    //     }
    //     else
    //     {
    //         Debug.LogWarning("No child collider found or collider belongs to root object.");
    //     }
    // }
}
