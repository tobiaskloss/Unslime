using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Spawning;
using Unity.Mathematics;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{

    public Transform snailSpawn;
    public Transform kidSpawn;

    private int counter = 1;
    
    private void Start() 
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, MLAPI.NetworkManager.ConnectionApprovedDelegate callback)
    {
        //Your logic here
        bool approve = true;
        bool createPlayerObject = true;

        // The prefab hash. Use null to use the default player prefab
        // If using this hash, replace "MyPrefabHashGenerator" with the name wof a prefab added to the NetworkPrefabs field of your NetworkManager object in the scene
        ulong? prefabHashKid = NetworkSpawnManager.GetPrefabHashFromGenerator("KID");
        ulong? prefabHashSnail = NetworkSpawnManager.GetPrefabHashFromGenerator("SNAIL");
    
        //If approve is true, the connection gets added. If it's false. The client gets disconnected
        if (counter % 2 == 1)
        {
            callback(createPlayerObject, prefabHashKid, approve, kidSpawn.position, quaternion.identity);
        }
        else
        {
            callback(createPlayerObject, prefabHashSnail, approve, kidSpawn.position, quaternion.identity);
        }

        counter++;
    }
}
