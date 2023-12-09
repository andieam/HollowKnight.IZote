global using Modding;
global using UnityEngine;
using Satchel;

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
        greyPrinceTemplate = preloadedObjects["GG_Grey_Prince_Zote"]["Grey Prince"];
    }
    private void HeroUpdateHook()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            var knight = HeroController.instance.gameObject;
            var greyPrinceTransform = knight.transform.Find("Grey Prince");
            if (greyPrinceTransform != null)
            {
                Log("Removing Grey Prince.");
                var greyPrince = greyPrinceTransform.gameObject;
                UnityEngine.Object.Destroy(greyPrince);
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
                var control = greyPrince.LocateMyFSM("Control");
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
                control.AddCustomAction("Set Damage", () => control.SetState("Stand"));
                control.RemoveAction("Stand", 2);
                control.RemoveAction("Stand", 1);
                control.RemoveTransition("Stand", "FINISHED");
                control.RemoveTransition("Stand", "TOOK DAMAGE");
                control.AddCustomAction("Stand", () => greyPrince.transform.localPosition = new Vector3(0, 2.2788f, 0));
                foreach (var state in control.FsmStates)
                {
                    state.InsertCustomAction(() =>
                    {
                        Log("Entering state " + state.Name + ".");
                    }, 0);
                }
            }
        }
    }
    private GameObject greyPrinceTemplate;
}
