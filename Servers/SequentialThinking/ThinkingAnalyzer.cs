using System;
using System.Collections.Generic;
using System.Linq;

namespace SequentialThinking
{
    public static class ThinkingAnalyzer
    {
        public static ThinkingSummary CreateSummary(SequentialThinkingSession session)
        {
            return new ThinkingSummary
            {
                SessionId = session.SessionId,
                ProblemStatement = session.ProblemStatement,
                ThoughtCount = session.Thoughts.Count,
                Duration = DateTime.UtcNow - session.CreatedAt,
                StageBreakdown = session.Thoughts
                    .GroupBy(t => t.Stage)
                    .ToDictionary(g => g.Key, g => g.Count()),
                KeyInsights = ExtractKeyInsights(session)
            };
        }

        public static ThinkingEvaluation Evaluate(SequentialThinkingSession session)
        {
            return new ThinkingEvaluation
            {
                SessionId = session.SessionId,
                Completeness = CalculateCompleteness(session),
                Coherence = CalculateCoherence(session),
                Depth = CalculateDepth(session),
                Strengths = IdentifyStrengths(session),
                WeakPoints = IdentifyWeakPoints(session),
                Recommendations = GenerateRecommendations(session)
            };
        }

        public static NextStepSuggestion GenerateNextStepSuggestion(SequentialThinkingSession session)
        {
            var currentStage = session.CurrentStage;
            
            return new NextStepSuggestion
            {
                SessionId = session.SessionId,
                CurrentStage = currentStage,
                SuggestedAction = StageRecommendations.GetForStage(currentStage),
                RelevantQuestions = StageRecommendations.GetRelevantQuestions(currentStage),
                PotentialApproaches = StageRecommendations.GetPotentialApproaches(currentStage)
            };
        }

        #region Helper Methods

        private static List<string> ExtractKeyInsights(SequentialThinkingSession session)
        {
            var insights = new List<string>();
            
            // Find the most significant thought from each stage
            var significantThoughts = session.Thoughts
                .GroupBy(t => t.Stage)
                .Select(g => g.OrderByDescending(t => t.Content.Length).FirstOrDefault())
                .Where(t => t != null);
            
            foreach (var thought in significantThoughts)
            {
                insights.Add($"[{thought.Stage}] {TruncateContent(thought.Content, 100)}");
            }
            
            return insights;
        }
        
        private static string TruncateContent(string content, int maxLength)
        {
            return content.Length <= maxLength ? content : content.Substring(0, maxLength) + "...";
        }
        
        private static double CalculateCompleteness(SequentialThinkingSession session)
        {
            // Check if all stages have at least one thought
            var stagesWithThoughts = session.Thoughts.Select(t => t.Stage).Distinct().Count();
            var totalStages = Enum.GetValues(typeof(ThinkingStage)).Length;
            
            return (double)stagesWithThoughts / totalStages;
        }
        
        private static double CalculateCoherence(SequentialThinkingSession session)
        {
            // Check if stages progress in a logical sequence
            bool hasOutOfOrderStages = false;
            var orderedThoughts = session.Thoughts.OrderBy(t => t.Id).ToList();
            
            for (int i = 1; i < orderedThoughts.Count; i++)
            {
                if ((int)orderedThoughts[i].Stage < (int)orderedThoughts[i-1].Stage - 1)
                {
                    hasOutOfOrderStages = true;
                    break;
                }
            }
            
            // Penalize if thoughts are out of order or too few
            double coherenceScore = hasOutOfOrderStages ? 0.7 : 1.0;
            if (session.Thoughts.Count < 5)
            {
                coherenceScore *= 0.8;
            }
            
            return coherenceScore;
        }
        
        private static double CalculateDepth(SequentialThinkingSession session)
        {
            // Calculate average thought length as proxy for depth
            if (!session.Thoughts.Any()) return 0;
            
            var averageThoughtLength = session.Thoughts.Average(t => t.Content.Length);
            
            // Normalize to a 0-1 scale (200 chars = good depth)
            return Math.Min(1.0, averageThoughtLength / 200.0);
        }
        
        private static List<string> IdentifyStrengths(SequentialThinkingSession session)
        {
            var strengths = new List<string>();
            
            if (session.Thoughts.Count >= 10)
            {
                strengths.Add("Thorough thought process with multiple thoughts");
            }
            
            if (session.Thoughts.Any(t => t.RevisedAt.HasValue))
            {
                strengths.Add("Demonstrates revision and refinement of thinking");
            }
            
            if (session.Thoughts.GroupBy(t => t.Stage).Count() >= 4)
            {
                strengths.Add("Covers multiple stages of the thinking process");
            }
            
            if (CalculateDepth(session) > 0.8)
            {
                strengths.Add("Shows depth in thought exploration");
            }
            
            // Default strength if none identified
            if (strengths.Count == 0)
            {
                strengths.Add("The thinking process has been initialized");
            }
            
            return strengths;
        }
        
        private static List<string> IdentifyWeakPoints(SequentialThinkingSession session)
        {
            var weakPoints = new List<string>();
            
            if (session.Thoughts.Count < 5)
            {
                weakPoints.Add("Limited number of thoughts recorded");
            }
            
            var stagesWithThoughts = session.Thoughts.Select(t => t.Stage).Distinct().Count();
            if (stagesWithThoughts < 3)
            {
                weakPoints.Add("Limited progression through thinking stages");
            }
            
            if (CalculateDepth(session) < 0.5)
            {
                weakPoints.Add("Thoughts could be more detailed and substantive");
            }
            
            // Default if no weak points identified
            if (weakPoints.Count == 0)
            {
                weakPoints.Add("No significant weaknesses identified");
            }
            
            return weakPoints;
        }
        
        private static List<string> GenerateRecommendations(SequentialThinkingSession session)
        {
            var recommendations = new List<string>();
            var weakPoints = IdentifyWeakPoints(session);
            
            // Generate recommendations based on weak points
            if (weakPoints.Contains("Limited number of thoughts recorded"))
            {
                recommendations.Add("Add more thoughts to explore the problem from different angles");
            }
            
            if (weakPoints.Contains("Limited progression through thinking stages"))
            {
                recommendations.Add("Advance through more stages of the thinking process");
            }
            
            if (weakPoints.Contains("Thoughts could be more detailed and substantive"))
            {
                recommendations.Add("Provide more detailed analysis in your thoughts");
            }
            
            // Add stage-specific recommendation
            recommendations.Add(StageRecommendations.GetForStage(session.CurrentStage));
            
            return recommendations;
        }
        
        #endregion
    }
}
