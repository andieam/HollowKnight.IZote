using GlobalEnums;
using HKMirror.Reflection;
using Modding;
using Satchel;
using UnityEngine;

namespace IZote;
public class IZote : Mod
{
    public IZote() : base("IZote")
    {
    }
    public override string GetVersion() => "1.0.0.0";
    public override List<(string, string)> GetPreloadNames()
    {
        return new List<(string, string)> { ("GG_Grey_Prince_Zote", "Grey Prince") };
    }
    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        ModHooks.HeroUpdateHook += HeroUpdateHook;
        HKMirror.Hooks.OnHooks.OnHeroController.WithOrig.DoAttack += DoAttack;
        greyPrinceTemplate = preloadedObjects["GG_Grey_Prince_Zote"]["Grey Prince"];
    }
    private void RewriteInitStates(PlayMakerFSM control)
    {
        control.RemoveTransition("Init", "FINISHED");
        control.AddTransition("Init", "FINISHED", "Level 3");
        control.RemoveAction("Dormant", 3);
        control.AddCustomAction("Dormant", () => control.SetState("Enter 1"));
        control.RemoveAction("Enter 1", 5);
        control.RemoveAction("Enter 1", 3);
        control.AddCustomAction("Enter 1", () => control.SetState("Activate"));
        control.RemoveAction("Activate", 3);
        control.RemoveAction("Activate", 2);
        control.RemoveAction("Activate", 1);
        control.RemoveTransition("Set Damage", "FINISHED");
        control.AddCustomAction("Set Damage", () =>
        {
            control.transform.localPosition = new Vector3(0, 2.2788f, 0);
            setupComplete = true;
        });
    }
    private void RewriteStandStates(PlayMakerFSM control)
    {
        control.RemoveAction("Stand", 2);
        control.RemoveAction("Stand", 1);
        control.RemoveTransition("Stand", "FINISHED");
        control.RemoveTransition("Stand", "TOOK DAMAGE");
    }
    private void RewriteRunStates(PlayMakerFSM control)
    {
        var audioSpawnPoint = control.transform.Find("Audio Spawn Point").gameObject;
        control.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShot>("Run Antic", 1).spawnPoint = audioSpawnPoint;
        control.RemoveAction("Run", 7);
        control.RemoveAction("Run", 6);
        control.RemoveAction("Run", 5);
        control.RemoveAction("Run", 4);
        control.RemoveAction("Run", 3);
        control.RemoveAction("Run", 2);
        control.RemoveAction("Run", 0);
        control.RemoveTransition("Run", "FINISHED");
        control.RemoveTransition("Run", "TOOK DAMAGE");
    }
    private void RewriteJumpStates(PlayMakerFSM control)
    {
        var audioSpawnPoint = control.transform.Find("Audio Spawn Point").gameObject;
        control.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShot>("Jump", 1).spawnPoint = audioSpawnPoint;
        control.RemoveAction("Jump", 6);
        control.RemoveAction("Jump", 5);
        control.RemoveAction("Jump", 4);
        control.RemoveAction("Jump", 3);
        control.InsertCustomAction("Jump", () =>
        {
            if (HeroController.instance.cState.jumping)
            {
                control.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShot>("Jump", 3).volume = 1;
            }
            else
            {
                control.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShot>("Jump", 3).volume = 0;
            }
        }, 0);
        control.RemoveTransition("Jump", "FINISHED");
    }
    private void RewriteChargeStates(PlayMakerFSM control)
    {
        var audioSpawnPoint = control.transform.Find("Audio Spawn Point").gameObject;
        control.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Charge Antic", 3).spawnPoint = audioSpawnPoint;
        control.RemoveAction("Charge Antic", 2);
        control.RemoveAction("Charge Antic", 1);
        control.RemoveAction("Charge Start", 7);
        control.RemoveAction("Charge Start", 6);
        control.RemoveAction("Charge Start", 5);
        control.RemoveAction("Charge Start", 0);
        control.RemoveTransition("Charge Start", "FINISHED");
        control.RemoveTransition("Charge Start", "L");
        control.RemoveTransition("Charge Start", "R");
        control.RemoveTransition("Charge Start", "FAIL");
    }
    private void ToggleGreyPrince()
    {
        var knight = HeroController.instance.gameObject;
        var greyPrinceTransform = knight.transform.Find("Grey Prince");
        if (greyPrinceTransform != null)
        {
            Log("Removing Grey Prince.");
            var greyPrince = greyPrinceTransform.gameObject;
            UnityEngine.Object.Destroy(greyPrince);
            HeroController.instance.RUN_SPEED = 8.3f;
            HeroController.instance.RUN_SPEED_CH = 10;
            HeroController.instance.RUN_SPEED_CH_COMBO = 11.5f;
            knight.GetComponent<Rigidbody2D>().gravityScale = 0.79f;
            HeroController.instance.JUMP_SPEED = 16.65f;
        }
        else
        {
            Log("Adding Grey Prince.");
            var greyPrince = UnityEngine.Object.Instantiate(greyPrinceTemplate, knight.transform);
            greyPrince.SetActive(true);
            greyPrince.name = "Grey Prince";
            greyPrince.transform.localScale = new Vector3(-1, 1, 1);
            UnityEngine.Object.Destroy(greyPrince.LocateMyFSM("Constrain X"));
            UnityEngine.Object.Destroy(greyPrince.GetComponent<DamageHero>());
            UnityEngine.Object.Destroy(greyPrince.GetComponent<Rigidbody2D>());
            var audioSpawnPoint = new GameObject();
            audioSpawnPoint.name = "Audio Spawn Point";
            audioSpawnPoint.transform.parent = greyPrince.transform;
            audioSpawnPoint.transform.localPosition = new Vector3(0, 1, 0);
            var control = greyPrince.LocateMyFSM("Control");
            RewriteInitStates(control);
            RewriteStandStates(control);
            RewriteRunStates(control);
            RewriteJumpStates(control);
            RewriteChargeStates(control);
            foreach (var state in control.FsmStates)
            {
                state.InsertCustomAction(() =>
                {
                    Log("Entering state " + state.Name + ".");
                }, 0);
            }
            setupComplete = false;
            HeroController.instance.RUN_SPEED = 12;
            HeroController.instance.RUN_SPEED_CH = 12;
            HeroController.instance.RUN_SPEED_CH_COMBO = 12;
            knight.GetComponent<Rigidbody2D>().gravityScale = 1.5f;
            HeroController.instance.JUMP_SPEED = 25;
        }
    }
    private void DoAttack(On.HeroController.orig_DoAttack orig, HeroController self)
    {
        var knight = HeroController.instance.gameObject;
        var greyPrinceTransform = knight.transform.Find("Grey Prince");
        if (greyPrinceTransform != null) return;
        orig(self);
    }
    // use ZOTE TITLE END to move state to a common idle state
    private void SetStateAsync(PlayMakerFSM control, string state)
    {
        if (control.ActiveStateName == state) return;
        var knight = HeroController.instance.gameObject;
        var greyPrinceTransform = knight.transform.Find("Grey Prince");
        if (control.ActiveStateName == "Run")
        {
            var particleRun = greyPrinceTransform.Find("Pt Run").gameObject;
            particleRun.SetActive(false);
            particleRun.SetActive(true);
        }
        control.SetState(state);
    }
    private void UpdateStates()
    {
        var knight = HeroController.instance.gameObject;
        var greyPrinceTransform = knight.transform.Find("Grey Prince");
        if (greyPrinceTransform != null && setupComplete)
        {
            var greyPrince = greyPrinceTransform.gameObject;
            var control = greyPrince.LocateMyFSM("Control");
            var charging = false;
            if (HeroController.instance.Reflect().nailChargeTimer > 0)
            {
                HeroController.instance.Reflect().nailChargeTimer = 0;
                if (HeroController.instance.cState.onGround)
                {
                    charging = true;
                }
            }
            if (HeroController.instance.cState.onGround)
            {
                if (charging)
                {
                    if (control.ActiveStateName != "Charge Start")
                    {
                        SetStateAsync(control, "Charge Antic");
                    }
                }
                else if (HeroController.instance.hero_state == ActorStates.running)
                {
                    if (control.ActiveStateName != "Run")
                    {
                        SetStateAsync(control, "Run Antic");
                    }
                }
                else
                {
                    SetStateAsync(control, "Stand");
                }
            }
            else
            {
                SetStateAsync(control, "Jump");
            }
        }
    }
    private void HeroUpdateHook()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ToggleGreyPrince();
        }
        UpdateStates();
    }
    private GameObject greyPrinceTemplate;
    private bool setupComplete;
}
