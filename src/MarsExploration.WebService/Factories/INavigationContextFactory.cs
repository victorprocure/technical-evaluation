namespace MarsExploration.WebService.Factories
{
    public interface INavigationContextFactory
    {
        NavigationContext CreateFromNavigationString(string navigationString);
    }
}