using CSharpMcpServer.VisualStudio;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VisualStudio
{
    [McpServerToolType]
    public class VisualStudioTools
    {
        private readonly VisualStudioInstanceManager _vsTools;

        public VisualStudioTools()
        {
            _vsTools = new VisualStudioInstanceManager();
        }

        [McpServerTool, Description("実行中のすべてのVisual Studioインスタンス情報を取得します")]
        public string GetAllVSInfo()
        {
            var allInfo = _vsTools.GetAllVisualStudioInfo();
            return JsonSerializer.Serialize(allInfo, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }

        [McpServerTool, Description("アクティブなソリューションの情報を取得します")]
        public string GetActiveSolution()
        {
            var instances = _vsTools.GetVisualStudioInstances();
            if (instances.Count == 0)
                return "実行中のVisual Studioインスタンスが見つかりません";

            var solutionInfo = _vsTools.GetSolutionInfo(instances[0]);
            if (solutionInfo == null)
                return "ソリューションが開かれていないか、取得できませんでした";

            return JsonSerializer.Serialize(solutionInfo, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }

        [McpServerTool, Description("現在選択されているファイルの内容を取得します")]
        public string GetActiveFileContent()
        {
            var instances = _vsTools.GetVisualStudioInstances();
            if (instances.Count == 0)
                return "実行中のVisual Studioインスタンスが見つかりません";

            var results = new StringBuilder();

            // すべてのインスタンスを処理
            for (int i = 0; i < instances.Count; i++)
            {
                var instance = instances[i];
                var activeDoc = _vsTools.GetActiveDocument(instance);

                // インスタンス情報のヘッダーを追加
                results.AppendLine($"// インスタンス {i + 1}:");

                if (activeDoc == null)
                {
                    results.AppendLine("アクティブなドキュメントが見つかりません");
                    results.AppendLine();
                    continue;
                }

                try
                {
                    if (File.Exists(activeDoc))
                    {
                        var content = File.ReadAllText(activeDoc);
                        results.AppendLine($"{activeDoc}");
                    }
                    else
                    {
                        results.AppendLine($"ファイル '{activeDoc}' が見つかりません");
                    }
                }
                catch (Exception ex)
                {
                    results.AppendLine($"ファイルの読み込み中にエラーが発生しました: {ex.Message}");
                }

                // インスタンス間の区切り
                if (i < instances.Count - 1)
                {
                    results.AppendLine();
                    results.AppendLine("// ----------------------------------------");
                    results.AppendLine();
                }
            }

            return results.ToString();
        }

        [McpServerTool, Description("Visual Studioで開いているすべてのファイル情報を取得します")]
        public string GetOpenFiles()
        {
            var instances = _vsTools.GetVisualStudioInstances();
            if (instances.Count == 0)
                return "実行中のVisual Studioインスタンスが見つかりません";

            var result = new Dictionary<string, object>();

            // すべてのインスタンスを処理
            for (int i = 0; i < instances.Count; i++)
            {
                var instance = instances[i];
                var instanceInfo = _vsTools.GetInstanceInfo(instance);
                var solutionInfo = _vsTools.GetSolutionInfo(instance);
                var openDocs = _vsTools.GetOpenDocuments(instance);

                // インスタンス情報と開いているファイルを格納
                var instanceData = new Dictionary<string, object>
                {
                    ["InstanceInfo"] = instanceInfo,
                    ["OpenDocuments"] = openDocs ?? new List<string>()
                };

                // ソリューション名をキーとして使用（なければインスタンス番号を使用）
                string key = solutionInfo != null && !string.IsNullOrEmpty(solutionInfo.FullName)
                            ? Path.GetFileNameWithoutExtension(solutionInfo.FullName)
                            : $"インスタンス {i + 1} (ソリューションなし)";

                // ソリューション情報も追加
                instanceData["SolutionInfo"] = solutionInfo;

                result[key] = instanceData;
            }

            if (result.Count == 0)
                return "開いているファイルが見つかりません";

            return JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
    }
}
