using System.ComponentModel;

namespace CommandTools.Models;

/// <summary>
/// シェルコマンドを実行するためのオプション
/// </summary>
public class ShellOptions
{
    /// <summary>
    /// 実行するコマンド
    /// </summary>
    [Description("実行するコマンドを指定します")]
    public string Command { get; set; } = "";

    /// <summary>
    /// コマンドの説明 (5-10語の簡潔な説明)
    /// </summary>
    [Description("コマンドの簡潔な説明（5-10語程度）を指定します")]
    public string Description { get; set; } = "";

    /// <summary>
    /// タイムアウト（ミリ秒単位、最大600000）
    /// </summary>
    [Description("コマンド実行のタイムアウト時間をミリ秒単位で指定します（最大600000）")]
    public int? Timeout { get; set; }
}