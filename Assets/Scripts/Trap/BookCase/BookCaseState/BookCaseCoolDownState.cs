using UnityEngine;

public class BookCaseCoolDownState : IState<BookCase>
{
    public void Enter(BookCase context)
    {
        Debug.Log($"{this}¡¯¿‘");
    }

    public void Exit(BookCase context)
    {
        Debug.Log($"{this}≈ª√‚");
    }

    public void Execute(BookCase context)
    {
        context.SetState(new BookCaseIdleState());
    }
}
