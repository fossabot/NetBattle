namespace NetBattle.Structure {
    public struct VisualState {
        public string Asset { get; }
        public string Animation { get; }
        public float RootTime { get; }
        public float AnimationSpeed { get; }
        public float Warp { get; }
        public bool Visible { get; }
        public bool EnemyVisible { get; }

        public VisualState(string asset, string animation, float rootTime, float animationSpeed, float warp, bool visible, bool enemyVisible) {
            Asset = asset;
            Animation = animation;
            RootTime = rootTime;
            AnimationSpeed = animationSpeed;
            Warp = warp;
            Visible = visible;
            EnemyVisible = enemyVisible;
        }
    }
}