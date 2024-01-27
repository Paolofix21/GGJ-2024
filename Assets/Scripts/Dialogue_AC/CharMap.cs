using System.Collections.Generic;
using UnityEngine;

namespace Advepa.SchoolMetaverse.Laboratori
{
    [CreateAssetMenu(fileName = "CharMap", menuName = "Advepa/MusicLab/CharMap")]
    public class CharMap : ScriptableObject
    {
        public List<MappedChars> DialogueChars = new();
        public Dictionary<char, AudioClip> mappedInfo = new();
    }
}

