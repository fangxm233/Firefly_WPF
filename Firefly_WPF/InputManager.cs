using System;
using System.Collections.Generic;
using System.Windows;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Firefly
{
    public class InputManager
    {
        private static Dictionary<Key, bool> _pressedKeys = new Dictionary<Key, bool>();
        private static bool _mouseLeftDown;
        private static Vector2 _mouseMovement = Vector2.Zero;
        private static Vector2 _mousePos = Vector2.Zero;

        public static void KeyDown(Key key)
        {
            _pressedKeys[key] = true;
        }

        public static void KeyUp(Key key)
        {
            _pressedKeys[key] = false;
        }

        public static bool IsKeyPressed(Key key)
        {
            return _pressedKeys.ContainsKey(key) && _pressedKeys[key];
        }

        public static void MouseDown()
        {
            _mouseLeftDown = true;
        }

        public static void MouseUp()
        {
            _mouseLeftDown = false;
        }

        public static bool IsMouseDown()
        {
            return _mouseLeftDown;
        }

        public static void MouseMove(Point position)
        {
            Vector2 newPos = new Vector2((float)position.X, (float)position.Y);
            _mouseMovement = newPos - _mousePos;
            _mousePos = newPos;
        }

        public static void ClearMouseMove()
        {
            _mouseMovement = Vector2.Zero;
        }

        public static Vector2 GetMouseMove()
        {
            return _mouseMovement;
        }
    }
}
