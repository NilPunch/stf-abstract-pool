## stf-abstract-pool
This project contains C# templated pool that keeps track of used objects and have highly customisable inner mechanisms.

# Aim
Make the universal, garbage free and automatic pool

# Usage
Inherit from base pool and override required methods. For more information see the source for the `abstract class Pool<T>`.
```csharp
// Simple pooling case, where we just allocate memory for objects and then manually return them to the pool
public class SimplePool<T> : Pool<T> where T : new()
{
	protected override T CreateNewItem() => new T();

	protected override void ActivateItem(T item) {}

	protected override void DeactivateItem(T item) {}

	protected override bool ItemIsFree(T item) => false;
}
  
// Unity3D prefabs pool, where pooled objects are hierarchically attached to the transform component of target gameobject.
// This pool also tracks the activity of objects. If any object has become inactive, then it will be returned to the pool.
public class PrefabPool : Boris.Pool<UnityEngine.GameObject>
{
	private GameObject _prefab;
	private GameObject _target;

	public PrefabPool(GameObject prefab, GameObject target, uint prewarm = 0)
	{
		this._prefab = prefab;
		this._target = target;
		Prewarm(prewarm);
	}

	protected override void ActivateItem(GameObject item)
	{
		item.SetActive(true);
	}

	protected override GameObject CreateNewItem()
	{
		GameObject newGameObject = GameObject.Instantiate(_prefab);
		newGameObject.transform.SetParent(_target.transform);
		return newGameObject;
	}

	protected override void DeactivateItem(GameObject item)
	{
		item.SetActive(false);
	}

	protected override bool ItemIsFree(GameObject item) => item.activeInHierarchy == false;
	protected override bool RefreshCondition() => true;
}
```

# Installation
Just copy the content of the `files` folder into your project.

# License
[MIT](https://choosealicense.com/licenses/mit/)
