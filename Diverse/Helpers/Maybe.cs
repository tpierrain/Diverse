using System;

namespace Diverse
{
    /// <summary>
    /// Maybe type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Maybe<T>
    {
        internal bool HasItem { get; }
        internal T Item { get; }

        public Maybe()
        {
            this.HasItem = false;
        }

        public Maybe(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            this.HasItem = true;
            this.Item = item;
        }

        public Maybe<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (this.HasItem)
                return new Maybe<TResult>(selector(this.Item));
            else
                return new Maybe<TResult>();
        }

        public T GetValueOrFallback(T fallbackValue)
        {
            if (fallbackValue == null)
                throw new ArgumentNullException(nameof(fallbackValue));

            if (this.HasItem)
                return this.Item;
            else
                return fallbackValue;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Maybe<T>;
            if (other == null)
                return false;

            return object.Equals(this.Item, other.Item);
        }

        public override int GetHashCode()
        {
            return this.HasItem ? this.Item.GetHashCode() : 0;
        }
    }
}