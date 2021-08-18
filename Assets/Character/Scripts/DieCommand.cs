
public class DieCommand : Command
{
    public void Execute(Character character)
    {
        character.Die();
    }
}
