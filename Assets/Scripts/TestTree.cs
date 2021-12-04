using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTree : MonoBehaviour
{
    BehaviourTreeEngine tree;
    void Start()
    {
        tree= new BehaviourTreeEngine();
        SequenceNode root = tree.CreateSequenceNode("fakeRoot", false);//root
        SelectorNode selector= tree.CreateSelectorNode("selector");
        LoopDecoratorNode loop = tree.CreateLoopNode("root", selector);

        //both nodes return failed to ensure they are going to be executed
        LeafNode node1 = tree.CreateLeafNode("node1", Action1, AlwaysFailed);
        LeafNode node2 = tree.CreateLeafNode("node2", Action2, AlwaysFailed);

        tree.SetRootNode(root);
        root.AddChild(loop);
        selector.AddChild(node1);
        selector.AddChild(node2);
    }

    public void Action1()
    {
        Debug.Log("Action1");
    }

    public void Action2()
    {
        Debug.Log("Action2");
    }

    #region ReturnValues Functions
    private void NoneAction() { }
    private ReturnValues AlwaysSucceed()
    {
        return ReturnValues.Succeed;
    }
    private ReturnValues AlwaysFailed()
    {
        return ReturnValues.Failed;
    }
    private ReturnValues AlwaysRunning()
    {
        return ReturnValues.Running;
    }
    #endregion ReturnValues Functions

    // Update is called once per frame
    void Update()
    {
        Debug.Log(tree.ActiveNode);
        tree.Update();
    }
}
