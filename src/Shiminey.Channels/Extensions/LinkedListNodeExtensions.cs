namespace Shiminey.Channels.Extensions
{
    using System.Collections.Generic;

    internal static class LinkedListNodeExtensions
    {
        internal static bool IsFirst<T>(this LinkedListNode<T> node)
            => node == node.List.First;

        internal static bool IsLast<T>(this LinkedListNode<T> node)
            => node == node.List.Last;
    }
}
