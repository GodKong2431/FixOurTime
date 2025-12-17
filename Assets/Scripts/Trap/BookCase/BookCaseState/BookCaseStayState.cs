using UnityEngine;

public class BookCaseStayState : IState<BookCase>
{
    float t;

    public void Enter(BookCase context)
    {
        Debug.Log($"{this}ÁøÀÔ");
        t = 0;
    }

    public void Exit(BookCase context)
    {
        Debug.Log($"{this}Å»Ãâ");
    }

    public void Execute(BookCase context)
    {
        t += Time.deltaTime;

        if(t >= context.StayTime)
        {
            context.SetState(new BookCaseReturnState());
        }
    }
}
