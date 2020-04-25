using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Diagnostics;

public class HelperFunctions : MonoBehaviour {

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string GetCurrentMethod()
    {
        StackTrace st = new StackTrace();
        StackFrame sf = st.GetFrame(1);

        return sf.GetMethod().Name;
    }

	public static GameObject GetChildGameObject(GameObject fromGameObject, string withName)
	{
		Transform[] ts = fromGameObject.GetComponentsInChildren<Transform>();
		foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
		return null;
	}
}
