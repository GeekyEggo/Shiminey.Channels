namespace Shiminey.Channels.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for <see cref="LinkedListNode{T}"/>
    /// </summary>
    internal static class LinkedListNodeExtensions
    {
        /// <summary>
        /// Determines whether this instance is the first node in the parent <see cref="LinkedList{T}"/>.
        /// </summary>
        /// <typeparam name="T">Specifies the element type of the linked list.</typeparam>
        /// <param name="node">This instance.</param>
        /// <returns><c>true</c> this instance is the first node in the parent <see cref="LinkedList{T}"/>; otherwise <c>false</c></returns>
        internal static bool IsFirst<T>(this LinkedListNode<T> node)
            => node == node.List.First;

        /// <summary>
        /// Determines whether this instance is the last node in the parent <see cref="LinkedList{T}"/>.
        /// </summary>
        /// <typeparam name="T">Specifies the element type of the linked list.</typeparam>
        /// <param name="node">This instance.</param>
        /// <returns><c>true</c> this instance is the last node in the parent <see cref="LinkedList{T}"/>; otherwise <c>false</c></returns>
        internal static bool IsLast<T>(this LinkedListNode<T> node)
            => node == node.List.Last;
    }
}
