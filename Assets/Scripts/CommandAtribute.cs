using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAtribute : Attribute
{
    public readonly string CommandName;
    public readonly string CommandDescription;

    public CommandAtribute(string commandName, string commandDescription)
    {
        CommandName = commandName;
        CommandDescription = commandDescription;
    }
}