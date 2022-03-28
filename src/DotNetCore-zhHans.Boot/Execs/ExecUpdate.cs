namespace DotNetCore_zhHans.Boot;

partial class ExecUpdate : ExecBase
{
    public ExecUpdate(ViewModel viewModel) : base(viewModel) { }

    public async override void Run()
    {
        vm.Title = "更新";
        vm.Details = "获取更新配置";
        vm.Context = "更新";
    }
}
