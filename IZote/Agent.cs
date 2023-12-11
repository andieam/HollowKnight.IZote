namespace IZote;

internal abstract class Agent<InputMessage, OutputMessage>
{
    public abstract List<(string, string)> GetPreloadNames();

    public abstract void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects);

    public abstract void Install();

    public abstract bool Installed();

    public abstract void Uninstall();

    public abstract OutputMessage Update(InputMessage inputMessage);
}
