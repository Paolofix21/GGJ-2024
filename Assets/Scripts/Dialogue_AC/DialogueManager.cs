using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Advepa.SchoolMetaverse.Laboratori
{
    #region Structs
    [Serializable]
    public struct Message
    {
        public enum MessageType { Regular, Special }
        public MessageType messageType;

        [TextArea(2, 10)] public string message;
    }

    [Serializable]
    public struct MappedChars
    {
        public char Letter;
        [Range(1f, 1.5f)]public float Pitch;
        public AudioClip CharClip;
    }
    #endregion

    public class DialogueManager
    {
        private static float textSpeed = .03f;
        static char current;

        public static async Task DisplayText(List<Message> _messageList, int _stageType, TMP_Text _text, AudioSource _audioSource, CharMap _map, CancellationTokenSource _source, CancellationToken _token)
        {
            if ((_messageList.Count - 1) < _stageType)
            {
                _audioSource.pitch = 1;
                return;
            }

            for (int i = 0; i < _messageList[_stageType].message.Length + 1; i++)
            {
                if (_token.IsCancellationRequested)
                {
                    _source.Dispose();
                    return;
                }
                    
                _text.text = _messageList[_stageType].message.Substring(0, i);

                if (_text.text.Length > 0)
                    current = _text.text[_text.text.Length-1];
                else
                    continue;

                if ((_text.text.Length % 2 == 0) && (char.IsLetter(current)))
                {
                    var value = _map.mappedInfo.Values.ToList().IndexOf(_map.mappedInfo[char.ToLower(current)]);
                    _audioSource.pitch = _map.DialogueChars[value].Pitch;
                    _audioSource.PlayOneShot(_map.mappedInfo[char.ToLower(current)]);
                }
                //await new WaitForSeconds(textSpeed);
                try
                {
                    await Task.Delay((int)(textSpeed * 1000), _token);
                }
                // Dispose if the Task was cancelled 
                catch (TaskCanceledException _)
                {
                    var dispose = _;
                    _source.Dispose();
                    return;
                }
            }
        }
    }
}
