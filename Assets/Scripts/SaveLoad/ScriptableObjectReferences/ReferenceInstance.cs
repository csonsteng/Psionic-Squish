using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public abstract class ReferenceInstance<T> : ISerializationCallbackReceiver
	where T : ReferenceData
{
	public T Data => data;
	protected T data;
	[SerializeField] protected string referenceID;

	public ReferenceInstance(T baseObject)
	{
		data = baseObject;
	}

	public void OnAfterDeserialize()
	{
		if(referenceID.IsNotNullOrEmpty())
			data = LoadReference();
	}

	public void OnBeforeSerialize() 
	{
		if(data != null)
			referenceID = data.GetUniqueID();
	}

	protected abstract T LoadReference();
}
