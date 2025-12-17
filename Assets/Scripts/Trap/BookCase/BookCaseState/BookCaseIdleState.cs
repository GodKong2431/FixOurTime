using UnityEngine;

public class BookCaseIdleState : IState<BookCase>
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
        Vector2 origin = new Vector2(context.Rb.position.x, context.BookCaseBotton);

        RaycastHit2D[] leftHits = Physics2D.RaycastAll(origin,Vector2.right,10f,1 << 10);
        RaycastHit2D[] rightHits = Physics2D.RaycastAll(origin,Vector2.left,10f,1 << 10);

        foreach (var hit in leftHits)
        {
            if (hit.collider == null)
                continue;

            if (hit.collider.attachedRigidbody == context.Rb)
                continue;

            context.SetState(new BookCasePushState());
            return;
        }
        foreach (var hit in rightHits)
        {
            if (hit.collider == null)
                continue;

            if (hit.collider.attachedRigidbody == context.Rb)
                continue;

            context.SetState(new BookCasePushState());
            return;
        }
    }
}
