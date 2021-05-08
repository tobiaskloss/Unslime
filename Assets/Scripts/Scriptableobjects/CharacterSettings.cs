using UnityEngine;

namespace Scriptableobjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Character Settings", order = 1)]

    public class CharacterSettings: ScriptableObject
    {
        public float jumpForce;
        public float walkSpeed;
    }
}