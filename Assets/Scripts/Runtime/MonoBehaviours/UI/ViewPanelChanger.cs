// This script is Used to easely navigate between Panels on Canvas
// It's using New Input system for unity


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Runtime.MonoBehaviours.UI
{
    public class ViewPanelChanger : MonoBehaviour
    {
        [SerializeField]
        private RectTransform MainPanel;
        [SerializeField,
         Tooltip("Will Hide main panel if any other is opened, and will show it if every other panel is closed")]
        private bool HideMainPanel = true;
        [SerializeField, Tooltip("Will Hide any other panel which is under the current one")]
        private bool HidePreviousPanels = true;

        [SerializeField]
        private UnityEvent OnQuitWhileInMainPanelEvent;


        private PlayerMainControls input;
        private Stack<RectTransform> PanelsStack;

        private InputAction QuitAction;

        private void Awake()
        {
            PanelsStack = new Stack<RectTransform>(8);
            input = new PlayerMainControls();
            
            input.PlayerMainActionMaps.Enable();
            QuitAction = input.PlayerMainActionMaps.Quit;
        }

        private void OnEnable()
        {
            QuitAction.performed += RemovePanelWrapper;
        }

        private void OnDisable()
        {
            QuitAction.performed -= RemovePanelWrapper;
        }

        // privat 
        /// <summary>
        /// Will Add new RectTransform to Stack and Make it Active
        /// </summary>
        /// <param name="PanelTransform">RectTransform component of the Panel</param>
        /// <param name="hideMainPanel">Should this added panel Hide Main View?</param>
        public void AddPanel(RectTransform PanelTransform)
        {
            if (HideMainPanel)
            {
                MainPanel.gameObject.SetActive(false);
            }

            if (HidePreviousPanels && PanelsStack.Count > 0)
            {
                PanelsStack.Peek().gameObject.SetActive(false);    
            }
            
            PanelsStack.Push(PanelTransform);
            PanelTransform.gameObject.SetActive(true);
        }

        /// <summary>
        /// Disables Last opened Panel if it was open by "AddPanel" method;
        /// </summary>
        public void RemovePanel()
        {
            if (PanelsStack.Count > 0)
            {
                var panel = PanelsStack.Pop();
                panel.gameObject.SetActive(false);

                if (HideMainPanel && PanelsStack.Count == 0)        //There we check if all panels are closed and if MainPanel was hiden, then we will enable it if so
                {
                    MainPanel.gameObject.SetActive(true);
                }
                
                if (HidePreviousPanels && PanelsStack.Count > 0)
                {
                    PanelsStack.Peek().gameObject.SetActive(true);    
                }
            }
            else
            {
                OnQuitWhileInMainPanelEvent?.Invoke();
            }
        }

        /// <summary>
        /// Closes all opened panels by relying on "HideMainPanel" And "HidePreviousPanels"  
        /// </summary>
        public void CloseAllPanels()
        {
            while (PanelsStack.Count > 0)
            {
                RemovePanel();
            }
        }

        private void RemovePanelWrapper(InputAction.CallbackContext context)
        {
            RemovePanel();
        }
    }
}
