using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDefine : MonoBehaviour
{
    public const string GOOGLE_PLAY_STORE = "https://play.google.com/store/apps/details?id=com.SunZero.CosmicHexaPuzzle";
    public const string APPLE_APP_STORE = "";

    public const float THIRD_PARTY_INIT_TIME = 1f;

    public const int MAX_CHAPTER = 4;
    public enum RewardType
    {
        Gold,
        Gem,
    }
}
