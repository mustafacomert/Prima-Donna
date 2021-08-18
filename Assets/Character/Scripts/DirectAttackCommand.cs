public class DirectAttackCommand : Command
{
    public void Execute(Character character)
    {
        character.DirectAttack();
    }
}