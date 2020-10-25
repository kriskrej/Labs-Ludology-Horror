using UnityEngine;

public static class Consts {
    public static class Layers {
        public static readonly LayerMask wallsFloorsCeilingsMask = LayerMask.GetMask("Walls");
    }
}

namespace Code.GameManagers {
    public class GameManager {
        public static bool Exists() {
            return true;
        }

        public class Instance {
            public class player {
                public static Camera playerCamera {
                    get { return Camera.main; }
                }
            }
        }
    }
}