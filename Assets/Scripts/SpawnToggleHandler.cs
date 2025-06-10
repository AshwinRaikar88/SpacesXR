using UnityEngine;

public class SpawnToggleHandler : MonoBehaviour
{
    public NewObjectSpawner spawner;
    public string prefabName;

    public void HandleToggleChanged(bool isOn)
    {
        if (isOn)
            spawner.Spawn(prefabName);
        else
            spawner.Despawn();
    }
}
