using UnityEngine;

public class SpawnToggleHandler : MonoBehaviour
{
    public NewObjectSpawner spawner;
    public ObjectDescriptionManager descriptionManager;
    public string prefabName;
    public string title;
    public string description;

    public void HandleToggleChanged(bool isOn)
    {
        if (isOn)
        {
            spawner.Spawn(prefabName);
            descriptionManager.UpdateDescription(title, description);
        }
        else
            spawner.Despawn();
    }
}
