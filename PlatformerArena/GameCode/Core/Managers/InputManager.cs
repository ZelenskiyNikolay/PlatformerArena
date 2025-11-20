using Microsoft.Xna.Framework.Input;

namespace Input
{
    public class InputManager
    {
        private static InputManager _instance = new InputManager();
        private KeyboardState _currentState;
        private KeyboardState _previousState;
        private InputManager() { }
        public static InputManager Instance { get { return _instance; } }
        public void Update()
        {
            _previousState = _currentState;
            _currentState = Keyboard.GetState();
        }

        // Проверка: клавиша нажата в этом кадре (впервые)
        public bool IsKeyPressed(Keys key)
        {
            return _currentState.IsKeyDown(key) && !_previousState.IsKeyDown(key);
        }
        public bool AnyKeyPressed()
        {
            var keys = _currentState.GetPressedKeys();
            if(keys.Length == 0) 
                return false;

            // если хотя бы одна была НЕ нажата на предыдущем кадре
            foreach (var key in keys)
                if (_previousState.IsKeyUp(key))
                    return true;

            return false;

        }
        // Проверка: клавиша удерживается
        public bool IsKeyHeldDown(Keys key)
        {
            return _currentState.IsKeyDown(key);
        }

        // Проверка: клавиша отпущена в этом кадре
        public bool IsKeyReleased(Keys key)
        {
            return !_currentState.IsKeyDown(key) && _previousState.IsKeyDown(key);
        }
     }
}



