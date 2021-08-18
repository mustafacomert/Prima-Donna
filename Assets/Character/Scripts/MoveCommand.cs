public class MoveCommand : Command
{
    public void Execute(Character character)
    {
        character.Move();
    }
}
