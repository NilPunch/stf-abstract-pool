public class SimplePool<T> : Pool<T> where T : new()
{
	// Called when the pool runs out of objects and needs to create a new one
	protected override T CreateNewItem() => new T();

	// Called right before getting an object from the pool
	protected override void ActivateItem(T item) {}
	
	// Called right before the object is returned to the pool
	protected override void DeactivateItem(T item) {}
	
	// Since we have not overridden the RefreshCondition method, there is no need to check objects for their freeness
	protected override bool ItemIsFree(T item) => false;
}
