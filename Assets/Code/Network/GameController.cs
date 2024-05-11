using System;
using UnityEngine;


public enum UserType
{
    Master, 
    User
}

[DefaultExecutionOrder(-1000)]
public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public UserType userType; 
    
    private void Awake()
    {
        Instance = this; 
    }
}
