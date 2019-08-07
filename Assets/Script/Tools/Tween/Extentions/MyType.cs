namespace Saro {
    struct bool3 {
        public bool x, y, z;

        public bool AnyTrue { get { return x || y || z; } }
        public bool AllTrue { get { return x && y && z; } }
    }

    struct bool4 {
        public bool x, y, z, w;

        public bool AnyTrue { get { return x || y || z || w; } }
        public bool AllTrue { get { return x && y && z && w; } }
    }
}