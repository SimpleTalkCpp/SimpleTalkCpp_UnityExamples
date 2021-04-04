// GENERATED AUTOMATICALLY FROM 'Assets/Lesson 015/Input/Lesson015_InputActions_Asset.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Lesson015_InputActions_Asset : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Lesson015_InputActions_Asset()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Lesson015_InputActions_Asset"",
    ""maps"": [
        {
            ""name"": ""MyGameMode"",
            ""id"": ""8d867c1f-77f9-4c07-8686-b120a033c5c7"",
            ""actions"": [
                {
                    ""name"": ""Action_MoveDown"",
                    ""type"": ""Value"",
                    ""id"": ""b2b0fa38-ec3a-489d-a122-bc68806947fc"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Action_Attack"",
                    ""type"": ""Button"",
                    ""id"": ""6c4267ee-5b2f-49b2-91df-74ebeef09dd8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""10af5c3e-58b6-487c-a50e-ee3e9ca51e84"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Action_MoveDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d655e90e-f57f-4711-966b-a5537732153d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Action_MoveDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5aeded1d-4866-4230-8f48-5a4c269bcd42"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Action_MoveDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7f8cc895-35b3-4b16-b797-e170dd4a115b"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Action_Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""SwimmingMode"",
            ""id"": ""dba03386-a5be-4b38-90a7-70c0f23a492b"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""7b79ec14-74cf-4406-a1b4-2da4a1759d76"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""221595cf-9298-4ed8-9f0a-78d3c5a741cd"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // MyGameMode
        m_MyGameMode = asset.FindActionMap("MyGameMode", throwIfNotFound: true);
        m_MyGameMode_Action_MoveDown = m_MyGameMode.FindAction("Action_MoveDown", throwIfNotFound: true);
        m_MyGameMode_Action_Attack = m_MyGameMode.FindAction("Action_Attack", throwIfNotFound: true);
        // SwimmingMode
        m_SwimmingMode = asset.FindActionMap("SwimmingMode", throwIfNotFound: true);
        m_SwimmingMode_Newaction = m_SwimmingMode.FindAction("New action", throwIfNotFound: true);
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

    // MyGameMode
    private readonly InputActionMap m_MyGameMode;
    private IMyGameModeActions m_MyGameModeActionsCallbackInterface;
    private readonly InputAction m_MyGameMode_Action_MoveDown;
    private readonly InputAction m_MyGameMode_Action_Attack;
    public struct MyGameModeActions
    {
        private @Lesson015_InputActions_Asset m_Wrapper;
        public MyGameModeActions(@Lesson015_InputActions_Asset wrapper) { m_Wrapper = wrapper; }
        public InputAction @Action_MoveDown => m_Wrapper.m_MyGameMode_Action_MoveDown;
        public InputAction @Action_Attack => m_Wrapper.m_MyGameMode_Action_Attack;
        public InputActionMap Get() { return m_Wrapper.m_MyGameMode; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MyGameModeActions set) { return set.Get(); }
        public void SetCallbacks(IMyGameModeActions instance)
        {
            if (m_Wrapper.m_MyGameModeActionsCallbackInterface != null)
            {
                @Action_MoveDown.started -= m_Wrapper.m_MyGameModeActionsCallbackInterface.OnAction_MoveDown;
                @Action_MoveDown.performed -= m_Wrapper.m_MyGameModeActionsCallbackInterface.OnAction_MoveDown;
                @Action_MoveDown.canceled -= m_Wrapper.m_MyGameModeActionsCallbackInterface.OnAction_MoveDown;
                @Action_Attack.started -= m_Wrapper.m_MyGameModeActionsCallbackInterface.OnAction_Attack;
                @Action_Attack.performed -= m_Wrapper.m_MyGameModeActionsCallbackInterface.OnAction_Attack;
                @Action_Attack.canceled -= m_Wrapper.m_MyGameModeActionsCallbackInterface.OnAction_Attack;
            }
            m_Wrapper.m_MyGameModeActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Action_MoveDown.started += instance.OnAction_MoveDown;
                @Action_MoveDown.performed += instance.OnAction_MoveDown;
                @Action_MoveDown.canceled += instance.OnAction_MoveDown;
                @Action_Attack.started += instance.OnAction_Attack;
                @Action_Attack.performed += instance.OnAction_Attack;
                @Action_Attack.canceled += instance.OnAction_Attack;
            }
        }
    }
    public MyGameModeActions @MyGameMode => new MyGameModeActions(this);

    // SwimmingMode
    private readonly InputActionMap m_SwimmingMode;
    private ISwimmingModeActions m_SwimmingModeActionsCallbackInterface;
    private readonly InputAction m_SwimmingMode_Newaction;
    public struct SwimmingModeActions
    {
        private @Lesson015_InputActions_Asset m_Wrapper;
        public SwimmingModeActions(@Lesson015_InputActions_Asset wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_SwimmingMode_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_SwimmingMode; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SwimmingModeActions set) { return set.Get(); }
        public void SetCallbacks(ISwimmingModeActions instance)
        {
            if (m_Wrapper.m_SwimmingModeActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_SwimmingModeActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_SwimmingModeActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_SwimmingModeActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_SwimmingModeActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public SwimmingModeActions @SwimmingMode => new SwimmingModeActions(this);
    public interface IMyGameModeActions
    {
        void OnAction_MoveDown(InputAction.CallbackContext context);
        void OnAction_Attack(InputAction.CallbackContext context);
    }
    public interface ISwimmingModeActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
