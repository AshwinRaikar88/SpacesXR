using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class XRSpawnManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField]
    private AudioSource spawnSFX;

    [SerializeField]
    private AudioSource notifSFX;

    [Header("UI Elements")]
    [SerializeField]
    private Slider scaleSlider;

    [SerializeField]
    private ParticleSystem spawnVFX;

    public GameObject libraryPanel;
    public GameObject loadingText;

    [Header("Scaling Settings")]
    [SerializeField]
    private int scaleDivisions = 100;
    [SerializeField]
    public float minScale = 0.1f;

    [SerializeField]
    public float maxScale = 5f;
    private float scaleStep;

    [Header("Spawn Settings")]
    [SerializeField]
    private bool randomYRotation = true;

    [SerializeField]
    private float yRotationRange = 45f;

    public GameObject spawnedObject;

    private bool isSpawning = false;

    [SerializeField]
    private Transform fixedSpawnPoint;

    public MagicScaler magicScaler;

    /// <summary>
    /// Event invoked after an object is spawned.
    /// </summary>
    public event Action<GameObject> OnObjectSpawned;

    void Awake()
    {        
        scaleStep = (maxScale - minScale) / scaleDivisions;

        if (scaleSlider != null)
        {
            scaleSlider.minValue = minScale;
            scaleSlider.maxValue = maxScale;
            scaleSlider.value = 0.001f;
        }

        if (spawnVFX != null)
        {
            spawnVFX.Stop();
        }
    }

    // void Start()
    // {
    //     if (scaleSlider != null)
    //         scaleSlider.onValueChanged.AddListener(OnSliderChanged);   
    // }


    public void SpawnAsync(string prefabAddress)
    {
        if (string.IsNullOrEmpty(prefabAddress) || isSpawning)
        {
            Debug.LogWarning("Spawn is already in progress or invalid address.");
            return;
        }

        isSpawning = true;

        libraryPanel.SetActive(false);
        loadingText.SetActive(true);

        // If there’s already a spawned object, destroy it first
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
        }

        if (spawnVFX != null)
        {
            spawnVFX.Play();
        }

        if (spawnSFX != null)
        {
            ToggleAudio();
        }

        Addressables.LoadAssetAsync<GameObject>(prefabAddress).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject prefab = handle.Result;

                spawnedObject = Instantiate(prefab, fixedSpawnPoint.position, Quaternion.identity);

                if (randomYRotation)
                {
                    float randomY = UnityEngine.Random.Range(-yRotationRange, yRotationRange);
                    spawnedObject.transform.Rotate(Vector3.up, randomY);
                }

                OnObjectSpawned?.Invoke(spawnedObject);

                if (magicScaler != null)
                    magicScaler.SetTargetObject(spawnedObject);
            }
            else
            {
                Debug.LogError($"Failed to load addressable prefab '{prefabAddress}'.");
            }

            libraryPanel.SetActive(true);
            loadingText.SetActive(false);
            isSpawning = false;

            if (spawnVFX != null)
            {
                spawnVFX.Stop();
            }

            if (spawnSFX != null)
            {
                ToggleAudio();
            }
            
            if (notifSFX != null)
            {   
                notifSFX.Play();
            }
        };
    }


    public void Spawn(string prefabName)
    {
        SpawnAsync($"Prefabs/{prefabName}");
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
    
    public void ToggleAudio()
    {
        if (spawnSFX.isPlaying)
        {
            spawnSFX.Stop();
        }
        else
        {
            spawnSFX.Play(); // Resumes from where it was paused
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
