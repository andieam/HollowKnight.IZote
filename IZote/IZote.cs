global using Modding;
global using UnityEngine;
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
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ActiveSceneChanged;
        ModHooks.HeroUpdateHook += HeroUpdateHook;
        greyPrinceTemplate = preloadedObjects["GG_Grey_Prince_Zote"]["Grey Prince"];
    }
    private void ActiveSceneChanged(UnityEngine.SceneManagement.Scene from, UnityEngine.SceneManagement.Scene to)
    {
        if (HeroController.instance != null)
        {
            var knight = HeroController.instance.gameObject;
            var greyPrinceTransform = knight.transform.Find("Grey Prince");
            if (greyPrinceTransform == null)
            {
                LogDebug("Detected a Knight w/o a Grey Prince. Adding one.");
                var greyPrince = UnityEngine.Object.Instantiate(greyPrinceTemplate, knight.transform);
            }
            else
            {
                LogDebug("Detected a Knight with a Grey Prince.");
            }
        }
    }
    private void HeroUpdateHook()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            var knight = HeroController.instance.gameObject;
            var greyPrinceTransform = knight.transform.Find("Grey Prince");
            if (greyPrinceTransform != null)
            {
                var greyPrince = greyPrinceTransform.gameObject;
                greyPrince.SetActive(!greyPrince.activeSelf);
            }
        }
    }
    private GameObject greyPrinceTemplate;
}
