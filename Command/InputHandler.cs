using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;

public class InputHandler
{ 
    List<ICommand> commands;

    public InputHandler()
    {
        //SetSlideRx();
        commands = new List<ICommand>();
    }

    public void ExecuteCommand(ICommand command)
    {
        commands.Add(command);
        command.Execute();
    }

    public void UndoCommand()
    {
        if (commands.Count != 0)
        {
            ICommand lastCommand = commands[commands.Count - 1];
            lastCommand.Undo();
            commands.Remove(lastCommand);
        }
        else
        {
            Debug.LogWarning("empty command list");
        }

    }

    //make command


    /*public void SetCommand(ICommand varietyCommand)
    {
        command = varietyCommand;//언제든지 원하는 커맨드로 변경가능하다.
        //varietyCommand 는 ICommand(interface) 를 포함하는 클래스
    }*/

    /*public void InputSlide()
    {
        commands.Add(command);
        command.Execute();
    }*/







}
