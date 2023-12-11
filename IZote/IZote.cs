namespace IZote;

public class IZote : Mod
{
    public IZote() : base("IZote")
    {
    }

    public override string GetVersion() => "2.0.0.0";

    public override List<(string, string)> GetPreloadNames()
    {
        List<(string, string)> preloadNames = new();
        foreach (var preloadName in knightAgent.GetPreloadNames())
        {
            preloadNames.Add(preloadName);
        }
        foreach (var preloadName in zoteAgent.GetPreloadNames())
        {
            preloadNames.Add(preloadName);
        }
        return preloadNames;
    }

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        HKMirror.Hooks.OnHooks.OnHeroController.AfterOrig.Update += Update;
        knightAgent.Initialize(preloadedObjects);
        zoteAgent.Initialize(preloadedObjects);
    }

    private bool Installed()
    {
        if (!knightAgent.Installed())
        {
            Assert.IsFalse(zoteAgent.Installed());
            return false;
        }
        else
        {
            Assert.IsTrue(zoteAgent.Installed());
            return true;
        }
    }

    private void Update(HKMirror.Hooks.OnHooks.OnHeroController.Delegates.Params_Update args)
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (!Installed())
            {
                knightAgent.Install();
                zoteAgent.Install();
                Assert.IsTrue(Installed());
            }
            else
            {
                knightAgent.Uninstall();
                zoteAgent.Uninstall();
                Assert.IsFalse(Installed());
            }
        }
        if (Installed())
        {
            var zoteMessage = knightAgent.Update(lastKnightMessage);
            lastKnightMessage = zoteAgent.Update(zoteMessage);
        }
    }

    private KnightAgent knightAgent;
    private ZoteAgent zoteAgent;
    private KnightAgent.Message lastKnightMessage;
}
