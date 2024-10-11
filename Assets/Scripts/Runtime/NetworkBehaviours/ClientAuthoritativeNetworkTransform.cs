using Unity.Netcode.Components;
using UnityEngine;

namespace Runtime.NetworkBehaviours
{
    [DisallowMultipleComponent]
    public class ClientAuthoritativeNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
