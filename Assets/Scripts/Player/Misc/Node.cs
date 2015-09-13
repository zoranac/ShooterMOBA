using UnityEngine;
using System.Collections;

public class Node {
	public enum NodeTypes
	{
		Move,
		Attack
	};
	public NodeTypes NodeType;

	public Vector3 Target;
	public float Duration = 0;
	public Node(NodeTypes nodeType, Vector3 target)
	{
		NodeType = nodeType;
		Target = target;
	}
	public Node(NodeTypes nodeType, Vector3 target, float duration)
	{
		NodeType = nodeType;
		Target = target;
		Duration = duration;
	}
}
