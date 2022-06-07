namespace Player
{
    public abstract class MoveStrategy
    {
        protected PlayerController controller;

        public float moveSpeed;
        public abstract void Move();
    }
}
