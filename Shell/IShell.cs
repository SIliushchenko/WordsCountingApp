namespace Shell
{
    /// <summary>
    /// An application container of packages, services and components that together makeup the logical structure of the application.
    /// </summary>
    public interface IShell
    {
        string StatusText { get; set; }
    }
}