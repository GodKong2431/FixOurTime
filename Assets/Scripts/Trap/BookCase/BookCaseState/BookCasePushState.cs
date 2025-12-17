

using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class BookCasePushState : IState<BookCase>
{
    public void Enter(BookCase context)
    {
        Debug.Log($"{this}진입");
    }

    public void Exit(BookCase context)
    {
        Debug.Log($"{this}탈출");
    }

    public void Execute(BookCase context)
    {
        
        RaycastHit2D leftHits = Physics2D.Raycast(context.Dir(), Vector2.down, 2f, 1 << 31);

        if (!leftHits)
        {
            context.SetState(new BookCaseStayState());
        }
        else
        {
            
            context.transform.position = Vector2.MoveTowards(context.transform.position, context.transform.position + context.MoveDir(true), context.Speed * Time.deltaTime);
        }
    }
}
