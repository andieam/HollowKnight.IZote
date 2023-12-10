namespace IZote;
public class IZote : Mod
{
    public IZote() : base("IZote")
    {
        instance = this;
    }
    public override string GetVersion() => "1.0.0.0";
    public override List<(string, string)> GetPreloadNames()
    {
        return new List<(string, string)> { ("GG_Grey_Prince_Zote", "Grey Prince") };
    }
    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        knightRewriter.Initialize(preloadedObjects);
        zoteRewriter.Initialize(preloadedObjects);
        HKMirror.Hooks.OnHooks.OnHeroController.AfterOrig.Update += Update;
    }
    private void SetStateSafe(PlayMakerFSM control, string state)
    {
        if (control.ActiveStateName == state)
        {
            return;
        }
        if (control.ActiveStateName == "Run")
        {
            var action = control.GetAction("Run End", 1);
            action.OnEnter();
        }
        else if ((control.ActiveStateName == "Charge Antic" && state != "Charge Start") || control.ActiveStateName == "Charge Start")
        {
            foreach (var i in new List<int> { 4, 5, 6, 7, 9 })
            {
                var action = control.GetAction("Charge Fall", i);
                action.OnEnter();
            }
        }
        control.SetState(state);
    }
    private void UpdateStates()
    {
        if (zoteRewriter.ready)
        {
            var knight = HeroController.instance.gameObject;
            var greyPrinceTransform = knight.transform.Find("Grey Prince");
            var greyPrince = greyPrinceTransform.gameObject;
            var control = greyPrince.LocateMyFSM("Control");
            var state = knightRewriter.Update();
            if (state == "Stand")
            {
                SetStateSafe(control, "Stand");
            }
            else if (state == "Run")
            {
                if (control.ActiveStateName != "Run")
                {
                    SetStateSafe(control, "Run Antic");
                }
            }
            else if (state == "Jump")
            {
                SetStateSafe(control, "Jump");
            }
            else if (state == "Charge")
            {
                if (control.ActiveStateName != "Charge Start")
                {
                    SetStateSafe(control, "Charge Antic");
                }
            }
        }
    }
    private void Update(HKMirror.Hooks.OnHooks.OnHeroController.Delegates.Params_Update args)
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            var knight = HeroController.instance.gameObject;
            var greyPrinceTransform = knight.transform.Find("Grey Prince");
            if (greyPrinceTransform != null)
            {
                zoteRewriter.Exit();
                knightRewriter.Exit();
            }
            else
            {
                knightRewriter.Enter();
                zoteRewriter.Enter();
            }
        }
        UpdateStates();
    }
    public static IZote instance;
    private KnightRewriter knightRewriter = new();
    private ZoteRewriter zoteRewriter = new();
}
