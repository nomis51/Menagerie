using Desktop.Robot;

namespace Menagerie.Core.Services.Abstractions;

public interface IKeyboardService
{
    void ClearModifiers();
    void ClearKey(Key key);
    void SendKey(Key key);
    void SendKey(Key key, Key modifier);
}