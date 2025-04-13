chcp 65001
@echo off
setlocal enabledelayedexpansion

rem コマンドライン引数があれば使用し、なければ入力を求める
if "%~1" == "" (
    set /p feature_name=作る機能の名前を入力してください: 
) else (
    set feature_name=%~1
)

echo 機能名: %feature_name%

rem 入力された名前でフォルダを作成
mkdir %feature_name%
if %errorlevel% neq 0 (
    echo フォルダの作成に失敗しました。
    goto :end
)
echo フォルダ %feature_name% を作成しました。

rem csprojファイルの作成
cd %feature_name%
dotnet new console
if %errorlevel% neq 0 (
    echo プロジェクトの作成に失敗しました。
    cd ..
    goto :end
)
echo プロジェクト %feature_name%.csproj を作成しました。

rem 必要なパッケージの追加
dotnet add package ModelContextProtocol --prerelease
if %errorlevel% neq 0 (
    echo ModelContextProtocol パッケージの追加に失敗しました。
    cd ..
    goto :end
)
echo ModelContextProtocol パッケージを追加しました。

dotnet add package Microsoft.Extensions.Hosting
if %errorlevel% neq 0 (
    echo Microsoft.Extensions.Hosting パッケージの追加に失敗しました。
    cd ..
    goto :end
)
echo Microsoft.Extensions.Hosting パッケージを追加しました。

rem Program.csファイルの内容を置き換え
echo using Microsoft.Extensions.DependencyInjection;> Program.cs
echo using Microsoft.Extensions.Hosting;>> Program.cs
echo.>> Program.cs
echo var builder = Host.CreateEmptyApplicationBuilder(settings: null);>> Program.cs
echo.>> Program.cs
echo builder.Services.AddMcpServer()>> Program.cs
echo     .WithStdioServerTransport()>> Program.cs
echo     .WithToolsFromAssembly();>> Program.cs
echo.>> Program.cs
echo var app = builder.Build();>> Program.cs
echo.>> Program.cs
echo await app.RunAsync();>> Program.cs

echo Program.cs ファイルを更新しました。

rem ToolsファイルをNameTools.csという名前で作成
echo using ModelContextProtocol.Server;> %feature_name%Tools.cs
echo using System.ComponentModel;>> %feature_name%Tools.cs
echo namespace %feature_name%Tools;>> %feature_name%Tools.cs
echo [McpServerToolType]>> %feature_name%Tools.cs
echo public static class %feature_name%Tools>> %feature_name%Tools.cs
echo {>> %feature_name%Tools.cs
echo     [McpServerTool, Description("")]>> %feature_name%Tools.cs
echo     public static void %feature_name%()>> %feature_name%Tools.cs
echo     {>> %feature_name%Tools.cs
echo     }>> %feature_name%Tools.cs
echo }>> %feature_name%Tools.cs

echo %feature_name%Tools.cs ファイルを作成しました。

cd ..

rem slnファイルを探して、新しいプロジェクトを追加
for %%f in (*.sln) do (
    echo ソリューションファイル %%f に %feature_name% プロジェクトを追加します。
    dotnet sln %%f add %feature_name%\%feature_name%.csproj
    if !errorlevel! neq 0 (
        echo プロジェクトのソリューションへの追加に失敗しました。
        goto :end
    )
    echo プロジェクトをソリューションに追加しました。
)

echo.
echo セットアップが完了しました！

:end
echo.
echo 終了するには何かキーを押してください...
pause >nul