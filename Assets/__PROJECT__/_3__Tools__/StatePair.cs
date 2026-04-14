using UnityEngine;

// 이전 값과 현재 값을 가지는 유틸리티 클래스
public class IntPair
{
    public int prev = 0;
    public int curr = 0;

    public bool IsDifferent()
    {
        return prev != curr;
    }

    public int Diff()
    {
        return curr - prev;
    }

    public void MakeSame()
    {
        prev = curr;
    }

    public bool IsCurrentEqualGreater()
    {
        return prev <= curr;
    }

    public bool IsCurrentEqualLess()
    {
        return prev >= curr;
    }

    public bool IsCurrentGreater()
    {
        return prev < curr;
    }

    public bool IsCurrentLess()
    {
        return prev > curr;
    }
}