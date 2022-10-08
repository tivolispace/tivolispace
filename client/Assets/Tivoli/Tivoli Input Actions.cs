//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.2
//     from Assets/Tivoli/Tivoli Input Actions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @TivoliInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @TivoliInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Tivoli Input Actions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""d20a1b4c-7eed-4cec-a338-89053621fd26"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""3c210c2d-07d0-4bbb-b712-dd8e0f2d87b4"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Turn"",
                    ""type"": ""Value"",
                    ""id"": ""4c510699-ea13-466e-bebe-6d48a72b5afc"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""ToggleMainMenu"",
                    ""type"": ""Button"",
                    ""id"": ""c5496161-aab4-476d-881e-7cbb8d416459"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""8f00b49b-8a71-48f4-b189-9d869de909bf"",
                    ""path"": ""<XRController>{LeftHand}/joystick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR Controller"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""3380a239-d661-40b7-bbbe-1055530e67fd"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""9a496e56-bfcd-4b7c-8279-c533a27c4970"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""a2659365-e711-4305-a7e3-c5635fe97972"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""fd154507-f807-475a-8ac3-456f67fcc155"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e1acd65a-634b-4c04-9c27-f38287c25e47"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""756ec286-6d77-4514-a38b-b5ba1f002d31"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ToggleMainMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e72f0cb8-1d1c-461e-927e-000b5a6211d7"",
                    ""path"": ""<ValveIndexController>{RightHand}/primaryButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR Controller"",
                    ""action"": ""ToggleMainMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""13d84cde-b5d0-4b39-bde9-34d728781473"",
                    ""path"": ""<XRController>{RightHand}/joystick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR Controller"",
                    ""action"": ""Turn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""VR Tracking"",
            ""id"": ""faa927bb-80c8-49fb-a574-0541c932cbe8"",
            ""actions"": [
                {
                    ""name"": ""CenterEyePosition"",
                    ""type"": ""Value"",
                    ""id"": ""7dde6ec3-b85e-425e-89ff-1fc31038a8a5"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""CenterEyeRotation"",
                    ""type"": ""Value"",
                    ""id"": ""acf5c778-1e49-4c4f-81e1-70737d6e4a9c"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LeftEyePosition"",
                    ""type"": ""Value"",
                    ""id"": ""63e5d755-fc30-418c-ba5c-7bd2c722af83"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LeftEyeRotation"",
                    ""type"": ""Value"",
                    ""id"": ""d47826a4-1676-4f6c-907f-0dfd7295e8d1"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""RightEyePosition"",
                    ""type"": ""Value"",
                    ""id"": ""54b3d767-3517-4051-8cdb-29dec2a9ac8f"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""RightEyeRotation"",
                    ""type"": ""Value"",
                    ""id"": ""4b844974-84a6-4fb2-aba4-f1d2d3b4dfac"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LeftHandPosition"",
                    ""type"": ""Value"",
                    ""id"": ""80373e99-05a4-43fa-9ae5-252bfa7df4f1"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LeftHandRotation"",
                    ""type"": ""Value"",
                    ""id"": ""0d937105-1354-41d2-837a-c8db3a54c6f9"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""RightHandPosition"",
                    ""type"": ""Value"",
                    ""id"": ""0d91adb9-322f-4fff-92f2-2d524b6963be"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""RightHandRotation"",
                    ""type"": ""Value"",
                    ""id"": ""839ccf1b-37ba-44f8-9064-6c14adb15367"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2208d180-f484-4d8f-beff-bfa1c26c2bec"",
                    ""path"": ""<XRHMD>/centerEyePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CenterEyePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e829f89b-e0cc-4f49-ad05-ff88c42fcadc"",
                    ""path"": ""<XRController>{LeftHand}/devicePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR Controller"",
                    ""action"": ""LeftHandPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e37213d8-d74d-4d16-a6f4-d0bc388937eb"",
                    ""path"": ""<XRHMD>/centerEyeRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CenterEyeRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3fa3bbb0-4444-4498-9bed-75bfab4daee8"",
                    ""path"": ""<XRController>{LeftHand}/deviceRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR Controller"",
                    ""action"": ""LeftHandRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""257fc299-41de-4ce1-9c11-7284c8ecf1ef"",
                    ""path"": ""<XRController>{RightHand}/devicePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR Controller"",
                    ""action"": ""RightHandPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8d0a6c67-efc8-486b-abac-1ff6a077714d"",
                    ""path"": ""<XRController>{RightHand}/deviceRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR Controller"",
                    ""action"": ""RightHandRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""71876fcc-cda4-4f1f-9bf1-ee643b190e73"",
                    ""path"": ""<XRHMD>/leftEyePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftEyePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d899a1df-21db-4b93-987f-3795d59c0157"",
                    ""path"": ""<XRHMD>/leftEyeRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftEyeRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""46c96818-47da-4a97-8e38-c7eea9c030e6"",
                    ""path"": ""<XRHMD>/rightEyePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightEyePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1096387c-1a80-4dd3-a9d2-9728c6df5904"",
                    ""path"": ""<XRHMD>/rightEyeRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightEyeRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Unused Player"",
            ""id"": ""2d22cebc-7e29-450c-a48f-656a6a052df7"",
            ""actions"": [
                {
                    ""name"": ""ThirdPersonLook"",
                    ""type"": ""Value"",
                    ""id"": ""ad972676-69d0-4526-83ca-d2d7d155a1a8"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""ThirdPersonActivateLook"",
                    ""type"": ""Button"",
                    ""id"": ""309b5339-dcef-487b-b924-95e8aa733f52"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ThirdPersonBoomLength"",
                    ""type"": ""Value"",
                    ""id"": ""ee34af99-741b-4709-a18a-dfccab732735"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""10c5dd40-76dc-4568-adbd-a2d8f310bc52"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThirdPersonLook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03d76122-b207-478a-9773-5c7622d6405d"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThirdPersonActivateLook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""409e878a-6574-449b-86d9-5e9f4fbd76e1"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThirdPersonBoomLength"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""XR Controller"",
            ""bindingGroup"": ""XR Controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<XRController>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Turn = m_Player.FindAction("Turn", throwIfNotFound: true);
        m_Player_ToggleMainMenu = m_Player.FindAction("ToggleMainMenu", throwIfNotFound: true);
        // VR Tracking
        m_VRTracking = asset.FindActionMap("VR Tracking", throwIfNotFound: true);
        m_VRTracking_CenterEyePosition = m_VRTracking.FindAction("CenterEyePosition", throwIfNotFound: true);
        m_VRTracking_CenterEyeRotation = m_VRTracking.FindAction("CenterEyeRotation", throwIfNotFound: true);
        m_VRTracking_LeftEyePosition = m_VRTracking.FindAction("LeftEyePosition", throwIfNotFound: true);
        m_VRTracking_LeftEyeRotation = m_VRTracking.FindAction("LeftEyeRotation", throwIfNotFound: true);
        m_VRTracking_RightEyePosition = m_VRTracking.FindAction("RightEyePosition", throwIfNotFound: true);
        m_VRTracking_RightEyeRotation = m_VRTracking.FindAction("RightEyeRotation", throwIfNotFound: true);
        m_VRTracking_LeftHandPosition = m_VRTracking.FindAction("LeftHandPosition", throwIfNotFound: true);
        m_VRTracking_LeftHandRotation = m_VRTracking.FindAction("LeftHandRotation", throwIfNotFound: true);
        m_VRTracking_RightHandPosition = m_VRTracking.FindAction("RightHandPosition", throwIfNotFound: true);
        m_VRTracking_RightHandRotation = m_VRTracking.FindAction("RightHandRotation", throwIfNotFound: true);
        // Unused Player
        m_UnusedPlayer = asset.FindActionMap("Unused Player", throwIfNotFound: true);
        m_UnusedPlayer_ThirdPersonLook = m_UnusedPlayer.FindAction("ThirdPersonLook", throwIfNotFound: true);
        m_UnusedPlayer_ThirdPersonActivateLook = m_UnusedPlayer.FindAction("ThirdPersonActivateLook", throwIfNotFound: true);
        m_UnusedPlayer_ThirdPersonBoomLength = m_UnusedPlayer.FindAction("ThirdPersonBoomLength", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Turn;
    private readonly InputAction m_Player_ToggleMainMenu;
    public struct PlayerActions
    {
        private @TivoliInputActions m_Wrapper;
        public PlayerActions(@TivoliInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Turn => m_Wrapper.m_Player_Turn;
        public InputAction @ToggleMainMenu => m_Wrapper.m_Player_ToggleMainMenu;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Turn.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurn;
                @Turn.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurn;
                @Turn.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurn;
                @ToggleMainMenu.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnToggleMainMenu;
                @ToggleMainMenu.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnToggleMainMenu;
                @ToggleMainMenu.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnToggleMainMenu;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Turn.started += instance.OnTurn;
                @Turn.performed += instance.OnTurn;
                @Turn.canceled += instance.OnTurn;
                @ToggleMainMenu.started += instance.OnToggleMainMenu;
                @ToggleMainMenu.performed += instance.OnToggleMainMenu;
                @ToggleMainMenu.canceled += instance.OnToggleMainMenu;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // VR Tracking
    private readonly InputActionMap m_VRTracking;
    private IVRTrackingActions m_VRTrackingActionsCallbackInterface;
    private readonly InputAction m_VRTracking_CenterEyePosition;
    private readonly InputAction m_VRTracking_CenterEyeRotation;
    private readonly InputAction m_VRTracking_LeftEyePosition;
    private readonly InputAction m_VRTracking_LeftEyeRotation;
    private readonly InputAction m_VRTracking_RightEyePosition;
    private readonly InputAction m_VRTracking_RightEyeRotation;
    private readonly InputAction m_VRTracking_LeftHandPosition;
    private readonly InputAction m_VRTracking_LeftHandRotation;
    private readonly InputAction m_VRTracking_RightHandPosition;
    private readonly InputAction m_VRTracking_RightHandRotation;
    public struct VRTrackingActions
    {
        private @TivoliInputActions m_Wrapper;
        public VRTrackingActions(@TivoliInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @CenterEyePosition => m_Wrapper.m_VRTracking_CenterEyePosition;
        public InputAction @CenterEyeRotation => m_Wrapper.m_VRTracking_CenterEyeRotation;
        public InputAction @LeftEyePosition => m_Wrapper.m_VRTracking_LeftEyePosition;
        public InputAction @LeftEyeRotation => m_Wrapper.m_VRTracking_LeftEyeRotation;
        public InputAction @RightEyePosition => m_Wrapper.m_VRTracking_RightEyePosition;
        public InputAction @RightEyeRotation => m_Wrapper.m_VRTracking_RightEyeRotation;
        public InputAction @LeftHandPosition => m_Wrapper.m_VRTracking_LeftHandPosition;
        public InputAction @LeftHandRotation => m_Wrapper.m_VRTracking_LeftHandRotation;
        public InputAction @RightHandPosition => m_Wrapper.m_VRTracking_RightHandPosition;
        public InputAction @RightHandRotation => m_Wrapper.m_VRTracking_RightHandRotation;
        public InputActionMap Get() { return m_Wrapper.m_VRTracking; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(VRTrackingActions set) { return set.Get(); }
        public void SetCallbacks(IVRTrackingActions instance)
        {
            if (m_Wrapper.m_VRTrackingActionsCallbackInterface != null)
            {
                @CenterEyePosition.started -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnCenterEyePosition;
                @CenterEyePosition.performed -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnCenterEyePosition;
                @CenterEyePosition.canceled -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnCenterEyePosition;
                @CenterEyeRotation.started -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnCenterEyeRotation;
                @CenterEyeRotation.performed -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnCenterEyeRotation;
                @CenterEyeRotation.canceled -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnCenterEyeRotation;
                @LeftEyePosition.started -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnLeftEyePosition;
                @LeftEyePosition.performed -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnLeftEyePosition;
                @LeftEyePosition.canceled -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnLeftEyePosition;
                @LeftEyeRotation.started -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnLeftEyeRotation;
                @LeftEyeRotation.performed -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnLeftEyeRotation;
                @LeftEyeRotation.canceled -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnLeftEyeRotation;
                @RightEyePosition.started -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnRightEyePosition;
                @RightEyePosition.performed -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnRightEyePosition;
                @RightEyePosition.canceled -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnRightEyePosition;
                @RightEyeRotation.started -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnRightEyeRotation;
                @RightEyeRotation.performed -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnRightEyeRotation;
                @RightEyeRotation.canceled -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnRightEyeRotation;
                @LeftHandPosition.started -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnLeftHandPosition;
                @LeftHandPosition.performed -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnLeftHandPosition;
                @LeftHandPosition.canceled -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnLeftHandPosition;
                @LeftHandRotation.started -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnLeftHandRotation;
                @LeftHandRotation.performed -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnLeftHandRotation;
                @LeftHandRotation.canceled -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnLeftHandRotation;
                @RightHandPosition.started -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnRightHandPosition;
                @RightHandPosition.performed -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnRightHandPosition;
                @RightHandPosition.canceled -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnRightHandPosition;
                @RightHandRotation.started -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnRightHandRotation;
                @RightHandRotation.performed -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnRightHandRotation;
                @RightHandRotation.canceled -= m_Wrapper.m_VRTrackingActionsCallbackInterface.OnRightHandRotation;
            }
            m_Wrapper.m_VRTrackingActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CenterEyePosition.started += instance.OnCenterEyePosition;
                @CenterEyePosition.performed += instance.OnCenterEyePosition;
                @CenterEyePosition.canceled += instance.OnCenterEyePosition;
                @CenterEyeRotation.started += instance.OnCenterEyeRotation;
                @CenterEyeRotation.performed += instance.OnCenterEyeRotation;
                @CenterEyeRotation.canceled += instance.OnCenterEyeRotation;
                @LeftEyePosition.started += instance.OnLeftEyePosition;
                @LeftEyePosition.performed += instance.OnLeftEyePosition;
                @LeftEyePosition.canceled += instance.OnLeftEyePosition;
                @LeftEyeRotation.started += instance.OnLeftEyeRotation;
                @LeftEyeRotation.performed += instance.OnLeftEyeRotation;
                @LeftEyeRotation.canceled += instance.OnLeftEyeRotation;
                @RightEyePosition.started += instance.OnRightEyePosition;
                @RightEyePosition.performed += instance.OnRightEyePosition;
                @RightEyePosition.canceled += instance.OnRightEyePosition;
                @RightEyeRotation.started += instance.OnRightEyeRotation;
                @RightEyeRotation.performed += instance.OnRightEyeRotation;
                @RightEyeRotation.canceled += instance.OnRightEyeRotation;
                @LeftHandPosition.started += instance.OnLeftHandPosition;
                @LeftHandPosition.performed += instance.OnLeftHandPosition;
                @LeftHandPosition.canceled += instance.OnLeftHandPosition;
                @LeftHandRotation.started += instance.OnLeftHandRotation;
                @LeftHandRotation.performed += instance.OnLeftHandRotation;
                @LeftHandRotation.canceled += instance.OnLeftHandRotation;
                @RightHandPosition.started += instance.OnRightHandPosition;
                @RightHandPosition.performed += instance.OnRightHandPosition;
                @RightHandPosition.canceled += instance.OnRightHandPosition;
                @RightHandRotation.started += instance.OnRightHandRotation;
                @RightHandRotation.performed += instance.OnRightHandRotation;
                @RightHandRotation.canceled += instance.OnRightHandRotation;
            }
        }
    }
    public VRTrackingActions @VRTracking => new VRTrackingActions(this);

    // Unused Player
    private readonly InputActionMap m_UnusedPlayer;
    private IUnusedPlayerActions m_UnusedPlayerActionsCallbackInterface;
    private readonly InputAction m_UnusedPlayer_ThirdPersonLook;
    private readonly InputAction m_UnusedPlayer_ThirdPersonActivateLook;
    private readonly InputAction m_UnusedPlayer_ThirdPersonBoomLength;
    public struct UnusedPlayerActions
    {
        private @TivoliInputActions m_Wrapper;
        public UnusedPlayerActions(@TivoliInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @ThirdPersonLook => m_Wrapper.m_UnusedPlayer_ThirdPersonLook;
        public InputAction @ThirdPersonActivateLook => m_Wrapper.m_UnusedPlayer_ThirdPersonActivateLook;
        public InputAction @ThirdPersonBoomLength => m_Wrapper.m_UnusedPlayer_ThirdPersonBoomLength;
        public InputActionMap Get() { return m_Wrapper.m_UnusedPlayer; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UnusedPlayerActions set) { return set.Get(); }
        public void SetCallbacks(IUnusedPlayerActions instance)
        {
            if (m_Wrapper.m_UnusedPlayerActionsCallbackInterface != null)
            {
                @ThirdPersonLook.started -= m_Wrapper.m_UnusedPlayerActionsCallbackInterface.OnThirdPersonLook;
                @ThirdPersonLook.performed -= m_Wrapper.m_UnusedPlayerActionsCallbackInterface.OnThirdPersonLook;
                @ThirdPersonLook.canceled -= m_Wrapper.m_UnusedPlayerActionsCallbackInterface.OnThirdPersonLook;
                @ThirdPersonActivateLook.started -= m_Wrapper.m_UnusedPlayerActionsCallbackInterface.OnThirdPersonActivateLook;
                @ThirdPersonActivateLook.performed -= m_Wrapper.m_UnusedPlayerActionsCallbackInterface.OnThirdPersonActivateLook;
                @ThirdPersonActivateLook.canceled -= m_Wrapper.m_UnusedPlayerActionsCallbackInterface.OnThirdPersonActivateLook;
                @ThirdPersonBoomLength.started -= m_Wrapper.m_UnusedPlayerActionsCallbackInterface.OnThirdPersonBoomLength;
                @ThirdPersonBoomLength.performed -= m_Wrapper.m_UnusedPlayerActionsCallbackInterface.OnThirdPersonBoomLength;
                @ThirdPersonBoomLength.canceled -= m_Wrapper.m_UnusedPlayerActionsCallbackInterface.OnThirdPersonBoomLength;
            }
            m_Wrapper.m_UnusedPlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ThirdPersonLook.started += instance.OnThirdPersonLook;
                @ThirdPersonLook.performed += instance.OnThirdPersonLook;
                @ThirdPersonLook.canceled += instance.OnThirdPersonLook;
                @ThirdPersonActivateLook.started += instance.OnThirdPersonActivateLook;
                @ThirdPersonActivateLook.performed += instance.OnThirdPersonActivateLook;
                @ThirdPersonActivateLook.canceled += instance.OnThirdPersonActivateLook;
                @ThirdPersonBoomLength.started += instance.OnThirdPersonBoomLength;
                @ThirdPersonBoomLength.performed += instance.OnThirdPersonBoomLength;
                @ThirdPersonBoomLength.canceled += instance.OnThirdPersonBoomLength;
            }
        }
    }
    public UnusedPlayerActions @UnusedPlayer => new UnusedPlayerActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    private int m_XRControllerSchemeIndex = -1;
    public InputControlScheme XRControllerScheme
    {
        get
        {
            if (m_XRControllerSchemeIndex == -1) m_XRControllerSchemeIndex = asset.FindControlSchemeIndex("XR Controller");
            return asset.controlSchemes[m_XRControllerSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnTurn(InputAction.CallbackContext context);
        void OnToggleMainMenu(InputAction.CallbackContext context);
    }
    public interface IVRTrackingActions
    {
        void OnCenterEyePosition(InputAction.CallbackContext context);
        void OnCenterEyeRotation(InputAction.CallbackContext context);
        void OnLeftEyePosition(InputAction.CallbackContext context);
        void OnLeftEyeRotation(InputAction.CallbackContext context);
        void OnRightEyePosition(InputAction.CallbackContext context);
        void OnRightEyeRotation(InputAction.CallbackContext context);
        void OnLeftHandPosition(InputAction.CallbackContext context);
        void OnLeftHandRotation(InputAction.CallbackContext context);
        void OnRightHandPosition(InputAction.CallbackContext context);
        void OnRightHandRotation(InputAction.CallbackContext context);
    }
    public interface IUnusedPlayerActions
    {
        void OnThirdPersonLook(InputAction.CallbackContext context);
        void OnThirdPersonActivateLook(InputAction.CallbackContext context);
        void OnThirdPersonBoomLength(InputAction.CallbackContext context);
    }
}
