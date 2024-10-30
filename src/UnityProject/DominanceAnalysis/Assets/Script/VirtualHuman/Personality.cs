using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Personality : MonoBehaviour
{
    [Range(0, 4)]
    public int paScore = 0; // Assured-Dominant
    [Range(0, 4)]
    public int bcScore = 0; // Arrogant-Calculating
    [Range(0, 4)]
    public int deScore = 0; // Cold-Hearted
    [Range(0, 4)]
    public int fgScore = 0; // Aloof-Introverted
    [Range(0, 4)]
    public int hiScore = 0; // Unassured-Submissive
    [Range(0, 4)]
    public int jkScore = 0; // Unassuming-Ingenuous
    [Range(0, 4)]
    public int lmScore = 0; // Warm-Agreeable
    [Range(0, 4)]
    public int noScore = 0; // Gregarious-Extraverted

    public string personalityDesc = "Personality Description";

    public float backChannelFrequency = 0.1f;

    public int CalculateDominance()
    {
        var scores = new[]
        {
        new { Name = "pa", Score = paScore },
        new { Name = "bc", Score = bcScore },
        new { Name = "de", Score = deScore },
        new { Name = "fg", Score = fgScore },
        new { Name = "hi", Score = hiScore },
        new { Name = "jk", Score = jkScore },
        new { Name = "lm", Score = lmScore },
        new { Name = "no", Score = noScore }
    };

        // 가장 높은 점수들 구하기
        var highestScore = scores.Max(s => s.Score);
        var highestScores = scores.Where(s => s.Score == highestScore).ToArray();

        // 가중치를 계산하기 위한 합산 변수
        int totalScore = 0;

        // 각 점수에 대해 가중치 계산
        foreach (var score in highestScores)
        {
            if (score.Name == "pa" || score.Name == "bc" || score.Name == "no")
            {
                totalScore += 1; // pa, bc, no는 1점
            }
            else if (score.Name == "de" || score.Name == "lm")
            {
                totalScore += 0; // de, lm은 0점
            }
            else if (score.Name == "fg" || score.Name == "hi" || score.Name == "jk")
            {
                totalScore -= 1; // fg, hi, jk는 -1점
            }
        }

        // 최종 점수에 따라 dominance 계산
        if (totalScore > 0)
        {
            return 2; // 양수일 경우 high
        }
        else if (totalScore == 0)
        {
            return 1; // 0일 경우 mid
        }
        else
        {
            return 0; // 음수일 경우 low
        }
    }

    public Dictionary<string, int> GetPersonality()
    {
        Dictionary<string, int> dict = new Dictionary<string, int>
        {
            { "paScore", paScore },
            { "bcScore", bcScore },
            { "deScore", deScore },
            { "fgScore", fgScore },
            { "hiScore", hiScore },
            { "jkScore", jkScore },
            { "lmScore", lmScore },
            { "noScore", noScore }
        };

        return dict;
    }
}
