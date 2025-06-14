[System.Serializable]
public class MoleculeInfo
{
    public int id;
    public string title;
    public string prefabName;
    public string description;
}

[System.Serializable]
public class MoleculeInfoList
{
    public MoleculeInfo[] molecules;
}
