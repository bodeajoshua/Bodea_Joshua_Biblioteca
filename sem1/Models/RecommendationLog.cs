// Models/RecommendationLog.cs
using System;

public class RecommendationLog
{
    public int ID { get; set; }
    public int ReaderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string BookIds { get; set; } = ""; 
}