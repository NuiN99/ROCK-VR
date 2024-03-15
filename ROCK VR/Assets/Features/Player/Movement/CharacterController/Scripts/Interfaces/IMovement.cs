namespace NuiN.Movement
{
    public interface IMovement
    {
        void FixedTick();
        void Move(IMovementInput input);
        void Rotate(IMovementInput input);
        void Jump();
    }
}