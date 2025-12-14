#if UNITY_EDITOR
using System;

[Serializable]
public class PendingTableBuild
{
    public string csvPath;
    public string soClassName;
    public string outputFolder;
}
#endif