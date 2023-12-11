namespace IZote;

internal class ZoteAgent : Agent<ZoteAgent.Message, KnightAgent.Message>
{
    public class Message { }

    public override List<(string, string)> GetPreloadNames()
    {
        return new List<(string, string)> {
            ("GG_Grey_Prince_Zote", "Grey Prince"),
            ("GG_Mighty_Zote","Battle Control")
        };
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

    public override KnightAgent.Message Update(Message inputMessage)
    {
        return null;
    }
}
