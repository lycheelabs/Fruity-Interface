namespace LycheeLabs.FruityInterface.Flow {
    public interface SequenceLayer {
        public bool IsBlockingLayersBelow { get; }
        public bool IsBlockedByLayersAbove { get; set; }

        void Update();
        void Clear();
    }

}
