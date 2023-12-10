namespace IZote;

internal class ZoteRewriter
{
    public void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
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
            control.transform.localPosition = new Vector3(0.1f, 1.1f, 0.001f);
            ready = true;
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
                control.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShot>("Jump", 2).volume = 1;
            }
            else
            {
                control.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShot>("Jump", 2).volume = 0;
            }
        }, 0);
        control.RemoveTransition("Jump", "FINISHED");
    }
    private void RewriteChargeStates(PlayMakerFSM control)
    {
        var chargeHit = control.gameObject.Find("Charge Hit");
        chargeHit.layer = LayerMask.NameToLayer("Attack");
        chargeHit.RemoveComponent<DamageHero>();
        chargeHit.AddComponent<DamageEnemies>();
        var audioSpawnPoint = control.transform.Find("Audio Spawn Point").gameObject;
        control.GetAction<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Charge Antic", 3).spawnPoint = audioSpawnPoint;
        control.RemoveAction("Charge Antic", 2);
        control.RemoveAction("Charge Antic", 1);
        control.AddCustomAction("Charge Antic", () =>
        {
            var damageEnemiesCharge = chargeHit.GetComponent<DamageEnemies>();
            var damageEnemiesSlash = HeroController.instance.gameObject.Find("Attacks").Find("Slash").LocateMyFSM("damages_enemy");
            damageEnemiesCharge.attackType = (AttackTypes)damageEnemiesSlash.FsmVariables.GetFsmInt("attackType").Value;
            damageEnemiesCharge.circleDirection = damageEnemiesSlash.FsmVariables.GetFsmBool("circleDirection").Value;
            damageEnemiesCharge.damageDealt = damageEnemiesSlash.FsmVariables.GetFsmInt("damageDealt").Value;
            damageEnemiesCharge.direction = damageEnemiesSlash.FsmVariables.GetFsmFloat("direction").Value;
            damageEnemiesCharge.ignoreInvuln = damageEnemiesSlash.FsmVariables.GetFsmBool("Ignore Invuln").Value;
            damageEnemiesCharge.magnitudeMult = damageEnemiesSlash.FsmVariables.GetFsmFloat("magnitudeMult").Value;
            damageEnemiesCharge.moveDirection = damageEnemiesSlash.FsmVariables.GetFsmBool("moveDirection").Value;
            damageEnemiesCharge.specialType = (SpecialTypes)damageEnemiesSlash.FsmVariables.GetFsmInt("Special Type").Value;
        });
        control.RemoveAction("Charge Start", 7);
        control.RemoveAction("Charge Start", 6);
        control.RemoveAction("Charge Start", 5);
        control.RemoveAction("Charge Start", 0);
        control.RemoveTransition("Charge Start", "FINISHED");
        control.RemoveTransition("Charge Start", "L");
        control.RemoveTransition("Charge Start", "R");
        control.RemoveTransition("Charge Start", "FAIL");
    }
    public void Enter()
    {
        var knight = HeroController.instance.gameObject;
        var greyPrince = UnityEngine.Object.Instantiate(greyPrinceTemplate, knight.transform);
        greyPrince.SetActive(true);
        greyPrince.name = "Grey Prince";
        greyPrince.transform.localScale = new Vector3(-1, 1, 1);
        UnityEngine.Object.Destroy(greyPrince.LocateMyFSM("Constrain X"));
        UnityEngine.Object.Destroy(greyPrince.GetComponent<DamageHero>());
        UnityEngine.Object.Destroy(greyPrince.GetComponent<Rigidbody2D>());
        UnityEngine.Object.Destroy(greyPrince.GetComponent<EnemyDeathEffectsUninfected>());
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
            state.AddCustomAction(() =>
            {
                IZote.instance.Log("Leaving state " + state.Name + ".");
            });
        }
        ready = false;
    }
    public void Exit()
    {
        var knight = HeroController.instance.gameObject;
        var greyPrinceTransform = knight.transform.Find("Grey Prince");
        var greyPrince = greyPrinceTransform.gameObject;
        UnityEngine.Object.Destroy(greyPrince);
        ready = false;
    }
    private GameObject greyPrinceTemplate;
    public bool ready;
}
