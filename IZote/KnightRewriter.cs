using Modding.Utils;

namespace IZote;

internal class KnightRewriter
{
    public void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        HKMirror.Hooks.OnHooks.OnHeroController.WithOrig.DoAttack += DoAttack;
        HKMirror.Hooks.OnHooks.OnHeroController.WithOrig.CanDoubleJump += CanDoubleJump;
        HKMirror.Hooks.OnHooks.OnHeroController.BeforeOrig.FixedUpdate += FixedUpdate;
    }
    private void ScaleHeight(PlayMakerFSM fsm, float scale)
    {
        var localScale = fsm.transform.localScale;
        localScale.y *= scale;
        fsm.transform.localScale = localScale;
    }
    private void ScaleHeight(GameObject gameObject, float scale)
    {
        var localScale = gameObject.transform.localScale;
        localScale.y *= scale;
        gameObject.transform.localScale = localScale;
    }
    public void Enter()
    {
        var controller = HeroController.instance;
        controller.RUN_SPEED = 12;
        controller.RUN_SPEED_CH = 12;
        controller.RUN_SPEED_CH_COMBO = 12;
        controller.GetComponent<Rigidbody2D>().gravityScale = 1.5f;
        controller.JUMP_SPEED = 25;
        controller.GetComponent<BoxCollider2D>().size = new Vector2(2.5469f, 3.75f);
        var heroBox = controller.gameObject.Find("HeroBox");
        heroBox.GetComponent<BoxCollider2D>().size = new Vector2(2.5469f, 3.75f);
        controller.GetComponent<tk2dSprite>().color = Vector4.zero;
        ScaleHeight(controller.dashBurst, heightScale);
        ScaleHeight(controller.sharpShadowPrefab, heightScale);
        ScaleHeight(controller.shadowdashDownBurstPrefab, heightScale);
        ScaleHeight(controller.shadowdashBurstPrefab, heightScale);
        ScaleHeight(controller.shadowRechargePrefab, heightScale);
        ScaleHeight(controller.shadowdashParticlesPrefab, heightScale);
        ScaleHeight(controller.shadowRingPrefab, heightScale);
        controller.DASH_SPEED = 35;
        controller.DASH_SPEED_SHARP = 35;
        controller.BIG_FALL_TIME = Mathf.Infinity;
        preivouslyOnGround = controller.cState.onGround;
    }
    public void Exit()
    {
        var controller = HeroController.instance;
        controller.RUN_SPEED = 8.3f;
        controller.RUN_SPEED_CH = 10;
        controller.RUN_SPEED_CH_COMBO = 11.5f;
        controller.GetComponent<Rigidbody2D>().gravityScale = 0.79f;
        controller.JUMP_SPEED = 16.65f;
        controller.GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 1.2813f);
        var heroBox = controller.gameObject.Find("HeroBox");
        heroBox.GetComponent<BoxCollider2D>().size = new Vector2(0.4554f, 1.1875f);
        controller.GetComponent<tk2dSprite>().color = Vector4.one;
        ScaleHeight(controller.dashBurst, 1 / heightScale);
        ScaleHeight(controller.sharpShadowPrefab, 1 / heightScale);
        ScaleHeight(controller.shadowdashDownBurstPrefab, 1 / heightScale);
        ScaleHeight(controller.shadowdashBurstPrefab, 1 / heightScale);
        ScaleHeight(controller.shadowRechargePrefab, 1 / heightScale);
        ScaleHeight(controller.shadowdashParticlesPrefab, 1 / heightScale);
        ScaleHeight(controller.shadowRingPrefab, 1 / heightScale);
        controller.DASH_SPEED = 20;
        controller.DASH_SPEED_SHARP = 28;
        controller.BIG_FALL_TIME = 1.1f;
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
    private bool CanDoubleJump(On.HeroController.orig_CanDoubleJump orig, HeroController self)
    {
        var knight = HeroController.instance.gameObject;
        var greyPrinceTransform = knight.transform.Find("Grey Prince");
        if (greyPrinceTransform == null)
        {
            return orig(self);
        }
        else
        {
            return false;
        }
    }
    private void FixedUpdate(HKMirror.Hooks.OnHooks.OnHeroController.Delegates.Params_FixedUpdate args)
    {
        if (IZote.instance.zoteRewriter.ready)
        {
            var controller = HeroController.instance;
            if (IZote.instance.zoteRewriter.slamEffectNew != null && IZote.instance.zoteRewriter.slamEffectNew.activeSelf)
            {
                controller.RUN_SPEED = 6;
                controller.RUN_SPEED_CH = 6;
                controller.RUN_SPEED_CH_COMBO = 6;
            }
            else
            {
                controller.RUN_SPEED = 12;
                controller.RUN_SPEED_CH = 12;
                controller.RUN_SPEED_CH_COMBO = 12;
            }
        }
    }
    public void UpdateBefore()
    {
    }
    public string UpdateAfter()
    {
        var controller = HeroController.instance.Reflect();
        controller.nailChargeTimer = 0;
        if (controller.runEffect != null)
        {
            controller.runEffect.SetActive(false);
        }
        var runEffect = controller.gameObject.scene.FindGameObject("Run Effects(Clone)");
        if (runEffect != null)
        {
            GameObject.Destroy(runEffect);
        }
        if (!preivouslyOnGround && HeroController.instance.cState.onGround)
        {
            preivouslyOnGround = true;
            return "Land";
        }
        preivouslyOnGround = HeroController.instance.cState.onGround;
        if (HeroController.instance.cState.onGround && controller.inputHandler.inputActions.attack.IsPressed && controller.CanAttack())
        {
            var knight = controller.gameObject;
            var greyPrinceTransform = knight.transform.Find("Grey Prince");
            var greyPrince = greyPrinceTransform.gameObject;
            var control = greyPrince.LocateMyFSM("Control");
            var rigidbody2D = knight.GetComponent<Rigidbody2D>();
            var velocity = rigidbody2D.velocity;
            if (control.ActiveStateName == "Charge Start" && Mathf.Abs(velocity.x) < 1)
            {
                var direction = knight.transform.localScale.x < 0 ? 1 : -1;
                var localPosition = knight.transform.localPosition;
                localPosition.x += Time.deltaTime * direction;
                knight.transform.localPosition = localPosition;
            }
            return "Charge";
        }
        else if (controller.cState.dashing)
        {
            return "Dash";
        }
        else if (controller.hero_state == ActorStates.running)
        {
            return "Run";
        }
        else if (HeroController.instance.cState.onGround)
        {
            return "Stand";
        }
        else
        {
            return "Jump";
        }
    }
    private float heightScale = 2;
    private bool preivouslyOnGround;
}
