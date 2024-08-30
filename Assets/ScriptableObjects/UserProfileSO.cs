using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UserProfile", menuName = "ScriptableObjects/NewUserProfile", order = 1)]
public class UserProfileSO : ScriptableObject
{
    // ok so we need
    public enum UserStatus { New, Old }
    public UserStatus CurrentUserStatus;
    public string PlayerName;
    public int PlayerTotalOwnedCards;
    public string PlayerLevel;  // lets just store it in string for right now
}
