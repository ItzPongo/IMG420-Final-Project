using Godot;
using System.Collections.Generic;

public partial class Blackboard : GodotObject
{
	private Dictionary<string, object> data = new Dictionary<string, object>();

	public void SetValue(string key, object value)
	{
		data[key] = value;
	}

	public T GetValue<T>(string key, T defaultValue = default)
	{
		if (data.TryGetValue(key, out object value))
		{
			return (T)value;
		}
		return defaultValue;
	}

	public bool HasValue(string key)
	{
		return data.ContainsKey(key);
	}

	public void Clear()
	{
		data.Clear();
	}
}
