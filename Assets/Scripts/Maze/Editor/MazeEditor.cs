using UnityEditor;
using UnityEngine.UIElements;

namespace Maze.Editor
{
    [CustomEditor(typeof(Maze))]
    public class MazeEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            
            return base.CreateInspectorGUI();
        }
    }
}