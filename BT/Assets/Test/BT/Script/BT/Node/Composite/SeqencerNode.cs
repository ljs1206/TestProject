public class SeqencerNode : CompositeNode
{
    int current = 0;

    protected override void OnStart()
    {
        current = 0;
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        var child = children[current];
        switch(child.Update()){
            case State.Running:
                return State.Running;
            case State.Failure:
                return State.Failure;
            case State.Success:
                current++;
                break;
        }

        return children.Count == current ? State.Success : State.Running;
    }
}
