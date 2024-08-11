using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatNode : DecoratorNode
{
    public int _repeatCount;
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        for(int i = 0; i < _repeatCount; ++i){
            while(true){
                if(State.Failure == child.Update() ||
                    State.Success == child.Update()){
                        break;
                }
            }
        }
        return State.Running;
    }
}
