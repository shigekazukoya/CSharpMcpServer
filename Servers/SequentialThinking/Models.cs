using System;
using System.Collections.Generic;

namespace SequentialThinking
{
    public enum ThinkingStage
    {
        ProblemDefinition,
        Research,
        Analysis,
        Synthesis,
        Conclusion,
        Reflection
    }
    
    public class Thought
    {
        public int Id { get; set; }
        
        public string Content { get; set; }
        
        public ThinkingStage Stage { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? RevisedAt { get; set; }
    }
    
    public class SequentialThinkingSession
    {
        public string SessionId { get; set; }
        
        public string ProblemStatement { get; set; }
        
        public List<Thought> Thoughts { get; set; } = new List<Thought>();
        
        public ThinkingStage CurrentStage { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public string ParentSessionId { get; set; }
        
        public string BranchDescription { get; set; }
    }
    
    public class SequentialThinkingResponse
    {
        public string SessionId { get; set; }
        
        public string Message { get; set; }
        
        public string Error { get; set; }
        
        public ThinkingStage CurrentStage { get; set; }
        
        public string NextRecommendedAction { get; set; }
    }
    
    public class ThinkingSummary
    {
        public string SessionId { get; set; }
        
        public string ProblemStatement { get; set; }
        
        public int ThoughtCount { get; set; }
        
        public TimeSpan Duration { get; set; }
        
        public Dictionary<ThinkingStage, int> StageBreakdown { get; set; }
        
        public List<string> KeyInsights { get; set; }
    }
    
    public class ThinkingEvaluation
    {
        public string SessionId { get; set; }
        
        public double Completeness { get; set; }
        
        public double Coherence { get; set; }
        
        public double Depth { get; set; }
        
        public List<string> Strengths { get; set; }
        
        public List<string> WeakPoints { get; set; }
        
        public List<string> Recommendations { get; set; }
    }
    
    public class NextStepSuggestion
    {
        public string SessionId { get; set; }
        
        public ThinkingStage CurrentStage { get; set; }
        
        public string SuggestedAction { get; set; }
        
        public List<string> RelevantQuestions { get; set; }
        
        public List<string> PotentialApproaches { get; set; }
    }
}
