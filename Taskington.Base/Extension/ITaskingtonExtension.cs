namespace Taskington.Base.Extension;

public interface ITaskingtonExtension<T>
{
    object? InitializeEnvironment(T baseEnvironment);
}
