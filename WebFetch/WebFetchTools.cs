using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WebFetchTools;

[McpServerToolType]
public static class WebFetchTools
{
    [McpServerTool, Description("Fetches URL content and extracts main text by removing ads, navigation, and scripts. Uses semantic HTML elements to identify main content.")]
    public static async Task<string> ExtractMainContentFromUrl(string url)
    {
        // HTMLを取得
        string html = await FetchHtml(url);

        // HtmlAgilityPackを使用してHTMLを解析
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // 不要な要素を削除
        RemoveUnwantedElements(doc);

        // メインコンテンツを抽出
        string mainContent = ExtractMainContent(doc);

        // テキストをクリーンアップして返す
        mainContent = CleanupText(mainContent);

        return mainContent;
    }

    static async Task<string> FetchHtml(string url)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            return await client.GetStringAsync(url);
        }
    }

    static void RemoveUnwantedElements(HtmlDocument doc)
    {
        // 広告、ナビゲーション、スクリプトなど不要な要素を削除
        var nodesToRemove = doc.DocumentNode.SelectNodes("//script|//style|//nav|//header|//footer|//iframe|//noscript|//aside|//form|//comment()|//div[contains(@class, 'ad')]|//div[contains(@class, 'comment')]|//div[contains(@class, 'sidebar')]");

        if (nodesToRemove != null)
        {
            foreach (var node in nodesToRemove.ToList())
            {
                node.Remove();
            }
        }
    }

    static string ExtractMainContent(HtmlDocument doc)
    {
        // メインコンテンツを抽出する試行順序
        // 1. article要素を確認
        var article = doc.DocumentNode.SelectSingleNode("//article");
        if (article != null)
        {
            return article.InnerText;
        }

        // 2. main要素を確認
        var main = doc.DocumentNode.SelectSingleNode("//main");
        if (main != null)
        {
            return main.InnerText;
        }

        // 3. contentという名前またはクラスを持つ要素を確認
        var content = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'content')]|//div[contains(@id, 'content')]|//div[contains(@class, 'post')]|//div[contains(@class, 'article')]");
        if (content != null)
        {
            return content.InnerText;
        }

        // 4. 最も長いテキストを含むdiv要素を確認
        string longestText = "";
        var divs = doc.DocumentNode.SelectNodes("//div");
        if (divs != null)
        {
            foreach (var div in divs)
            {
                string text = div.InnerText.Trim();
                if (text.Length > longestText.Length)
                {
                    longestText = text;
                }
            }
            if (longestText.Length > 0)
            {
                return longestText;
            }
        }

        // 5. body全体を最終手段として返す
        return doc.DocumentNode.SelectSingleNode("//body").InnerText;
    }

    static string CleanupText(string text)
    {
        // 連続する空白を削除
        text = Regex.Replace(text, @"\s+", " ");

        // 空の行を削除
        text = Regex.Replace(text, @"\n\s*\n", "\n");

        // HTMLエンティティをデコード
        text = System.Net.WebUtility.HtmlDecode(text);

        return text.Trim();
    }


    //[McpServerTool, Description("Fetches content from a specified URL using HTTP GET.")]
    //public static async Task<string> FetchUrl(string url, int timeoutInSeconds = 30)
    //{
    //    ValidateUrl(url);
        
    //    using var client = CreateHttpClient(timeoutInSeconds);
    //    try
    //    {
    //        var response = await client.GetAsync(url);
    //        response.EnsureSuccessStatusCode();
    //        return await response.Content.ReadAsStringAsync();
    //    }
    //    catch (HttpRequestException ex)
    //    {
    //        throw new Exception($"Failed to fetch URL: {ex.Message}", ex);
    //    }
    //    catch (TaskCanceledException)
    //    {
    //        throw new Exception($"Request timed out after {timeoutInSeconds} seconds.");
    //    }
    //}

    private static HttpClient CreateHttpClient(int timeoutInSeconds)
    {
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(timeoutInSeconds)
        };
        
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 WebFetchTools/1.0");
        
        return client;
    }

    private static void ValidateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL cannot be empty or null.", nameof(url));
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("Invalid URL format.", nameof(url));
        }

        if (uri.Scheme != "http" && uri.Scheme != "https")
        {
            throw new ArgumentException("Only HTTP and HTTPS protocols are supported.", nameof(url));
        }
    }
}

