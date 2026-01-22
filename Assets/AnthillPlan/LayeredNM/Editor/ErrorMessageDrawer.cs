using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AnthillPlan.LayeredNM {

    [CustomPropertyDrawer(typeof(ErrorMessage))]
    public class ErrorMessageDrawer : PropertyDrawer {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            var container = new VisualElement();
            var message  = new PropertyField(property.FindPropertyRelative("message"));
            container.Add(message);
            return container;
        }
    }

}
