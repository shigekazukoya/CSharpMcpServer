using System.ComponentModel;

namespace CommandTools.Models;

/// <summary>
/// コマンド実行の結果
/// </summary>
public class ShellResult
{
    /// <summary>
    /// 標準出力
    /// </summary>
    [Description("コマンド実行の標準出力結果")]
    public string Stdout { get; set; } = "";

    /// <summary>
    /// 標準エラー出力
    /// </summary>
    [Description("コマンド実行の標準エラー出力結果")]
    public string Stderr { get; set; } = "";

    /// <summary>
    /// コマンドが中断されたかどうか
    /// </summary>
    [Description("コマンドがタイムアウトなどで中断された場合はtrue")]
    public bool Interrupted { get; set; }

    /// <summary>
    /// 出力が画像であるかどうか
    /// </summary>
    [Description("出力が画像データの場合はtrue")]
    public bool IsImage { get; set; }

    /// <summary>
    /// サンドボックス内で実行されたかどうか
    /// </summary>
    [Description("コマンドがサンドボックス環境で実行された場合はtrue")]
    public bool Sandbox { get; set; }
}