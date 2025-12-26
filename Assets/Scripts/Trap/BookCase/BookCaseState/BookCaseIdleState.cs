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
        Vector2 origin = new Vector2(context.Rb.position.x, context.BookCaseBottom);
        Vector2 RaySize = new Vector2(40, 1);

        RaycastHit2D[] BoxHits = Physics2D.BoxCastAll(origin, RaySize, 0f, Vector2.down, 0.2f, 1 << 10);

        if (BoxHits.Length > 0)
        {
            foreach (var hit in BoxHits)
            {
                if (hit.collider.bounds.min.y > context.BookCaseBottom)
                    context.SetState(new BookCasePushState());
                return;
            }
        }
    }
}
