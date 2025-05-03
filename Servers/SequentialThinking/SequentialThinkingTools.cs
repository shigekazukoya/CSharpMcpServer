using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace SequentialThinking
{
    public class ThoughtData
    {
        public string Thought { get; set; }
        public int ThoughtNumber { get; set; }
        public int TotalThoughts { get; set; }
        public bool IsRevision { get; set; }
        public int? RevisesThought { get; set; }
        public int? BranchFromThought { get; set; }
        public string BranchId { get; set; }
        public bool? NeedsMoreThoughts { get; set; }
        public bool NextThoughtNeeded { get; set; }

        public string ToStringFormattedText()
        {
            string prefix;
            string context = "";

            if (this.IsRevision)
            {
                prefix = "🔄 Revision";
                context = $" (revising thought {this.RevisesThought})";
            }
            else if (this.BranchFromThought.HasValue)
            {
                prefix = "🌿 Branch";
                context = $" (from thought {this.BranchFromThought}, ID: {this.BranchId})";
            }
            else
            {
                prefix = "💭 Thought";
            }

            string header = $"{prefix} {this.ThoughtNumber}/{this.TotalThoughts}{context}";
            int borderLength = Math.Max(header.Length, this.Thought.Length) + 4;
            string border = new string('─', borderLength);

            return $@"
┌{border}┐
│ {header.PadRight(borderLength - 2)} │
├{border}┤
│ {this.Thought.PadRight(borderLength - 2)} │
└{border}┘";
        }

    }

    public class SequentialThinkingServer
    {
        private List<ThoughtData> _thoughtHistory = new List<ThoughtData>();
        private Dictionary<string, List<ThoughtData>> _branches = new Dictionary<string, List<ThoughtData>>();

        public object ProcessThought(ThoughtData validatedInput)
        {
            try
            {
                if (validatedInput.ThoughtNumber > validatedInput.TotalThoughts)
                {
                    validatedInput.TotalThoughts = validatedInput.ThoughtNumber;
                }

                _thoughtHistory.Add(validatedInput);

                if (validatedInput.BranchFromThought.HasValue && !string.IsNullOrEmpty(validatedInput.BranchId))
                {
                    if (!_branches.ContainsKey(validatedInput.BranchId))
                    {
                        _branches[validatedInput.BranchId] = new List<ThoughtData>();
                    }
                    _branches[validatedInput.BranchId].Add(validatedInput);
                }

                return new
                {
                    thoughtNumber = validatedInput.ThoughtNumber,
                    totalThoughts = validatedInput.TotalThoughts,
                    nextThoughtNeeded = validatedInput.NextThoughtNeeded,
                    branches = _branches.Keys.ToArray(),
                    thoughtHistoryLength = _thoughtHistory.Count
                };
            }
            catch (Exception ex)
            {
                
                var errorResponse = new[]
                {
                    new
                    {
                        type = "text",
                        text = JsonSerializer.Serialize(new
                        {
                            error = ex.Message,
                            status = "failed"
                        }, new JsonSerializerOptions { WriteIndented = true })
                    }
                };

                return new
                {
                    content = errorResponse,
                    isError = true
                };
            }
        }
    }
    
    [McpServerToolType]
    public static class SequentialThinkingTool
    {
        private static readonly SequentialThinkingServer _thinkingServer;
        
        static SequentialThinkingTool()
        {
            _thinkingServer = new SequentialThinkingServer();
        }
        
        [McpServerTool]
        [Description("A detailed tool for dynamic and reflective problem-solving through thoughts.\nThis tool helps analyze problems through a flexible thinking process that can adapt and evolve.\nEach thought can build on, question, or revise previous insights as understanding deepens.")]
        public static object SequentialThinking(ThoughtData arguments)
        {
            return _thinkingServer.ProcessThought(arguments);
        }
    }
}
