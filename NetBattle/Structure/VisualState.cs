namespace NetBattle.Structure {
    public struct VisualState {
        public string Animation { get; }
        public float RootTime { get; }
        public float AnimationSpeed { get; }

        public VisualState(string animation, float rootTime, float animationSpeed) {
            Animation = animation;
            RootTime = rootTime;
            AnimationSpeed = animationSpeed;
        }
    }
}