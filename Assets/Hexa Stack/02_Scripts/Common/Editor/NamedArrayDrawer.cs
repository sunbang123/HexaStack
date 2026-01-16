using UnityEngine;
using UnityEditor;
using HexaStack.Core;

// 에디터 전용 코드 (빌드에 포함 안 됨)
[CustomPropertyDrawer(typeof(NamedArrayAttribute))]
public class NamedArrayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        try
        {
            // 속성에서 Enum 타입을 가져옴
            NamedArrayAttribute namedAttribute = attribute as NamedArrayAttribute;

            // 현재 그려지고 있는 배열의 인덱스 파싱 ("[0]", "[1]" 등)
            int index = int.Parse(property.propertyPath.Split('[', ']')[1]);

            // Enum 이름 가져오기
            string[] enumNames = System.Enum.GetNames(namedAttribute.TargetEnum);

            // 범위 내에 있다면 라벨 교체
            if (index < enumNames.Length)
            {
                label.text = enumNames[index];
            }
            else
            {
                label.text = $"Unknown ({index})"; // Enum 개수보다 배열이 더 길 때
            }
        }
        catch
        {
            // 뭔가 파싱 에러나면 그냥 원래대로 표시
        }

        // 원래대로 그리기 (라벨만 바뀐 상태)
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}