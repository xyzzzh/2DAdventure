using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    public UnityEvent<Character> OnEventRaised;

    public void RasiedEvent(Character character)
    {
        OnEventRaised?.Invoke(character);
    }
}
