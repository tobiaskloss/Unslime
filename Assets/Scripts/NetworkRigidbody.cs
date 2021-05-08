using System;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

public class NetworkRigidbody : NetworkBehaviour
{
    public NetworkVariableVector2 netVelocity = new NetworkVariableVector2(new NetworkVariableSettings()
    {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.OwnerOnly,
        SendTickrate = 20
    });
    public NetworkVariableFloat netAngularVelocity = new NetworkVariableFloat(new NetworkVariableSettings()
    {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.OwnerOnly,
        SendTickrate = 20
    });
    public NetworkVariableVector2 netPosition = new NetworkVariableVector2(new NetworkVariableSettings()
    {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.OwnerOnly,
        SendTickrate = 20
    });
    public NetworkVariableFloat netRotation = new NetworkVariableFloat(new NetworkVariableSettings()
    {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.OwnerOnly,
        SendTickrate = 20
    });
    public NetworkVariableUInt netUpdateId = new NetworkVariableUInt(new NetworkVariableSettings()
    {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.OwnerOnly,
        SendTickrate = 20
    });

    [SerializeField]
    bool m_SyncVelocity = true;

    [SerializeField]
    bool m_SyncAngularVelocity = true;

    [SerializeField]
    bool m_SyncPosition = true;

    [SerializeField]
    bool m_SyncRotation = true;
    
    [SerializeField]
    float m_InterpolationTime;

    [Serializable]
    struct InterpolationState
    {
        public Vector2 PositionDelta;
        public float RotationDelta;
        public Vector2 VelocityDelta;
        public float AngularVelocityDelta;
        public float TimeRemaining;
        public float TotalTime;
    }

    uint m_InterpolationChangeId;
    InterpolationState m_InterpolationState;
    Rigidbody2D m_Rigidbody;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    void BeginInterpolation()
    {
        m_InterpolationState = new InterpolationState()
        {
            PositionDelta = netPosition.Value - m_Rigidbody.position,
            RotationDelta = (-1f * m_Rigidbody.rotation) * netRotation.Value,
            VelocityDelta = netVelocity.Value - m_Rigidbody.velocity,
            AngularVelocityDelta = netAngularVelocity.Value - m_Rigidbody.angularVelocity,
            TimeRemaining = m_InterpolationTime,
            TotalTime = m_InterpolationTime
        };
    }

    void FixedUpdate()
    {
        if (!NetworkVariablesInitialized())
        {
            return;
        }

        if (IsOwner)
        {
            bool changed = false;

            if (m_SyncPosition)
            {
                changed |= TryUpdate(netPosition, m_Rigidbody.position);
            }

            if (m_SyncRotation)
            {
                changed |= TryUpdate(netRotation, m_Rigidbody.rotation);
            }

            if (m_SyncVelocity)
            {
                changed |= TryUpdate(netVelocity, m_Rigidbody.velocity);
            }

            if (m_SyncAngularVelocity)
            {
                changed |= TryUpdate(netAngularVelocity, m_Rigidbody.angularVelocity);
            }

            if (changed)
            {
                netUpdateId.Value++;
            }
        }
        else
        {
            if (m_InterpolationChangeId != netUpdateId.Value)
            {
                BeginInterpolation();
                m_InterpolationChangeId = netUpdateId.Value;
            }

            float deltaTime = Time.fixedDeltaTime;
            if (0 < m_InterpolationState.TimeRemaining)
            {
                deltaTime = Mathf.Min(deltaTime, m_InterpolationState.TimeRemaining);
                m_InterpolationState.TimeRemaining -= deltaTime;

                deltaTime /= m_InterpolationState.TotalTime;

                if (m_SyncPosition)
                {
                    m_Rigidbody.position +=
                        m_InterpolationState.PositionDelta * deltaTime;
                }

                if (m_SyncRotation)
                {
                    m_Rigidbody.rotation =
                        m_Rigidbody.rotation * Mathf.Lerp(1f,m_InterpolationState.RotationDelta, deltaTime);
                }

                if (m_SyncVelocity)
                {
                    m_Rigidbody.velocity +=
                        m_InterpolationState.VelocityDelta * deltaTime;
                }

                if (m_SyncAngularVelocity)
                {
                    m_Rigidbody.angularVelocity +=
                        m_InterpolationState.AngularVelocityDelta * deltaTime;
                }
            }
        }
    }

    bool NetworkVariablesInitialized()
    {
        return netVelocity.Settings.WritePermission == NetworkVariablePermission.OwnerOnly;
    }

    static bool TryUpdate(NetworkVariableVector2 variable, Vector2 value)
    {
        var current = variable.Value;
        if (Mathf.Approximately(current.x, value.x)
            && Mathf.Approximately(current.y, value.y))
        {
            return false;
        }

        variable.Value = value;
        return true;
    }

    static bool TryUpdate(NetworkVariableFloat variable, float value)
    {
        var current = variable.Value;
        if (Mathf.Approximately(current, value))
        {
            return false;
        }

        variable.Value = value;
        return true;
    }
}