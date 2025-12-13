using System.Collections;
using UnityEngine;

public class AutoMoveGear : MoveGear
{
    private void Start()
    {
        StartCoroutine(AutoMove());
    }

    IEnumerator AutoMove()
    {
        while (true)
        {
            MoveNextPoint();
            ChangeNextPoint();

            yield return null;
        }
    }
}
