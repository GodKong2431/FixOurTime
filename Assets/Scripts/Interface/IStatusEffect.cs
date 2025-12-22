using UnityEngine;

public interface IStatusEffect<T>
{
    string Name { get; }
    float Duration { get; set; }
    bool IsPositive { get; }
    void OnEnter(T conText);
    void OnExit(T conText);
    void OnExecute(T conText);

}
