

using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class Checkbox : GUIElement
    {

        public Texture2D Box { get; set; }
        public bool Checked { get; set; }

        public override void Update() {
            if (InputManager.IsMouseReleased(0) &&
                Bounds.Contains(InputManager.GetMousePosition())) {
                Checked = !Checked;
                OnAction();
            }
                
        }

    }
}
