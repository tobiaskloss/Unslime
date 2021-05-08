using System.Runtime.CompilerServices;
using MLAPI.Spawning;
using MLAPI.Transports.UNET;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MLAPI.Extensions
{

    [DisallowMultipleComponent]
    public class NetworkManagerHud : MonoBehaviour
    {
        NetworkManager m_NetworkManager;
        UNetTransport m_Transport;

         
        [SerializeField] private TMP_InputField _portInputField;
        [SerializeField] private TMP_InputField _hostInputField;

        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;

        [SerializeField] private CanvasGroup _canvasGroup;

        void Awake()
        {
            // Only cache networking manager but not transport here because transport could change anytime.
            m_NetworkManager = FindObjectOfType<NetworkManager>();
            m_Transport = (UNetTransport) m_NetworkManager.NetworkConfig.NetworkTransport;

        }

        public void Host()
        {
            m_NetworkManager.StartHost(new Vector3(0, 0, 0), Quaternion.identity, true,
                NetworkSpawnManager.GetPrefabHashFromGenerator("SNAIL"));
            HideUI();
        }

        public void Join()
        {
            m_Transport.ConnectAddress = _hostInputField.text;
            ushort.TryParse(_portInputField.text, out ushort port);
            m_Transport.ConnectPort = port;
            m_NetworkManager.StartClient();
            HideUI();
        }

        private void HideUI()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0;
        }
        
        private void ShowUI()
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1;
        }

    }


}