using System;
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
        public NetworkManager m_NetworkManager;
        public UNetTransport m_Transport;

         
        [SerializeField] private TMP_InputField _portInputField;
        [SerializeField] private TMP_InputField _hostInputField;
        
        [SerializeField] private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _portInputField.text = "7777";
            _hostInputField.text = "178.200.49.47";
        }

        public void Host()
        {
            m_Transport.ConnectAddress = _hostInputField.text;
            ushort.TryParse(_portInputField.text, out ushort port);
            m_Transport.ConnectPort = port;
            m_NetworkManager.StartHost(new Vector3(0, 0, 0), Quaternion.identity, true, NetworkSpawnManager.GetPrefabHashFromGenerator("KID"));
            HideUI();
        }

        public void Join()
        {
            m_Transport.ConnectAddress = _hostInputField.text;
            ushort.TryParse(_portInputField.text, out ushort port);
            m_Transport.ConnectPort = port;
            m_NetworkManager.StartClient();
            Debug.Log(m_Transport.ConnectAddress);
            Debug.Log(port);
            HideUI();
        }

        public void HideUI()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0;
        }
        
        public void ShowUI()
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1;
        }

    }


}