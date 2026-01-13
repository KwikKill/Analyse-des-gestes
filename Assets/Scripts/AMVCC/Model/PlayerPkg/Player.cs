using UnityEngine;

namespace AMVCC.Model.PlayerPkg
{
    public class Player
    {
        public Vector3 Destination;
        public CharacterClass CharacterClass;
        public float Speed = 0.05f;
        public float JumpSpeed = 0.05f;

        public Player(CharacterClass characterClass)
        {
            Destination = Vector3.zero;
            CharacterClass = characterClass;
        }
    }
}
