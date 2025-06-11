using UnityEngine;
using TMPro;

public class ObjectDescriptionManager : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    private MoleculeInfo[] moleculeData;

    void Start()
    {
        LoadDescriptionsFromJSON();
    }

    void LoadDescriptionsFromJSON()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("molecules_data");
        if (jsonFile != null)
        {
            string json = "{\"molecules\":" + jsonFile.text + "}";
            MoleculeInfoList data = JsonUtility.FromJson<MoleculeInfoList>(json);
            moleculeData = data.molecules;
            // Debug.Log("Molecule descriptions loaded successfully.");
            // foreach (var info in moleculeData)
            // {
            //     Debug.Log($"Title: {info.title}, Description: {info.description}");
            // }
        }
        else
        {
            Debug.LogError("Molecule description file not found!");
        }
    }

    public void ShowObjectDescription(int id)
    {
        if (id >= 0 && id < moleculeData.Length)
        {
            var info = moleculeData[id];
            titleText.text = info.title;
            descriptionText.text = info.description;

            Debug.Log(info.title);
            Debug.Log(info.description);
        }
        else
        {
            titleText.text = "Not Found";
            descriptionText.text = "No description available for this molecule.";
        }
    }
    
    public void UpdateDescription(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
    }
}
