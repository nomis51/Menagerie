namespace Menagerie.Shared.Abstractions;

public interface IService
{
    void Initialize();
    Task Start();
}