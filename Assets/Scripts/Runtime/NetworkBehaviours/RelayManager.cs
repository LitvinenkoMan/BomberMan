using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace MonoBehaviours.Network
{
    public class RelayManager : MonoBehaviour
    {
        public string JoinCode { get; private set; }

        public static RelayManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async void Start()
        {
            await UnityServices.InitializeAsync();
            await SignInAnonymously();
        }

        private async Task SignInAnonymously()
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Signed in as: {AuthenticationService.Instance.PlayerId}");
        }

        public async Task<Allocation> CreateRelay(int maxPlayers)
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
                Debug.Log($"Relay created with allocation ID: {allocation.AllocationId}");
                return allocation;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        public async Task<string> GetJoinCode(Allocation allocation)
        {
            try
            {
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log($"Join code: {joinCode}");
                JoinCode = joinCode;
                return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        public async Task<JoinAllocation> JoinRelay(string joinCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                Debug.Log($"Joined Relay with join code: {joinCode}");
                JoinCode = joinCode;
                return joinAllocation;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        // public async Task<Allocation[]> GetListOfRelays()
        // {
        //     RelayServer ;
        //     
        // }
    }
}     