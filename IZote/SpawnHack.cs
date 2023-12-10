namespace IZote;
public class SpawnHack : FsmStateAction
{
    public void CopyFrom(SpawnObjectFromGlobalPool source)
    {
        gameObject = source.gameObject;
        spawnPoint = source.spawnPoint;
        position = source.position;
        rotation = source.rotation;
        storeObject = source.storeObject;
    }
    // Token: 0x0600396E RID: 14702 RVA: 0x0014E84F File Offset: 0x0014CA4F
    public override void Reset()
    {
        this.gameObject = null;
        this.spawnPoint = null;
        this.position = new FsmVector3
        {
            UseVariable = true
        };
        this.rotation = new FsmVector3
        {
            UseVariable = true
        };
        this.storeObject = null;
    }

    // Token: 0x0600396F RID: 14703 RVA: 0x0014E88C File Offset: 0x0014CA8C
    public override void OnEnter()
    {
        if (this.gameObject.Value != null)
        {
            Vector3 a = Vector3.zero;
            Vector3 euler = Vector3.up;
            if (this.spawnPoint.Value != null)
            {
                a = this.spawnPoint.Value.transform.position;
                if (!this.position.IsNone)
                {
                    a += this.position.Value;
                }
                euler = ((!this.rotation.IsNone) ? this.rotation.Value : this.spawnPoint.Value.transform.eulerAngles);
            }
            else
            {
                if (!this.position.IsNone)
                {
                    a = this.position.Value;
                }
                if (!this.rotation.IsNone)
                {
                    euler = this.rotation.Value;
                }
            }
            if (this.gameObject != null)
            {
                // GameObject value = this.gameObject.Value.Spawn(a, Quaternion.Euler(euler));
                GameObject value = UnityEngine.Object.Instantiate(this.gameObject.Value);
                value.transform.localPosition = a;
                value.transform.localRotation = Quaternion.Euler(euler);
                IZote.instance.Log("Instantiated an object.");
                this.storeObject.Value = value;
            }
        }
        base.Finish();
    }

    // Token: 0x04003C2A RID: 15402
    [RequiredField]
    public FsmGameObject gameObject;

    // Token: 0x04003C2B RID: 15403
    public FsmGameObject spawnPoint;

    // Token: 0x04003C2C RID: 15404
    public FsmVector3 position;

    // Token: 0x04003C2D RID: 15405
    public FsmVector3 rotation;

    // Token: 0x04003C2E RID: 15406
    [UIHint(UIHint.Variable)]
    public FsmGameObject storeObject;
}
