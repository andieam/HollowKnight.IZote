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
        HKMirror.Hooks.OnHooks.OnHeroController.BeforeOrig.Update += UpdateBefore;
        HKMirror.Hooks.OnHooks.OnHeroController.AfterOrig.Update += UpdateAfter;
    }
    private void UpdateBefore(HKMirror.Hooks.OnHooks.OnHeroController.Delegates.Params_Update args)
    {
        var knight = HeroController.instance.gameObject;
        var greyPrinceTransform = knight.transform.Find("Grey Prince");
        if (Input.GetKeyDown(KeyCode.F2))
        {
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
        if (zoteRewriter.ready)
        {
            greyPrinceTransform.localPosition = new Vector3(0.1f, 1.1f, 0.001f);
            knight.GetComponent<tk2dSprite>().color = Vector4.zero;
            knightRewriter.UpdateBefore();
        }
    }
    private void SetStateSafe(PlayMakerFSM control, string state)
    {
        if (control.ActiveStateName == state)
        {
            return;
        }
        if (control.ActiveStateName == "Land Waves"
            || control.ActiveStateName == "Land Normal")
        {
            return;
        }
        if (control.ActiveStateName == "Land Dir"
            || control.ActiveStateName == "Slash Waves L"
            || control.ActiveStateName == "Slash Waves R"
            || control.ActiveStateName == "Stomp Slash"
            || control.ActiveStateName == "Slash End")
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
    private void UpdateAfter(HKMirror.Hooks.OnHooks.OnHeroController.Delegates.Params_Update args)
    {
        if (zoteRewriter.ready)
        {
            var knight = HeroController.instance.gameObject;
            var greyPrinceTransform = knight.transform.Find("Grey Prince");
            var greyPrince = greyPrinceTransform.gameObject;
            var control = greyPrince.LocateMyFSM("Control");
            var state = knightRewriter.UpdateAfter();
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
            else if (state == "Dash")
            {
                SetStateSafe(control, "Stomp Shift L");
            }
            else if (state == "Jump")
            {
                SetStateSafe(control, "Jump");
            }
            else if (state == "Land")
            {
                SetStateSafe(control, "Land Waves");
            }
            else if (state == "Stomp")
            {
                SetStateSafe(control, "Stomp");
            }
            else if (state == "Slash")
            {
                SetStateSafe(control, "Land Dir");
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
    public static IZote instance;
    private KnightRewriter knightRewriter = new();
    public ZoteRewriter zoteRewriter = new();
}
