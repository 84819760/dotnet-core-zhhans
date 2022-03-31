using System.Reflection;
using System.Windows;

namespace DotNetCore_zhHans.Boot;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static string[] Args { get; private set; } = null!;

    public static readonly string? Version =
        $"{typeof(App).Assembly.GetName().Version?.ToString(3)}";

    protected override void OnStartup(StartupEventArgs e)
    {

        Args = e.Args ?? Array.Empty<string>();
        if (Args.Any(x => x == "--updateOk"))
        {
            Share.GetRootDirectory(true);
            MessageBox.Show("更新完成", Version);
        }
        Share.Show("Boot", Args);
    }
}
