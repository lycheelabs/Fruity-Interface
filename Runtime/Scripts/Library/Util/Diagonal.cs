using UnityEngine;
using System.Collections.ObjectModel;
using System;

namespace LycheeLabs.FruityInterface {

    public enum Diagonal {
        UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT, NONE
    }

    public static class Diagonals {

        public static readonly ReadOnlyCollection<Diagonal> All
            = CreateArray(Diagonal.UP_LEFT, Diagonal.UP_RIGHT, Diagonal.DOWN_LEFT, Diagonal.DOWN_RIGHT, Diagonal.NONE);
        public static readonly ReadOnlyCollection<Diagonal> NonZero
            = CreateArray(Diagonal.UP_LEFT, Diagonal.UP_RIGHT, Diagonal.DOWN_LEFT, Diagonal.DOWN_RIGHT);
        public static readonly ReadOnlyCollection<Diagonal> None
            = CreateArray(Diagonal.NONE);
        
        private static int[] xIndices;
        private static int[] zIndices;

        private static Diagonal[] reversals;
        private static Diagonal[] rotationsCW;
        private static Diagonal[] rotationsACW;

        static Diagonals () {
            xIndices = new int[5];
            xIndices[(int)Diagonal.UP_LEFT] = -1;
            xIndices[(int)Diagonal.UP_RIGHT] = 1;
            xIndices[(int)Diagonal.DOWN_LEFT] = -1;
            xIndices[(int)Diagonal.DOWN_RIGHT] = 1;

            zIndices = new int[5];
            zIndices[(int)Diagonal.UP_LEFT] = 1;
            zIndices[(int)Diagonal.UP_RIGHT] = 1;
            zIndices[(int)Diagonal.DOWN_LEFT] = -1;
            zIndices[(int)Diagonal.DOWN_RIGHT] = -1;

            reversals = new Diagonal[5];
            reversals[(int)Diagonal.NONE] = Diagonal.NONE;
            reversals[(int)Diagonal.UP_LEFT] = Diagonal.DOWN_RIGHT;
            reversals[(int)Diagonal.UP_RIGHT] = Diagonal.DOWN_LEFT;
            reversals[(int)Diagonal.DOWN_LEFT] = Diagonal.UP_RIGHT;
            reversals[(int)Diagonal.DOWN_RIGHT] = Diagonal.UP_LEFT;

            rotationsCW = new Diagonal[5];
            rotationsCW[(int)Diagonal.NONE] = Diagonal.NONE;
            rotationsCW[(int)Diagonal.UP_LEFT] = Diagonal.UP_RIGHT;
            rotationsCW[(int)Diagonal.UP_RIGHT] = Diagonal.DOWN_RIGHT;
            rotationsCW[(int)Diagonal.DOWN_RIGHT] = Diagonal.DOWN_LEFT;
            rotationsCW[(int)Diagonal.DOWN_LEFT] = Diagonal.UP_LEFT;

            rotationsACW = new Diagonal[5];
            rotationsACW[(int)Diagonal.NONE] = Diagonal.NONE;
            rotationsACW[(int)Diagonal.UP_LEFT] = Diagonal.DOWN_LEFT;
            rotationsACW[(int)Diagonal.DOWN_LEFT] = Diagonal.DOWN_RIGHT;
            rotationsACW[(int)Diagonal.DOWN_RIGHT] = Diagonal.UP_RIGHT;
            rotationsACW[(int)Diagonal.UP_RIGHT] = Diagonal.UP_LEFT;
        }

        private static ReadOnlyCollection<Diagonal> CreateArray(params Diagonal[] diagonals) {
            return Array.AsReadOnly(diagonals);
        }


        // -----------------------------------------------------

        public static Diagonal Random (bool includeNone = false) {
            if (includeNone) {
                return All[UnityEngine.Random.Range(0, All.Count)];
            } else {
                return NonZero[UnityEngine.Random.Range(0, NonZero.Count)];
            }
        }

        // -----------------------------------------------------

        public static int XIndex (this Diagonal direction) {
            return xIndices[(int)direction];
        }

        public static int ZIndex (this Diagonal direction) {
            return zIndices[(int)direction];
        }

        public static Vector3 ToVector (this Diagonal direction) {
            return new Vector3(xIndices[(int)direction], 0, zIndices[(int)direction]);
        }

        public static Vector2 ToVector2 (this Diagonal direction) {
            return new Vector2(xIndices[(int)direction], zIndices[(int)direction]);
        }

        public static Vector2Int ToIntVector (this Diagonal direction) {
            return new Vector2Int(xIndices[(int)direction], zIndices[(int)direction]);
        }

        public static Diagonal Reverse (this Diagonal direction) {
            return reversals[(int)direction];
        }

        public static Diagonal RotateCW (this Diagonal direction) {
            return rotationsCW[(int)direction];
        }

        public static Diagonal RotateCCW (this Diagonal direction) {
            return reversals[(int)rotationsCW[(int)direction]];
        }

        public static bool IsNone (this Diagonal direction) {
            return direction == Diagonal.NONE;
        }

    }

}