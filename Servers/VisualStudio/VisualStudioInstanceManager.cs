using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;
using EnvDTE80;

namespace CSharpMcpServer.VisualStudio
{
    /// <summary>
    /// Visual Studioの情報を取得するためのツールクラス
    /// </summary>
    public class VisualStudioInstanceManager
    {
        // ROT (Running Object Table)からVisual Studioのインスタンスを取得するためのWin32 API
        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

        /// <summary>
        /// Visual Studioのすべての実行中インスタンスを取得
        /// </summary>
        /// <returns>実行中のVisual Studioインスタンスのリスト</returns>
        public List<DTE2> GetVisualStudioInstances()
        {
            List<DTE2> instances = new List<DTE2>();

            try
            {
                IRunningObjectTable rot;
                IEnumMoniker enumMoniker;
                IBindCtx bindCtx;

                if (GetRunningObjectTable(0, out rot) != 0)
                    return instances;

                if (CreateBindCtx(0, out bindCtx) != 0)
                    return instances;

                rot.EnumRunning(out enumMoniker);
                enumMoniker.Reset();

                IntPtr fetched = IntPtr.Zero;
                IMoniker[] moniker = new IMoniker[1];

                while (enumMoniker.Next(1, moniker, fetched) == 0)
                {
                    string displayName;
                    moniker[0].GetDisplayName(bindCtx, null, out displayName);

                    if (displayName.Contains("VisualStudio.DTE"))
                    {
                        object comObject;
                        rot.GetObject(moniker[0], out comObject);

                        if (comObject is DTE2 dte)
                        {
                            instances.Add(dte);
                        }
                    }

                    Marshal.ReleaseComObject(moniker[0]);
                }

                Marshal.ReleaseComObject(enumMoniker);
                Marshal.ReleaseComObject(rot);
                Marshal.ReleaseComObject(bindCtx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting Visual Studio instances: {ex.Message}");
            }

            return instances;
        }

        public VisualStudioInstanceInfo GetInstanceInfo(DTE2 dte)
        {
            if (dte == null)
                return null;

            try
            {
                var info = new VisualStudioInstanceInfo
                {
                    DisplayName = dte.Name,
                    Version = dte.Version,
                    Edition = dte.Edition,
                    FullName = dte.FullName,
                    Mode = dte.Mode.ToString()
                };

                return info;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting instance info: {ex.Message}");
                return null;
            }
        }

        public SolutionInfo GetSolutionInfo(DTE2 dte)
        {
            if (dte == null || dte.Solution == null)
                return null;

            try
            {
                Solution2 solution = dte.Solution as Solution2;
                if (solution == null)
                    return null;

                var solutionInfo = new SolutionInfo
                {
                    FullName = solution.FullName,
                    Projects = new List<string>()
                };

                foreach (Project project in solution.Projects)
                {
                    // プロジェクト情報を追加
                    var projectInfo = GetProjectInfo(project);
                    if (projectInfo != null)
                    {
                        solutionInfo.Projects.Add(projectInfo);
                    }
                }

                return solutionInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting solution info: {ex.Message}");
                return null;
            }
        }

        private string GetProjectInfo(Project project)
        {
            if (project == null)
                return null;

            try
            {
                return project.FullName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting project info: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// プロジェクトアイテム情報を取得（再帰的に処理）
        /// </summary>
        /// <param name="item">プロジェクトアイテム</param>
        /// <returns>プロジェクトアイテム情報</returns>
        private ProjectItemInfo GetProjectItemInfo(ProjectItem item)
        {
            if (item == null)
                return null;

            try
            {
                var itemInfo = new ProjectItemInfo
                {
                    Name = item.Name,
                    SubItems = new List<ProjectItemInfo>()
                };

                try
                {
                    // ファイルパスの取得を試みる
                    if (item.FileCount > 0)
                    {
                        string filePath = null;
                        try
                        {
                            filePath = item.FileNames[0];
                        }
                        catch { }

                        itemInfo.FilePath = filePath;
                    }

                    // サブアイテムの処理
                    if (item.ProjectItems != null)
                    {
                        foreach (ProjectItem subItem in item.ProjectItems)
                        {
                            var subItemInfo = GetProjectItemInfo(subItem);
                            if (subItemInfo != null)
                            {
                                itemInfo.SubItems.Add(subItemInfo);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing sub-items: {ex.Message}");
                }

                return itemInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting project item info: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 現在開いているドキュメントの情報を取得
        /// </summary>
        /// <param name="dte">Visual StudioのDTEインスタンス</param>
        /// <returns>開いているドキュメントのリスト</returns>
        public List<string> GetOpenDocuments(DTE2 dte)
        {
            if (dte == null)
                return new List<string>();

            var documents = new List<string>();

            try
            {
                foreach (Document doc in dte.Documents)
                {
                    try
                    {
                        var docInfo = doc.FullName;
                        documents.Add(docInfo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing document: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting open documents: {ex.Message}");
            }

            return documents;
        }

        public string GetActiveDocument(DTE2 dte)
        {
            if (dte == null || dte.ActiveDocument == null)
                return null;

            try
            {
                var doc = dte.ActiveDocument;
                return doc.FullName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting active document: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 特定のVisual Studioインスタンスからすべての情報を取得
        /// </summary>
        /// <param name="dte">Visual StudioのDTEインスタンス</param>
        /// <returns>Visual Studioの完全な情報</returns>
        public VisualStudioInfo GetAllInfo(DTE2 dte)
        {
            if (dte == null)
                return null;

            try
            {
                var vsInfo = new VisualStudioInfo
                {
                    InstanceInfo = GetInstanceInfo(dte),
                    SolutionInfo = GetSolutionInfo(dte),
                    OpenDocuments = GetOpenDocuments(dte),
                    ActiveDocument = GetActiveDocument(dte)
                };

                return vsInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all VS info: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// すべてのVisual Studioインスタンス情報を取得
        /// </summary>
        /// <returns>すべてのインスタンス情報のリスト</returns>
        public List<VisualStudioInfo> GetAllVisualStudioInfo()
        {
            var result = new List<VisualStudioInfo>();
            
            try
            {
                var instances = GetVisualStudioInstances();
                foreach (var instance in instances)
                {
                    try
                    {
                        var info = GetAllInfo(instance);
                        if (info != null)
                        {
                            result.Add(info);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing VS instance: {ex.Message}");
                    }
                    finally
                    {
                        // COMオブジェクトのリリース
                        try
                        {
                            Marshal.ReleaseComObject(instance);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllVisualStudioInfo: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// ドキュメントの内容を取得（複数行の場合も対応）
        /// </summary>
        /// <param name="dte">Visual StudioのDTEインスタンス</param>
        /// <param name="document">ドキュメント</param>
        /// <returns>ドキュメントの内容</returns>
        public string GetDocumentContent(DTE2 dte, Document document)
        {
            if (dte == null || document == null)
                return null;

            try
            {
                TextDocument textDoc = document.Object("TextDocument") as TextDocument;
                if (textDoc != null)
                {
                    EditPoint startPoint = textDoc.StartPoint.CreateEditPoint();
                    EditPoint endPoint = textDoc.EndPoint.CreateEditPoint();
                    return startPoint.GetText(endPoint);
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting document content: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// アクティブなドキュメントの内容を取得
        /// </summary>
        /// <param name="dte">Visual StudioのDTEインスタンス</param>
        /// <returns>アクティブなドキュメントの内容</returns>
        public string GetActiveDocumentContent(DTE2 dte)
        {
            if (dte == null || dte.ActiveDocument == null)
                return null;

            return GetDocumentContent(dte, dte.ActiveDocument);
        }

        /// <summary>
        /// ソリューション内の特定の拡張子を持つファイルを検索
        /// </summary>
        /// <param name="dte">Visual StudioのDTEインスタンス</param>
        /// <param name="extension">検索する拡張子（.cs, .vb など）</param>
        /// <returns>見つかったファイルのパスのリスト</returns>
        public List<string> FindFilesByExtension(DTE2 dte, string extension)
        {
            var files = new List<string>();
            
            if (dte == null || dte.Solution == null || string.IsNullOrEmpty(extension))
                return files;

            try
            {
                Solution2 solution = dte.Solution as Solution2;
                if (solution == null)
                    return files;

                // 拡張子が.で始まっていない場合は追加
                if (!extension.StartsWith("."))
                    extension = "." + extension;
                
                // 検索処理は別のヘルパーメソッドに実装
                SearchFilesInSolution(solution, extension, files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding files by extension: {ex.Message}");
            }
            
            return files;
        }
        
        private void SearchFilesInSolution(Solution2 solution, string extension, List<string> files)
        {
            foreach (Project project in solution.Projects)
            {
                try
                {
                    SearchFilesInProject(project, extension, files);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error searching in project {project.Name}: {ex.Message}");
                }
            }
        }
        
        private void SearchFilesInProject(Project project, string extension, List<string> files)
        {
            if (project == null)
                return;
                
            try
            {
                // ソリューションフォルダの場合は、含まれるプロジェクトに対して再帰的に検索
                if (project.Kind == "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}")
                {
                    foreach (ProjectItem item in project.ProjectItems)
                    {
                        if (item.SubProject != null)
                        {
                            SearchFilesInProject(item.SubProject, extension, files);
                        }
                    }
                }
                else
                {
                    // 通常のプロジェクトの場合は、すべてのプロジェクトアイテムを検索
                    SearchFilesInProjectItems(project.ProjectItems, extension, files);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching in project {project.Name}: {ex.Message}");
            }
        }
        
        private void SearchFilesInProjectItems(ProjectItems items, string extension, List<string> files)
        {
            if (items == null)
                return;
                
            foreach (ProjectItem item in items)
            {
                try
                {
                    // ファイルがある場合はチェック
                    if (item.FileCount > 0)
                    {
                        string filePath = null;
                        try
                        {
                            filePath = item.FileNames[0];
                            if (!string.IsNullOrEmpty(filePath) && 
                                filePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                            {
                                files.Add(filePath);
                            }
                        }
                        catch { }
                    }

                    // サブアイテムも再帰的に検索
                    if (item.ProjectItems != null && item.ProjectItems.Count > 0)
                    {
                        SearchFilesInProjectItems(item.ProjectItems, extension, files);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing project item: {ex.Message}");
                }
            }
        }
    }

    #region データモデルクラス

    /// <summary>
    /// Visual Studioの完全な情報
    /// </summary>
    public class VisualStudioInfo
    {
        public VisualStudioInstanceInfo InstanceInfo { get; set; }
        public SolutionInfo SolutionInfo { get; set; }
        public List<string> OpenDocuments { get; set; }
        public string ActiveDocument { get; set; }
    }

    /// <summary>
    /// Visual Studioインスタンスの基本情報
    /// </summary>
    public class VisualStudioInstanceInfo
    {
        public string DisplayName { get; set; }
        public string Version { get; set; }
        public string Edition { get; set; }
        public string FullName { get; set; }
        public string Mode { get; set; }
    }

    public class SolutionInfo
    {
        public string FullName { get; set; }
        public List<string> Projects { get; set; }
    }

    public class ProjectItemInfo
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public List<ProjectItemInfo> SubItems { get; set; }
    }

    #endregion
}
