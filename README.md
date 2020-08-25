## stf-abstract-pool
This project contains C# templated pool that keeps track of used objects and have highly customisable inner mechanisms.

# Aim
Make the universal, garbage free and automatic pool

# Usage
Inherit from base pool and override required methods
```csharp
  // Simple pooling case
	public class SimplePool<T> : Pool<T> where T : new()
	{
		protected override T CreateNewItem() => new T();

		protected override void ActivateItem(T item) {}

		protected override void DeactivateItem(T item) {}

		protected override bool ItemIsFree(T item) => false;
	}
  
  // For Unity3D prefabs pooling, where pooled objects are hierarchically attached to the Transform component of target gameobject
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
