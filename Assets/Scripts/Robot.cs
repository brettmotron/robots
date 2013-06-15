using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Robot : MonoBehaviour {
	
	public enum Command {
		None = 0,
		Forward1,
		Forward2,
		Forward3,
		RotateLeft,
		RotateRight,
		UTurn,
		Back1,
		MAX
	}
	
	public Tile currentTile;
	public Facing facing;
	public bool isDead;
	
	List<Command> commandList;
	int commandListLimit = 5;
	int currentCommandIndex;
	bool readyToProcessCommand = true;
	bool commandsFinished;
	bool hasCompletedStep;
	
	delegate IEnumerator CommandMethod();
	
	CommandMethod[] Commands;
	CommandMethod Forward1;
	CommandMethod Forward2;
	CommandMethod Forward3;
	CommandMethod RotateLeft;
	CommandMethod RotateRight;
	CommandMethod UTurn;
	CommandMethod Backward1;
	
	void Awake() {		
		if (null == Commands) {
			Forward1 = new CommandMethod(ForwardOneCommand);
			Forward2 = new CommandMethod(ForwardTwoCommand);
			Forward3 = new CommandMethod(ForwardThreeCommand);
			RotateLeft = new CommandMethod(RotateLeftCommand);
			RotateRight = new CommandMethod(RotateRightCommand);
			UTurn = new CommandMethod(UTurnCommand);
			Backward1 = new CommandMethod(BackwardOneCommand);
			Commands = new CommandMethod[] {() => {return null;}, Forward1, Forward2, Forward3, RotateLeft, RotateRight, UTurn, Backward1};
		}
	}
	
	void Start() {
		commandList = new List<Command>();
		currentTile = BoardMaster.SharedInstance.GetStartingTile();
	}
		
	
	void Update() {
		if (commandList.Count == commandListLimit) {
			if (readyToProcessCommand) {
				StartCoroutine(ProcessNextCommand());
			}
		}	
		
		if (commandsFinished) {
			ClearCommandList();
		}
	}
	
	IEnumerator ProcessNextCommand() {
		if (isDead) {
            yield break;	
		}
		
		
		if (currentCommandIndex < commandListLimit) {
			readyToProcessCommand = false;
			yield return StartCoroutine(Commands[(int)commandList[currentCommandIndex]]());
			yield return StartCoroutine(ProcessBoardEffects());
			readyToProcessCommand = true;			
		} else {
			commandsFinished = true;	
		}
		
		++currentCommandIndex;	
	}
	
	IEnumerator ProcessBoardEffects() {
		if (isDead) {
			yield break;
		}
		
		yield return StartCoroutine(BoardMaster.SharedInstance.ProcessBoardEffects(this));
	}
	
	public void QueueCommand(Command command) {
		if (commandList.Count < commandListLimit) {
			commandList.Add(command);
			Debug.Log(command + " added to command list");
		} else {
			Debug.Log("Command list full!");	
		}
	}
	
	void ClearCommandList() {
		commandList.Clear();
		currentCommandIndex = 0;				
		commandsFinished = false;
	}
	
	public IEnumerator ForwardOneCommand() {
		Debug.Log("Moving Forward One.");
		yield return StartCoroutine(MoveForwardOne(1));
	}
	
	public IEnumerator ForwardTwoCommand() {
		Debug.Log("Moving Forward Two.");
		yield return StartCoroutine(MoveForwardOne(2));
		yield return StartCoroutine(MoveForwardOne(2));
	}

	public IEnumerator ForwardThreeCommand() {
		Debug.Log("Moving Forward Three.");
		yield return StartCoroutine(MoveForwardOne(3));
		yield return StartCoroutine(MoveForwardOne(3));
		yield return StartCoroutine(MoveForwardOne(3));
	}
	
	public IEnumerator RotateLeftCommand() {
		Debug.Log("Rotating Left.");
		yield return StartCoroutine(DoRotateLeft(1));
	}
		
	public IEnumerator RotateRightCommand() {
		Debug.Log("Rotating Right.");
		yield return StartCoroutine(DoRotateRight(1));
	}
	
	public IEnumerator UTurnCommand() {
		Debug.Log("U-Turning.");		
		if (Random.Range(0,2) == 0) {
			yield return StartCoroutine(DoRotateLeft(2));
			yield return StartCoroutine(DoRotateLeft(2));
		} else {
			yield return StartCoroutine(DoRotateRight(2));
			yield return StartCoroutine(DoRotateRight(2));
		}
	}
	
	public IEnumerator BackwardOneCommand() {
		Debug.Log("Moving Backward One.");
		yield return StartCoroutine(MoveBackwardOne(1));	
	}
	
	public IEnumerator MoveForwardOne(float speed) {
		if (isDead) {
			yield break;
		}
		
		Vector3 pos;
		Tile newTile = BoardMaster.SharedInstance.GetForwardTile(currentTile, facing, out pos);
		
		while (Vector3.SqrMagnitude(pos - transform.position) > 0.05*0.05) {
			transform.Translate(Vector3.forward * Time.deltaTime * speed);
			yield return null;	
		}
		
		currentTile = newTile;
		transform.position = pos;
		
		if (null == newTile) {
			Debug.Log("I'm dead!");
			isDead = true;
		}		
	}
	
	public IEnumerator MoveBackwardOne(float speed) {
		if (isDead) {
			yield break;
		}
		
		Vector3 pos;
		Tile newTile = BoardMaster.SharedInstance.GetBackwardTile(currentTile, facing, out pos);
		
		while (Vector3.SqrMagnitude(pos - transform.position) > 0.05*0.05) {
			transform.Translate(Vector3.back * Time.deltaTime * speed);
			yield return null;	
		}
		
		currentTile = newTile;
		transform.position = pos;
		
		if (null == newTile) {
			Debug.Log("I'm dead!");
			isDead = true;
		}		
	}
	
	public IEnumerator DoRotateLeft(float speed) {
		if (isDead) {
			yield break;
		}
		
		Facing newFacing = Utils.RotateLeftFacing(facing);
		float targetAngle = 90 * (int)newFacing;
		while (Mathf.Abs(targetAngle - transform.localEulerAngles.y) > 2) {
			transform.Rotate(0, -Time.deltaTime * 45 * speed, 0);
			yield return null;
		}
		facing = newFacing;
		transform.localEulerAngles = targetAngle * Vector3.up;	
	}
	
	public IEnumerator DoRotateRight(float speed) {
		if (isDead) {
			yield break;
		}
		
		Facing newFacing = Utils.RotateRightFacing(facing);
		float targetAngle = 90 * (int)newFacing;
		while (Mathf.Abs(targetAngle - transform.localEulerAngles.y) > 2) {
			transform.Rotate(0, Time.deltaTime * 45 * speed, 0);
			yield return null;
		}
		facing = newFacing;
		transform.localEulerAngles = targetAngle * Vector3.up;	
	}
}
