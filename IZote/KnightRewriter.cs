namespace IZote;

internal class KnightRewriter
{
    public void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        HKMirror.Hooks.OnHooks.OnHeroController.WithOrig.DoAttack += DoAttack;
    }
    private void DoAttack(On.HeroController.orig_DoAttack orig, HeroController self)
    {
        var knight = HeroController.instance.gameObject;
        var greyPrinceTransform = knight.transform.Find("Grey Prince");
        if (greyPrinceTransform == null)
        {
            orig(self);
        }
    }
    public void Enter()
    {
        var controller = HeroController.instance;
        controller.RUN_SPEED = 12;
        controller.RUN_SPEED_CH = 12;
        controller.RUN_SPEED_CH_COMBO = 12;
        controller.GetComponent<Rigidbody2D>().gravityScale = 1.5f;
        controller.JUMP_SPEED = 25;
    }
    public void Exit()
    {
        var controller = HeroController.instance;
        controller.RUN_SPEED = 8.3f;
        controller.RUN_SPEED_CH = 10;
        controller.RUN_SPEED_CH_COMBO = 11.5f;
        controller.GetComponent<Rigidbody2D>().gravityScale = 0.79f;
        controller.JUMP_SPEED = 16.65f;
    }
    public string Update()
    {
        var controller = HeroController.instance.Reflect();
        controller.nailChargeTimer = 0;
        if (HeroController.instance.cState.onGround)
        {
            if (controller.inputHandler.inputActions.attack.IsPressed && controller.CanAttack())
            {
                return "Charge";
            }
            else if (HeroController.instance.hero_state == ActorStates.running)
            {
                return "Run";
            }
            else
            {
                return "Stand";
            }
        }
        else
        {
            return "Jump";
        }
    }
}
