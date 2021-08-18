using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleCommand : Command
{
    public void Execute(Character character)
    {
        character.Idle();
    }
}
