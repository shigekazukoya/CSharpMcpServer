using System;
using System.Linq;

namespace SequentialThinking
{
    public static class StageManager
    {
        public static ThinkingStage DetermineNextStage(ThinkingStage currentStage, SequentialThinkingSession session)
        {
            var thoughtsInCurrentStage = session.Thoughts.Count(t => t.Stage == currentStage);
            
            // Progress to next stage after sufficient thoughts in current stage
            if (thoughtsInCurrentStage >= 3)
            {
                // Advance to the next stage
                return currentStage switch
                {
                    ThinkingStage.ProblemDefinition => ThinkingStage.Research,
                    ThinkingStage.Research => ThinkingStage.Analysis,
                    ThinkingStage.Analysis => ThinkingStage.Synthesis,
                    ThinkingStage.Synthesis => ThinkingStage.Conclusion,
                    ThinkingStage.Conclusion => ThinkingStage.Reflection,
                    _ => currentStage
                };
            }
            
            return currentStage;
        }
    }
}
