using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialCommandManager : MonoBehaviour
{
    private Dictionary<string, MethodInfo> Commands = new Dictionary<string, MethodInfo>();
    [SerializeField] private TMP_InputField textInput;
	[SerializeField] private TMP_Text textOutput;
	[SerializeField] private RectTransform textOutputBG;
    [SerializeField] private GameObject chatPanel;
    
    public static TutorialCommandManager Instance { get; private set; }
    
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
                //Debug.Log(attribute.CommandName + " added");
            }
        }

        Debug.Log("onSubmit: " + textInput);
        textInput.onSubmit.AddListener(OnSubmit);
    }

    public void ShowChatPanel()
    {
        chatPanel.SetActive(true);
        textOutput.text = "<alpha=#FF>" + textOutput.text.Substring(11);
        EventSystem.current.SetSelectedGameObject(textInput.gameObject, null);
        _selectedIndex = -1;
    }

    public void HideChatPanel()
    {
        textInput.text = "";
        EventSystem.current.SetSelectedGameObject(null);
        textOutput.text = "<alpha=#00>" + textOutput.text.Substring(11);
        chatPanel.SetActive(false);
    }
    
    private void Update()
    {
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
    }

    private void OnSubmit(string text)
    {
        Debug.Log("OnSubmit" + text);
        ProcessCommand(textInput.text);
        TutorialUIController.instance.Dispose();
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

        args.Reverse();
        if (args.Count != 0) SendMassage(string.Join(" ", args), PlayerPrefs.GetString("name")); 
        //(_instances[typeof(PlayerMovmant)] as PlayerMovmant)?.sendMassage(string.Join(" ", args));
    }

    public void FastMassage(string text)
    {
        ProcessCommand(text);
    }

    public void SendMassage(string text, string name)
    {
        int index = textOutput.text.Length-1;
        
        StartCoroutine(MessageControl(text, name, index, 10));
    }

    Action<int, int> ChangeLenght;

    IEnumerator MessageControl(string text, string name, int index, float time)
    {
        string fullMessage;
        ChangeLenght += (i, count) =>
        {
            if (i < index)
            {
                index-=count;
            }
        };
        float timer = 0;
        fullMessage = ($"<alpha=#{255.ToString("X2")}><mark=#00000099>{name}: {text}</mark>\n");
        textOutput.text += fullMessage;
        int lastLenght = fullMessage.Length;
        
        while (timer < time)
        {
            fullMessage=($"<alpha=#{((int)(Mathf.Clamp01(chatPanel.activeSelf ? 255 : (time - timer)*0.5f) * 255)).ToString("X2")}><mark=#00000099>{name}: {text}</mark>\n");
            textOutput.text = textOutput.text.Remove(index, lastLenght).Insert(index, fullMessage);
            
            
            lastLenght = fullMessage.Length;
            timer += Time.deltaTime;
            yield return null;
        }

        fullMessage = ($"<mark=#00000099>{name}: {text}</mark>\n");
        textOutput.text = textOutput.text.Remove(index, lastLenght).Insert(index, fullMessage);
        ChangeLenght.Invoke(index, lastLenght-fullMessage.Length);
    }
}