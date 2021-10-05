using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionNode : NodeDTree
{
    private NodeDTree _trueNode;
    private NodeDTree _falseNode;
    public delegate bool Question();
    private Question _question;


    public QuestionNode(NodeDTree trueNode, NodeDTree falseNode, Question question)
    {
        _trueNode = trueNode;
        _falseNode = falseNode;
        _question = question;

    }

    public override void Execute()
    {
        if (_question())
        {
            _trueNode.Execute();
        }
        else
        {
            _falseNode.Execute();
        }
    }
}
