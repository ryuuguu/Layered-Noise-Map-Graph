using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AnthillPlan.LayeredNM {

    [CustomPropertyDrawer(typeof(RuntimeVariable))]
    public class RuntimeVariableDrawer : PropertyDrawer {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row; 
            var name = new Label(property.FindPropertyRelative("name").stringValue);
            container.Add(name);
            
            // not used now but may be need for subgraphs in a later graph toolkit version
            //var kind = new Label(property.FindPropertyRelative("variableKind").stringValue);
            //container.Add(kind);
            
            var datatype = property.FindPropertyRelative("dataType").stringValue; 
            
            var intString = typeof(int).ToString();
            if (intString == datatype) {
                var intVal = new PropertyField(property.FindPropertyRelative("intVal"));
                container.Add(intVal);
            }
            var floatString = typeof(float).ToString();
            if (floatString == datatype) {
                var floatVal = new PropertyField(property.FindPropertyRelative("floatVal"));
                container.Add(floatVal);
            }
            var flatMapString = typeof(float[,]).ToString();
            if (flatMapString == datatype) {
                var flatMap = new PropertyField(property.FindPropertyRelative("flatMap"));
                container.Add(flatMap);
            }
            
            return container;
        }
        
      
    }
   
}
