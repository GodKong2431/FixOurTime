using UnityEngine;

public static class Utill
{
    /// <summary>
    /// 스트링배열의 속성들이 null이나 비어있는지 체크하는 메서드
    /// </summary>
    /// <param name="values">체크할 문자열 배열</param>
    /// <returns>null이나 비어있다면 true 값이 다 들어있다면 false</returns>
    public static bool IsAllStringNullOrEmpty(string[] values)
    {
        foreach (string value in values) 
        {
            if (string.IsNullOrEmpty(value))
                return true;
        }
        return false;
    }
}
