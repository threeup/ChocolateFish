using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour 
{
	public static InputManager instance;
	public static InputManager Instance { get { return instance; } }


	public delegate void OnInputKeyDelegate(KeyCode key);
	public delegate void OnInputAxisDelegate(float deltaX, float deltaY);
	public OnInputKeyDelegate HandleKey;
	public OnInputAxisDelegate HandleAxis;

	public GUIText debugInputText;
	protected Vector2 mouseVec = new Vector2();
	Vector2[] touchListAnchor;
	//TauPawn lastPawn;


 	public InputManager()
	{
		instance = this;

	}
	

	public void Setup() 
	{
		//lastPawn = null;

		touchListAnchor = new Vector2[4];
		HandleKey = DefaultHandleKey;
		HandleAxis = DefaultHandleAxis;
	}
	
	
	void Update() 
	{
		UpdateKeys();
		//UpdateSwipe();
		UpdateHotSpot();
		UpdateMouseWorld();
		//MouseDrag();
	}

	public void UpdateMouseWorld()
	{

		Camera cam = Camera.main;
		Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane);
		Vector3 worldPoint = cam.ScreenToWorldPoint(screenPoint);
		mouseVec.x = worldPoint.x;
		mouseVec.y = worldPoint.y;
	}

	public void UpdateKeys()
	{
		float deltaX = 0f;
		float deltaY = 0f;
		if (Input.GetKey(KeyCode.LeftArrow))
		{ 
			deltaX = -1f;
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			deltaX = 1f;
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			deltaY = -1f;
		}
		else if (Input.GetKey(KeyCode.UpArrow))
		{
			deltaY = 1f;
		} 
		HandleAxis(deltaX,deltaY);

		if (Input.GetKey(KeyCode.Space))
		{
			HandleKey(KeyCode.Space);
		} 
	}

	public void UpdateHotSpot()
	{
#if UNITY_ANDROID
		int i = 0;
		while (i < Input.touchCount && i < 4) 
		{
			Touch touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
			{
				Vector2 midpoint = new Vector2(Screen.width*0.5f, Screen.height*0.15f);
				Vector2 diff = touch.position - midpoint;
				diff.x = diff.x/Screen.width;
				diff.y = diff.y/Screen.height;
				diff.x *= (Mathf.Abs(diff.x) < 0.1f) ? 0f : 4f;
				diff.y *= (Mathf.Abs(diff.y) < 0.05f) ? 0f : 5f;
				
				debugInputText.text = diff.x +" , "+diff.y;
				HandleAxis(Mathf.Clamp(diff.x, -1f, 1f), Mathf.Clamp(diff.y, -1f, 1f));
			}
			 ++i;
		}
#endif		
	}

	public void UpdateSwipe()
	{
#if UNITY_ANDROID
		int i = 0;
		while (i < Input.touchCount && i < 4) 
		{
			Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
            	touchListAnchor[i] = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
            	bool isDpad = touch.position.y < Screen.height*0.3f;
            	
            	if (isDpad)
            	{
            		Vector2 diff = touch.position - touchListAnchor[i] ;
	            	HandleAxis(Mathf.Clamp(diff.x/30f, -2f, 2f),0);
	            }
	            else
	            {
	            	
	            	Vector2 diff = touch.position - touchListAnchor[i] ;
	            	HandleAxis(0, Mathf.Clamp(diff.y/30f, -2f, 2f));
	            }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
            	touchListAnchor[i] = Vector2.zero;
            }
            ++i;
        }
#endif  
	}

	public void AddInput(OnInputAxisDelegate del)
	{
		HandleAxis += del;
	}
	public void AddInput(OnInputKeyDelegate del)
	{
		HandleKey += del;
	}

	public void RemoveInput(OnInputAxisDelegate del)
	{
		HandleAxis -= del;
	}
	public void RemoveInput(OnInputKeyDelegate del)
	{
		HandleKey -= del;
	}

	void DefaultHandleKey(KeyCode key)
	{
		//Debug.Log("key "+key);
	}

	void DefaultHandleAxis(float x, float y)
	{
		if (Mathf.Abs(x) > 0.1 || Mathf.Abs(y) > 0.1)
		{
			//Debug.Log(x+" "+y);
		}
	}

	/*
	public void MouseDrag()
	{
		BasicObject bobj = Input.GetMouseButtonDown(0) ? GetBobjAtMouse() : null;
		if (lastBobj != bobj)
		{
			if (lastBobj != null && lastBobj.HandleRelease != null)
			{
				lastBobj.HandleRelease();
			}
			lastBobj = bobj;
		}
		if(bobj != null && bobj.HandleSelect != null)
		{
			bobj.HandleSelect();
		}

	}


	public virtual BasicObject GetBobjAtMouse()
	{
		Vector3 rayDir = GreenVars.isCameraFlat ? Vector3.up : Vector3.forward;

		Vector3 mouseVecThree = Utilities.ToXZVector(mouseVec);
		Ray ray = new Ray(mouseVecThree + 5*rayDir, -rayDir);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 10))
		{
			return hit.collider.gameObject.GetComponent<BasicObject>();
		}
		else
		{

		}
		return null;
	}
	*/
}
