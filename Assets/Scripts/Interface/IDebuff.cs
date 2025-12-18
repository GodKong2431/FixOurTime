using UnityEngine;

public interface IDebuff<T>
{
    string Name { get; }
    float Duration { get; set; }
    void OnEnter(T conText);
    void OnExit(T conText);
    void OnExecute(T conText);

}
