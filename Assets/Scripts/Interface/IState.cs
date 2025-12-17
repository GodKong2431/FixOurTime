
public interface IState<T>
{
    void Enter(T context);
    void Exit(T context);
    void Execute(T context);
}
