using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ModelContextProtocol.Server;

namespace SequentialThinking
{
    [McpServerToolType]
    public class SequentialThinkingTools
    {
        // Session storage
        private static readonly Dictionary<string, SequentialThinkingSession> _sessions = new();

        [McpServerTool]
        [Description("Initiates a new sequential thinking process with a specified problem statement")]
        public static SequentialThinkingResponse InitiateThinking(string problemStatement, string sessionId = null)
        {
            sessionId ??= Guid.NewGuid().ToString();
            
            var session = new SequentialThinkingSession
            {
                SessionId = sessionId,
                ProblemStatement = problemStatement,
                Thoughts = new List<Thought>(),
                CurrentStage = ThinkingStage.ProblemDefinition,
                CreatedAt = DateTime.UtcNow
            };
            
            _sessions[sessionId] = session;
            
            return new SequentialThinkingResponse
            {
                SessionId = sessionId,
                Message = "Sequential thinking process initiated",
                CurrentStage = session.CurrentStage,
                NextRecommendedAction = StageRecommendations.GetForStage(session.CurrentStage)
            };
        }

        [McpServerTool]
        [Description("Adds a new thought to the sequential thinking process")]
        public static SequentialThinkingResponse AddThought(string sessionId, string content, ThinkingStage? stage = null)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
            {
                return new SequentialThinkingResponse { Error = "Session not found" };
            }
            
            var currentStage = stage ?? session.CurrentStage;
            
            var thought = new Thought
            {
                Id = session.Thoughts.Count + 1,
                Content = content,
                Stage = currentStage,
                CreatedAt = DateTime.UtcNow
            };
            
            session.Thoughts.Add(thought);
            session.CurrentStage = StageManager.DetermineNextStage(currentStage, session);
            
            return new SequentialThinkingResponse
            {
                SessionId = sessionId,
                Message = "Thought added successfully",
                CurrentStage = session.CurrentStage,
                NextRecommendedAction = StageRecommendations.GetForStage(session.CurrentStage)
            };
        }

        [McpServerTool]
        [Description("Retrieves the current state of the sequential thinking process")]
        public static SequentialThinkingSession GetSession(string sessionId)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
            {
                throw new ArgumentException("Session not found");
            }
            
            return session;
        }

        [McpServerTool]
        [Description("Generates a summary of the entire sequential thinking process")]
        public static ThinkingSummary GenerateSummary(string sessionId)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
            {
                throw new ArgumentException("Session not found");
            }
            
            return ThinkingAnalyzer.CreateSummary(session);
        }

        [McpServerTool]
        [Description("Revises an existing thought in the sequential thinking process")]
        public static SequentialThinkingResponse ReviseThought(string sessionId, int thoughtId, string newContent)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
            {
                return new SequentialThinkingResponse { Error = "Session not found" };
            }
            
            var thought = session.Thoughts.FirstOrDefault(t => t.Id == thoughtId);
            if (thought == null)
            {
                return new SequentialThinkingResponse { Error = "Thought not found" };
            }
            
            thought.Content = newContent;
            thought.RevisedAt = DateTime.UtcNow;
            
            return new SequentialThinkingResponse
            {
                SessionId = sessionId,
                Message = "Thought revised successfully",
                CurrentStage = session.CurrentStage,
                NextRecommendedAction = StageRecommendations.GetForStage(session.CurrentStage)
            };
        }

        [McpServerTool]
        [Description("Creates a new branch of thinking from a specific thought point")]
        public static SequentialThinkingResponse CreateThoughtBranch(string sessionId, int thoughtId, string branchDescription)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
            {
                return new SequentialThinkingResponse { Error = "Session not found" };
            }
            
            var parentThought = session.Thoughts.FirstOrDefault(t => t.Id == thoughtId);
            if (parentThought == null)
            {
                return new SequentialThinkingResponse { Error = "Parent thought not found" };
            }
            
            var branchSessionId = Guid.NewGuid().ToString();
            var branchSession = new SequentialThinkingSession
            {
                SessionId = branchSessionId,
                ProblemStatement = session.ProblemStatement,
                Thoughts = session.Thoughts.Where(t => t.Id <= thoughtId).ToList(),
                CurrentStage = parentThought.Stage,
                CreatedAt = DateTime.UtcNow,
                ParentSessionId = sessionId,
                BranchDescription = branchDescription
            };
            
            _sessions[branchSessionId] = branchSession;
            
            return new SequentialThinkingResponse
            {
                SessionId = branchSessionId,
                Message = $"New thought branch created from thought {thoughtId}",
                CurrentStage = branchSession.CurrentStage,
                NextRecommendedAction = StageRecommendations.GetForStage(branchSession.CurrentStage)
            };
        }

        [McpServerTool]
        [Description("Clears the current sequential thinking session")]
        public static SequentialThinkingResponse ClearSession(string sessionId)
        {
            if (!_sessions.ContainsKey(sessionId))
            {
                return new SequentialThinkingResponse { Error = "Session not found" };
            }
            
            _sessions.Remove(sessionId);
            
            return new SequentialThinkingResponse { Message = "Session cleared successfully" };
        }

        [McpServerTool]
        [Description("Evaluates the quality and coherence of the sequential thinking process")]
        public static ThinkingEvaluation EvaluateThinking(string sessionId)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
            {
                throw new ArgumentException("Session not found");
            }
            
            return ThinkingAnalyzer.Evaluate(session);
        }

        [McpServerTool]
        [Description("Suggests the next step to take in the sequential thinking process")]
        public static NextStepSuggestion SuggestNextStep(string sessionId)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
            {
                throw new ArgumentException("Session not found");
            }
            
            return ThinkingAnalyzer.GenerateNextStepSuggestion(session);
        }
    }
}
