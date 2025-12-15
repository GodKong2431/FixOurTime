#if UNITY_EDITOR
using System;

//컴파일 이후에도 작업해야해서 저장해 놓는 목록
[Serializable]
public class PendingTableBuild
{
    public string csvPath;
    public string soClassName;
}
#endif