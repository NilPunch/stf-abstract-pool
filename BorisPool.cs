using System.Collections.Generic;
using UnityEngine;

namespace Boris
{
	public class LinkedItem<T>
	{
		public LinkedListNode<LinkedItem<T>> Link = null;
		public T Item;
	}

	public abstract class Pool<T>
	{
		private readonly Queue<T> _pool = new Queue<T>(); // Items pool
		private readonly LinkedList<LinkedItem<T>> _used = new LinkedList<LinkedItem<T>>(); // Shared and used items
		private readonly Queue<LinkedListNode<LinkedItem<T>>> _nodePool = new Queue<LinkedListNode<LinkedItem<T>>>(); // Pool of shared nodes

		protected Queue<T> Pooled => _pool;
		protected LinkedList<LinkedItem<T>> Used => _used;

		public bool AutomaticRefresh = true;

		public int Count => _pool.Count + _used.Count;
		public int Available => _pool.Count;
		public int Using => _used.Count;


		// Use it into constructor for child class if needed
		protected void Prewarm(uint prewarm = 0)
		{
			while (prewarm > 0)
			{
				T item = CreateNewItem();
				DeactivateItem(item);
				_pool.Enqueue(item);
				--prewarm;
			}
		}

		public LinkedItem<T> Get()
		{
			T item;
			LinkedItem<T> linkedItem;

			// Update info about items usage
			if (RefreshCondition() && AutomaticRefresh)
			{
				RefreshUnused();
			}

			// Get item from pool or creating new
			if (_pool.Count == 0)
			{
				item = CreateNewItem();
			}
			else
			{
				item = _pool.Dequeue();
			}

			// Create and add item to Used or get LinkedListNode from node pool
			if (_nodePool.Count == 0)
			{
				linkedItem = new LinkedItem<T>();   // linkedItem initialisation
				_used.AddLast(linkedItem);  // This create new LinkedListNode with Value == linkedItem
			}
			else
			{
				var linkListNodeLinkedItem = _nodePool.Dequeue();
				linkedItem = linkListNodeLinkedItem.Value;  // linkedItem initialisation
				_used.AddLast(linkListNodeLinkedItem);  // This doesn't create anything
			}

			// Preparaction before using
			ActivateItem(item);

			// Packing object
			linkedItem.Link = _used.Last;
			linkedItem.Item = item;

			return linkedItem;
		}

		public void RefreshUnused()
		{
			LinkedListNode<LinkedItem<T>> node = _used.First;
			while (node != null)
			{
				LinkedListNode<LinkedItem<T>> current = node;
				node = node.Next;

				// Check if item is unused
				if (ItemIsFree(current.Value.Item))
				{
					DeactivateItem(current.Value.Item); // Preparacion before pooling
					_pool.Enqueue(current.Value.Item); // Pooling
					_used.Remove(current); // Remove node from LinkedList
					_nodePool.Enqueue(current); // Pool this node
					current.Value.Link = null;  // For safety
				}
			}
		}

		public void Return(LinkedItem<T> linkedItem)
		{
			DeactivateItem(linkedItem.Item); // Preparacion before pooling
			_pool.Enqueue(linkedItem.Item); // Pooling
			if (linkedItem.Link != null)
			{
				_used.Remove(linkedItem.Link); // Remove node from LinkedList
				_nodePool.Enqueue(linkedItem.Link); // Pool this node
			}
		}

		#region Must override this
		// Custom item creation
		protected abstract T CreateNewItem();
		// Predicate that check if item is not in use
		protected abstract bool ItemIsFree(T item);
		// Custom item activation
		protected abstract void ActivateItem(T item);
		// Custom item deactivation
		protected abstract void DeactivateItem(T item);
		#endregion

		#region Otional override
		// Automatic update case on every Get()
		protected virtual bool RefreshCondition() => false;
		#endregion
	}

	public class SimplePool<T> : Pool<T> where T : new()
	{
		protected override T CreateNewItem() => new T();

		protected override void ActivateItem(T item) {}

		protected override void DeactivateItem(T item) {}

		protected override bool ItemIsFree(T item) => false;
	}
}

