using UnityEngine;

public class BookCasePushState : IState<BookCase>
{
    public void Enter(BookCase context)
    {
    }

    public void Exit(BookCase context)
    {
    }

    public void Execute(BookCase context)
    {
        
        RaycastHit2D leftHits = Physics2D.Raycast(context.Dir(), Vector2.down, 0.1f, 1 << 31);

        if (!leftHits)
        {
            context.SetState(new BookCaseStayState());
        }
        else
        {
            Debug.Log(leftHits.collider);
            context.transform.position = Vector2.MoveTowards(context.transform.position, context.transform.position + context.MoveDir(true), context.Speed * Time.deltaTime);
        }
    }
}
