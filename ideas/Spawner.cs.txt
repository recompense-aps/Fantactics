public class Spawner
{
	private Dictionary<string,PackedScene> scenes = new Dictionary<string,PackedScene>();

	public T Spawn<T>(string name) where T:Node
	{
		if(!scenes.ContainsKey(name))
		{
			throw Global.Error(string.Format("Could not spawn '{0}'. The scene could not be found.", name));
		}

		object instance = scenes[name].Instance();

		if(instance is T)
		{
			return instance as T;
		}

		throw Global.Error(string.Format("Could not spawn '{0}'. Could not convert {0} to type {1}", name, T.GetType()));
	}

	public void Load(string pathToScene, string name)
	{
		if(scenes.ContainsKey(name))
		{
			throw Global.Error(string.Format("Could not load '{0}'. It has been loaded already", name));
		}
		PackedScene scene = GD.Load<PackedScene>(pathToScene);
		scenes.Add(name, scene);
	}

	public void LoadGroup(string folderPath)
	{
		List<string> files = new List<string>();

		// get them some how...

		foreach(string file in files)
		{
			// process them
			string name,file;
			Load(file,name);
		}
	}
}