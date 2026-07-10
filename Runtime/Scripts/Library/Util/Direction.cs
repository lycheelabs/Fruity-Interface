using UnityEngine;
using System.Collections.ObjectModel;
using System;

namespace LycheeLabs.FruityInterface {

    public enum Direction {
        LEFT, UP, RIGHT, DOWN, NONE
    }

    public enum DirectionTurn {
        STRAIGHT, TURN_LEFT, TURN_RIGHT, REVERSE
    }

    // Define an extension method in a non-nested static class. 
    public static class Directions {

        public static readonly ReadOnlyCollection<Direction> All 
            = CreateArray(Direction.LEFT, Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.NONE);
        public static readonly ReadOnlyCollection<Direction> NonZero 
            = CreateArray(Direction.LEFT, Direction.UP, Direction.RIGHT, Direction.DOWN);
        public static readonly ReadOnlyCollection<Direction> None 
            = CreateArray(Direction.NONE);

        private static int[] xIndices;
        private static int[] zIndices;

        private static Direction[] reversals;
        private static Direction[] rotationsCW;
        private static Direction[] rotationsACW;

        static Directions () {
            xIndices = new int[5];
            xIndices[(int)Direction.LEFT] = -1;
            xIndices[(int)Direction.RIGHT] = 1;

            zIndices = new int[5];
            zIndices[(int)Direction.UP] = 1;
            zIndices[(int)Direction.DOWN] = -1;

            reversals = new Direction[5];
            reversals[(int)Direction.NONE] = Direction.NONE;
            reversals[(int)Direction.LEFT] = Direction.RIGHT;
            reversals[(int)Direction.RIGHT] = Direction.LEFT;
            reversals[(int)Direction.UP] = Direction.DOWN;
            reversals[(int)Direction.DOWN] = Direction.UP;

            rotationsCW = new Direction[5];
            rotationsCW[(int)Direction.NONE] = Direction.NONE;
            rotationsCW[(int)Direction.LEFT] = Direction.UP;
            rotationsCW[(int)Direction.UP] = Direction.RIGHT;
            rotationsCW[(int)Direction.RIGHT] = Direction.DOWN;
            rotationsCW[(int)Direction.DOWN] = Direction.LEFT;

            rotationsACW = new Direction[5];
            rotationsACW[(int)Direction.NONE] = Direction.NONE;
            rotationsACW[(int)Direction.LEFT] = Direction.DOWN;
            rotationsACW[(int)Direction.UP] = Direction.LEFT;
            rotationsACW[(int)Direction.RIGHT] = Direction.UP;
            rotationsACW[(int)Direction.DOWN] = Direction.RIGHT;
        }

        private static ReadOnlyCollection<Direction> CreateArray (params Direction[] directions) {
            return Array.AsReadOnly(directions);
        }

        // -----------------------------------------------------

        public static Direction Random (bool includeNone = false) {
            if (includeNone) {
                return All[UnityEngine.Random.Range(0, All.Count)];
            } else {
                return NonZero[UnityEngine.Random.Range(0, NonZero.Count)];
            }
        }

        // -----------------------------------------------------

        public static int XIndex (this Direction direction) {
            return xIndices[(int)direction];
        }

        public static int ZIndex (this Direction direction) {
            return zIndices[(int)direction];
        }

        public static bool IsHorizontal (this Direction direction) {
            return direction == Direction.LEFT || direction == Direction.RIGHT;
        }

        public static bool IsVertical (this Direction direction) {
            return direction == Direction.UP || direction == Direction.DOWN;
        }

        public static Vector3 ToVector (this Direction direction) {
            return new Vector3(xIndices[(int)direction], 0, zIndices[(int)direction]);
        }

        public static Vector2 ToVector2 (this Direction direction) {
            return new Vector2(xIndices[(int)direction], zIndices[(int)direction]);
        }

        public static Vector2Int ToIntVector (this Direction direction) {
            return new Vector2Int(xIndices[(int)direction], zIndices[(int)direction]);
        }

        public static Direction Reverse (this Direction direction) {
            return reversals[(int)direction];
        }

        public static Direction RotateCW (this Direction direction) {
            return rotationsCW[(int)direction];
        }

        public static Direction RotateCCW (this Direction direction) {
            return reversals[(int)rotationsCW[(int)direction]];
        }

        public static bool IsNone (this Direction direction) {
            return direction == Direction.NONE;
        }

        public static float Angle (this Direction direction) {
            switch (direction) {
                case Direction.RIGHT: return 0;
                case Direction.UP: return 90;
                case Direction.LEFT: return 180;
                case Direction.DOWN: return 270;
            }
            return 0;
        }

        public static Direction FromAngle (float angleDegrees) {
            // Normalize to [0,360)
            angleDegrees = (angleDegrees % 360 + 360) % 360;

            // Find nearest 90� sector
            if (angleDegrees < 45f || angleDegrees >= 315f)
                return Direction.RIGHT;
            if (angleDegrees < 135f)
                return Direction.UP;
            if (angleDegrees < 225f)
                return Direction.LEFT;
            return Direction.DOWN;
        }

        public static Direction FromVector (Vector2Int vector) {
            if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y)) {
                return vector.x > 0 ? Direction.RIGHT : Direction.LEFT;
            }
            if (vector.y > 0) return Direction.UP;
            if (vector.y < 0) return Direction.DOWN;
            return Direction.NONE;
        }

        public static DirectionTurn TurnTowards (this Direction fromDirection, Direction toDirection) {
            if (reversals[(int)fromDirection] == toDirection) {
                return DirectionTurn.REVERSE;
            }
            if (rotationsCW[(int)fromDirection] == toDirection) {
                return DirectionTurn.TURN_RIGHT;
            }
            if (rotationsACW[(int)fromDirection] == toDirection) {
                return DirectionTurn.TURN_LEFT;
            }
            return DirectionTurn.STRAIGHT;
        }

    }

}