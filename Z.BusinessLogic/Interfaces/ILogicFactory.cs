namespace Z.BusinessLogic.Interfaces
{
    public interface ILogicFactory
    {
        MainWindowLogic GenerateMainWindowLogic(IMainWindowViewModelAccess viewModel);
    }
}