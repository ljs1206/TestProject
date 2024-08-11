using System.Collections.Generic;

public abstract class CompositeNode : Node
{
    public List<Node> children = new();

    public override Node Clone()
    {
        CompositeNode node = Instantiate(this);
        node.children = children.ConvertAll(c => c.Clone());
        return node;
    }
}
