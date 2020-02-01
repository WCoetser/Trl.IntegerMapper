﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace IntegerMapper.Core.ByteEnumerableIntegerMapper
{
    /// <summary>
    /// Maps equal <see cref="IEnumerable{byte}"/> to equal integers.
    /// </summary>
    public class ByteEnumerableMapper : IIntegerMapper<IEnumerable<byte>>
    {
        /// <summary>
        /// The collection of known mapped byte arrays
        /// </summary>
        private readonly ByteEnumerableMapperNode _rootNode;

        /// <summary>
        /// Next integer to assign to an input value.
        /// </summary>
        private uint _nextAssignableInteger;

        /// <summary>
        /// Keeps track of which integers have been mapped to which values.
        /// </summary>
        private readonly List<ByteEnumerableMapperNode> _inverseMap;

        public ByteEnumerableMapper()
        {
            _nextAssignableInteger = MapConstants.FirstMappableInteger;
            _rootNode = new ByteEnumerableMapperNode(null, null);
            _inverseMap = new List<ByteEnumerableMapperNode>
            {
                _rootNode
            };
        }

        public uint Map(IEnumerable<byte>? byteIterator)
        {
            var byteInputEnumerator = byteIterator?.GetEnumerator();
            if (byteIterator == null || !byteInputEnumerator!.MoveNext())
            {
                return MapConstants.NullOrEmpty;
            }

            var currentNode = _rootNode;
            do
            {
                if (currentNode.NextNodes[byteInputEnumerator.Current] == null)
                {
                    currentNode.NextNodes[byteInputEnumerator.Current] = new ByteEnumerableMapperNode(byteInputEnumerator.Current, currentNode);
                }
                currentNode = currentNode.NextNodes[byteInputEnumerator.Current];
            } while (byteInputEnumerator.MoveNext());

            if (!currentNode.MappedValue.HasValue)
            {
                currentNode.MappedValue = _nextAssignableInteger;
                _inverseMap.Add(currentNode);
                _nextAssignableInteger++;
            }

            return currentNode.MappedValue.Value;
        }

        public IEnumerable<byte> ReverseMap(uint mappedValue)
        {
            if (mappedValue >= _nextAssignableInteger)
            {
                throw new Exception($"Value has not been mapped: {mappedValue}");
            }
            if (mappedValue == MapConstants.NullOrEmpty)
            {
                return Array.Empty<byte>();
            }
            var reverseNode = _inverseMap[(int)mappedValue];
            return reverseNode.GetRepresentedValue().Reverse();
        }
    }
}