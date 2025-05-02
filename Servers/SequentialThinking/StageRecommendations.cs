using System.Collections.Generic;

namespace SequentialThinking
{
    public static class StageRecommendations
    {
        public static string GetForStage(ThinkingStage stage)
        {
            return stage switch
            {
                ThinkingStage.ProblemDefinition => "Define the problem clearly and identify key constraints or requirements",
                ThinkingStage.Research => "Gather relevant information and explore existing solutions or approaches",
                ThinkingStage.Analysis => "Break down the problem into components and analyze each part systematically",
                ThinkingStage.Synthesis => "Combine insights and develop potential solutions or frameworks",
                ThinkingStage.Conclusion => "Evaluate solutions and select the most promising approach",
                ThinkingStage.Reflection => "Consider limitations, alternative perspectives, and areas for improvement",
                _ => "Continue the thinking process"
            };
        }

        public static List<string> GetRelevantQuestions(ThinkingStage stage)
        {
            return stage switch
            {
                ThinkingStage.ProblemDefinition => new List<string>
                {
                    "What are the key aspects of this problem?",
                    "What constraints or requirements must be considered?",
                    "How would you know if this problem is solved?"
                },
                ThinkingStage.Research => new List<string>
                {
                    "What existing solutions or approaches are relevant?",
                    "What data or information would be helpful?",
                    "What experts or resources could provide insights?"
                },
                ThinkingStage.Analysis => new List<string>
                {
                    "What are the component parts of this problem?",
                    "What patterns or relationships can be identified?",
                    "What are the underlying causes or mechanisms?"
                },
                ThinkingStage.Synthesis => new List<string>
                {
                    "How can the insights be combined into a solution?",
                    "What alternative approaches could be considered?",
                    "How might different perspectives inform the solution?"
                },
                ThinkingStage.Conclusion => new List<string>
                {
                    "Which solution best addresses the original problem?",
                    "What criteria should be used to evaluate success?",
                    "What are the next steps for implementation?"
                },
                ThinkingStage.Reflection => new List<string>
                {
                    "What limitations or assumptions influenced the thinking?",
                    "What could be improved in the thinking process?",
                    "What additional perspectives should be considered?"
                },
                _ => new List<string> { "What is the next step in your thinking?" }
            };
        }

        public static List<string> GetPotentialApproaches(ThinkingStage stage)
        {
            var approaches = new List<string>();
            
            switch (stage)
            {
                case ThinkingStage.ProblemDefinition:
                    approaches.Add("Reframe the problem from different perspectives");
                    approaches.Add("Break down the problem into smaller, manageable components");
                    approaches.Add("Define clear success criteria for the solution");
                    break;
                case ThinkingStage.Research:
                    approaches.Add("Consider analogous problems and their solutions");
                    approaches.Add("Investigate existing research or literature on the topic");
                    approaches.Add("Gather data or examples related to the problem");
                    break;
                case ThinkingStage.Analysis:
                    approaches.Add("Identify patterns or trends in the research");
                    approaches.Add("Analyze the relationships between different components");
                    approaches.Add("Consider root causes or underlying mechanisms");
                    break;
                case ThinkingStage.Synthesis:
                    approaches.Add("Combine insights to generate potential solutions");
                    approaches.Add("Develop a framework or model for addressing the problem");
                    approaches.Add("Integrate different perspectives or approaches");
                    break;
                case ThinkingStage.Conclusion:
                    approaches.Add("Evaluate solutions against defined criteria");
                    approaches.Add("Consider implementation strategies for the chosen solution");
                    approaches.Add("Identify potential challenges or obstacles");
                    break;
                case ThinkingStage.Reflection:
                    approaches.Add("Reflect on the strengths and limitations of the process");
                    approaches.Add("Consider what could be improved in future thinking");
                    approaches.Add("Identify additional perspectives or angles to explore");
                    break;
            }
            
            return approaches;
        }
    }
}
