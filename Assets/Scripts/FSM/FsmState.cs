public abstract class FsmState<T>
{
    protected T owner;
    public void Initialize(T owner)
    {
        this.owner = owner;
        OnInitialize();
    }
    public abstract void OnInitialize();
    public abstract void OnEnter();
    public abstract void OnUpdate(float deltaTime);
    public abstract void OnExit();
}
