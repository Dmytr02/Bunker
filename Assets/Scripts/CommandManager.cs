using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommandManager : MonoBehaviourPunCallbacks
{
    private Dictionary<string, MethodInfo> Commands = new Dictionary<string, MethodInfo>();
    [SerializeField] private TMP_InputField textInput;
	[SerializeField] private TMP_Text textOutput;
    [SerializeField] private GameObject chatPanel;
    
    public static CommandManager Instance { get; private set; }
    
    private Dictionary<Type, object> _instances = new Dictionary<Type, object>();
    
    private List<string> _buffer = new List<string>();
    private int _selectedIndex = -1;

    public void AddInstance(object instance)
    {
        _instances.Add(instance.GetType(), instance);   
    }
    private void Awake()
    {
        if(!Instance) Instance = this;
        else Destroy(this);
        AddInstance(this);
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (MethodInfo method in assembly.GetTypes().SelectMany(classType => classType.GetMethods()))
            {
                CommandAtribute attribute = method.GetCustomAttribute<CommandAtribute>();
                if (attribute == null) continue;

                Commands.Add(attribute.CommandName, method);
                Debug.Log(attribute.CommandName + " added");
            }
        }


        textInput.onSubmit.AddListener(OnSubmit);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            chatPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(textInput.gameObject, null);
            _selectedIndex = -1;
        }

        if (chatPanel.activeSelf && Input.GetKeyDown(KeyCode.UpArrow))
        {
            _selectedIndex = Mathf.Min(_selectedIndex + 1, _buffer.Count - 1);
            textInput.text = _buffer[_selectedIndex];
        }
        
        if (chatPanel.activeSelf && Input.GetKeyDown(KeyCode.DownArrow))
        {
            _selectedIndex = Mathf.Max(_selectedIndex - 1, -1);
            if(_selectedIndex != -1) textInput.text = _buffer[_selectedIndex];
            else textInput.text = "";
        }
        
        if (chatPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            textInput.text = "";
            EventSystem.current.SetSelectedGameObject(null);
            chatPanel.SetActive(false);
        }
    }

    private void OnSubmit(string text)
    {
        ProcessCommand(textInput.text);
        textInput.text = "";
        EventSystem.current.SetSelectedGameObject(null);
        chatPanel.SetActive(false);
    }
    
    private void ProcessCommand(string command)
    {
        if(string.IsNullOrEmpty(command)) return;
        
        List<string> tokens = command.Split(' ').ToList();
        
        List<object> args = new List<object>();
         
        if (tokens.Count == 0) return;
        int index = tokens.Count-1;
        while (index>=0)
        {
            if (!Commands.TryGetValue(tokens[index], out MethodInfo method))
            {
                args.Add(tokens[index]);
                index--;
                continue;
            }

            if (method.GetParameters().Length > tokens.Count - 1)
            {
                Debug.Log($"params Count is not corresponds to params of {tokens[index]}");
                return;
            }
            
            object[] param = new object[method.GetParameters().Length];
         
            for (int i = 0; i < method.GetParameters().Length; i++) 
                param[i] = Convert.ChangeType(args[i], method.GetParameters()[i].ParameterType);
            
            object instance = this;

            if (method.DeclaringType != null && _instances.ContainsKey(method.DeclaringType))
                instance = _instances[method.DeclaringType];
            
            
            args.RemoveRange(0, method.GetParameters().Length);

            object arg = method.Invoke(instance, param);
            
            if(method.ReturnType != typeof(void)) args.Insert(0, arg);
            
            index--;
        }
        
        _buffer.Insert(0, command);

        if (args.Count != 0) photonView.RPC("SendMassage", RpcTarget.All, string.Join(" ", args)); 
        (_instances[typeof(PlayerMovmant)] as PlayerMovmant)?.sendMassage(string.Join(" ", args));
    }

    [PunRPC]
    private void SendMassage(string text)
    {
        textOutput.text +=  "name - <indent=10%>" + text + "</indent>\n";
    }

    [CommandAtribute("/debug", "write text to chat visible only for you")]
    public void _Debug(string text)
    {
        textOutput.text += "<color=#FFFF00>" + text + "</color>";
    }

    [CommandAtribute("-ray", "return gameObject you are looking at")]
    public GameObject _Ray()
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit)) return null;
        return hit.collider.gameObject;
    }
    
    [CommandAtribute("-name", "take a gameObject return name of it")]
    public string _getName(GameObject go)
    {
        return go.name;
    }

	[CommandAtribute("/help", "show help")]
    public void _help()
    {
        _Debug(string.Join("\n", Commands.Select(i => $"{i.Key} - {i.Value.GetCustomAttribute<CommandAtribute>().CommandDescription}")));
    }
}