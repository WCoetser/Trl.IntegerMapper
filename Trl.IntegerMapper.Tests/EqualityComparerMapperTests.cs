﻿using System;
using Trl.IntegerMapper.EqualityComparerIntegerMapper;
using Trl.IntegerMapper.Tests.Fixtures;
using Xunit;

namespace Trl.IntegerMapper.Tests
{
    public class EqualityComparerMapperTests
    {
        private readonly EqualityComparerMapper<string> _mapper;

        public EqualityComparerMapperTests() 
            => _mapper = new EqualityComparerMapper<string>(StringComparer.InvariantCultureIgnoreCase);

        [Fact]
        public void DefaultContainerShouldContainEmptyCase()
        {
            // Arrange & Act
            var empty = _mapper.ReverseMap(0);

            // Assert
            Assert.Null(empty);
        }

        [Fact]
        public void ShouldMapValueToFirstAssignableInteger()
        {
            // Arrange & Act
            var first = _mapper.Map("testing 123");

            // Assert
            Assert.Equal(MapConstants.FirstMappableInteger, first);
        }

        [Fact]
        public void ShouldRepeatedlyMapSameValuesToSameInputs()
        {
            // Arrange
            var testCases = TestFixtures.GetTestDataForString();

            foreach (var testCase in testCases)
            {
                for (int i = 0; i < 3; i++)
                {
                    // Act
                    var value = _mapper.Map(testCase.Input);
                    var valueUpperCase = _mapper.Map(testCase.Input.ToUpper());

                    // Assert
                    Assert.Equal(testCase.ExpectedOutput, value);
                    Assert.Equal(testCase.ExpectedOutput, valueUpperCase);
                }
            }
            Assert.Equal(testCases.Length + 1, (int)_mapper.MappedObjectsCount);
        }

        [Fact]
        public void ShouldDoInverseMapForValues()
        {
            // Arrange
            var testCases = TestFixtures.GetTestDataForString();
            foreach (var testCase in testCases)
            {
                _mapper.Map(testCase.Input);
            }
            
            foreach (var testCase in testCases)
            {
                // Act
                var input = _mapper.ReverseMap(testCase.ExpectedOutput);

                // Assert
                Assert.Equal(testCase.Input, input);
            }
        }

        [Fact]
        public void ShouldClear()
        {
            // Arrange
            _mapper.Map("testing 123");

            // Act
            _mapper.Clear();

            // Assert
            Assert.Equal(1u, _mapper.MappedObjectsCount); // should only contain empty case
        }
    }
}