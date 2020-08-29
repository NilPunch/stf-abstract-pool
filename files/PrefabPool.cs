public class PrefabPool : Pool<UnityEngine.GameObject>
{
	private GameObject _prefab;
	private GameObject _target;
  
	public PrefabPool(GameObject prefab, GameObject target, uint prewarm = 0)
	{
		this._prefab = prefab;
		this._target = target;
		Prewarm(prewarm);
	}
  
  	// Called right before getting an object from the pool, so we just activate it
	protected override void ActivateItem(GameObject item)
	{
		item.SetActive(true);
	}
  
  	// On item creation we just instantiate new GameObject from _prefab and attach it to the parent
	protected override GameObject CreateNewItem()
	{
		GameObject newGameObject = GameObject.Instantiate(_prefab);
		newGameObject.transform.SetParent(_target.transform);
		return newGameObject;
	}
  
  	// Called right before the object is returned to the pool
	protected override void DeactivateItem(GameObject item)
	{
		item.SetActive(false);
	}
  
	protected override bool ItemIsFree(GameObject item) => item.activeInHierarchy == false;
  
  	// This means that we will check the activity of objects every call to the Get() function.
	protected override bool RefreshCondition() => true;
}
