using System;

namespace NetBattle.Structure {
    public struct Sound {
        public Guid Id { get; }
        public float BaseTime { get; }
        public float ClipStartTime { get; }
        public float ClipEndTime { get; }
        public string Resource { get; }
        public bool Spatial { get; }
        public FVector3 Location { get; }

        public Sound(string resource, float baseTime, float clipStartTime, float clipEndTime) {
            Id = Guid.NewGuid();
            BaseTime = baseTime;
            ClipStartTime = clipStartTime;
            ClipEndTime = clipEndTime;
            Resource = resource;
            Spatial = false;
            Location = FVector3.Zero;
        }

        public Sound(string resource, FVector3 location, float baseTime, float clipStartTime, float clipEndTime) {
            Id = Guid.NewGuid();
            BaseTime = baseTime;
            ClipStartTime = clipStartTime;
            ClipEndTime = clipEndTime;
            Resource = resource;
            Spatial = true;
            Location = location;
        }
    }
}