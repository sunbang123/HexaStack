using UnityEngine;

namespace HexaStack.Core
{
    // 인스펙터에서 배열 이름 대신 Enum 이름을 보여주는 속성
    public class NamedArrayAttribute : PropertyAttribute
    {
        public System.Type TargetEnum;
        public NamedArrayAttribute(System.Type targetEnum)
        {
            TargetEnum = targetEnum;
        }
    }
}