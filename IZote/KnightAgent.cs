namespace IZote;

internal class KnightAgent : Agent<KnightAgent.Message, ZoteAgent.Message>
{
    public class Message { }

    public override List<(string, string)> GetPreloadNames()
    {
        return new List<(string, string)> { };
    }

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
    }

    public override void Install()
    {

    }

    public override bool Installed()
    {
        return false;
    }

    public override void Uninstall()
    {

    }

    public override ZoteAgent.Message Update(Message inputMessage)
    {
        return null;
    }
}
