using System;
using System.Collections.Generic;
using System.Linq;
using Core.ScriptableObjects;
using UnityEngine;

namespace Runtime.MonoBehaviours.CharacterSelectionSystem
{
    [Serializable]
    public class CharacterVisualsHanger : MonoBehaviour
    {
        [SerializeField, Tooltip("Used as a reference which will give position and rotation for the Character Visuals")]
        private GameObject placeHolder;
        
        [SerializeField] private List<CharacterData> _charactersDataList;
        
        private Dictionary<string, GameObject> _characterVisualsDictionary;
        private GameObject _currentCharacterVisuals;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _characterVisualsDictionary = new Dictionary<string, GameObject>();
            _charactersDataList.ForEach(characterData =>
            {
                var visuals = Instantiate(characterData.Visuals, transform);
                visuals.transform.position = placeHolder.transform.position;
                visuals.transform.rotation = placeHolder.transform.rotation;
                visuals.GetComponent<Animator>().runtimeAnimatorController = characterData.AnimatorController;
                visuals.SetActive(false);
                
                _characterVisualsDictionary.Add(characterData.Name, visuals);
            });
            _currentCharacterVisuals = _characterVisualsDictionary.First().Value;
        }

        public void ChangeVisuals(CharacterData characterData)
        {
            _currentCharacterVisuals.SetActive(false);
            _currentCharacterVisuals = _characterVisualsDictionary[characterData.Name];
            _currentCharacterVisuals.SetActive(true);
            
            _currentCharacterVisuals.GetComponent<RuntimeAnimatorController>();
        }
    }
}
