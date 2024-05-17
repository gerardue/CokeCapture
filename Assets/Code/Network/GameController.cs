using System;
using UnityEngine;

public enum UserType
{
    Master, 
    User
}

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public UserType userType;

    [Header("Networks")]
    [SerializeField]
    private GameObject m_masterNetworkPrefab;

    [SerializeField]
    private GameObject m_userNetworkPrefab; 
    
    private void Awake()
    {
        Instance = this;

        switch (userType)
        {
            case UserType.Master:
                Instantiate(m_masterNetworkPrefab);
                // m_masterNetwork.SetActive(true);
                // gameObject.AddComponent<MasterNetworkEvents>(); 
                break;
            case UserType.User:
                Instantiate(m_userNetworkPrefab);
                // m_masterNetwork.SetActive(true);
                // gameObject.AddComponent<UserNetworkEvents>(); 
                break;
        }
    }
}
