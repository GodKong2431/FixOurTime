using UnityEngine;

public class BookCaseReturnState : IState<BookCase>
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

        context.transform.position = Vector2.MoveTowards(context.transform.position, context.transform.position + context.MoveDir(false), context.ReturnSpeed * Time.deltaTime);

        if (Vector2.Distance(context.transform.position, (Vector3)context.StartPos) <= 0.05f)
        {
            context.SetState(new BookCaseCoolDownState());
        }
    }
}
