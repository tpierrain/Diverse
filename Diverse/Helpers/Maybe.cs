using System;

namespace Diverse
{
    /// <summary>
    /// Maybe monad. A <see cref="Maybe{T}"/> is a discriminated union type with two possible value constructors.
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    public sealed class Maybe<T>
    {
        internal bool HasItem { get; }
        internal T Item { get; }

        /// <summary>
        /// Instantiates a <see cref="Maybe{T}"/> that has no Item.
        /// </summary>
        public Maybe()
        {
            this.HasItem = false;
        }

        /// <summary>
        /// Instantiates a <see cref="Maybe{T}"/> that has an item.
        /// </summary>
        /// <param name="item">The item of the <see cref="Maybe{T}"/> instance.</param>
        public Maybe(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            this.HasItem = true;
            this.Item = item;
        }

        /// <summary>
        /// Select
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public Maybe<TResult> Select<TResult>(Func<T, TResult> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (this.HasItem)
                return new Maybe<TResult>(selector(this.Item));
            else
                return new Maybe<TResult>();
        }

        /// <summary>
        /// Gets the value or fallback with provided value otherwise.
        /// </summary>
        /// <param name="fallbackValue">The fallback value to return if no item is set.</param>
        /// <returns>The Value</returns>
        public T GetValueOrFallback(T fallbackValue)
        {
            if (fallbackValue == null)
                throw new ArgumentNullException(nameof(fallbackValue));

            if (this.HasItem)
                return this.Item;
            else
                return fallbackValue;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Maybe{T}"/> objects are considered equal.
        /// </summary>
        /// <param name="obj">The second <see cref="Maybe{T}"/> object to compare.</param>
        /// <returns><b>true</b> if the two <see cref="Maybe{T}"/> are equal, <b>false</b> otherwise.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as Maybe<T>;
            if (other == null)
                return false;

            return object.Equals(this.Item, other.Item);
        }

        /// <summary>
        /// Hash function.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Maybe{T}"/>.</returns>
        public override int GetHashCode()
        {
            return this.HasItem ? this.Item.GetHashCode() : 0;
        }
    }
}