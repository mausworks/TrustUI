﻿using Moq;
using ScrawlCore.Stringification;
using System;
using System.Linq;
using Xunit;

using static Xunit.Assert;

namespace ScrawlCore.Test
{
    public class CompositeStringifierTests
    {
        [Fact]
        public void CreateWithNullStringifiers_ThrowsArgumentNullException()
            => Throws<ArgumentNullException>(() => new CompositeStringifier(null));

        [Fact]
        public void CreateWithEmptyStringifiers_ThrowsArgumentException()
            => Throws<ArgumentException>(() => new CompositeStringifier(Enumerable.Empty<Stringifier>()));

        [Fact]
        public void CreateWithNullKnownTypes_ThrowsArgumentNullException()
            => Throws<ArgumentNullException>(() => new CompositeStringifier(new[] { new NoopStringifier() }, null));

        [Fact]
        public void Stringifies_UsingChildStringifier()
        {
            var inputType = typeof(int);
            
            var childStringifierMock = new Mock<Stringifier>();

            childStringifierMock.Setup(m => m.CanStringify(inputType))
                .Returns(true)
                .Verifiable();

            childStringifierMock.Setup(m => m.Stringify(It.IsAny<int>()))
                .Returns<int>(val => val.ToString())
                .Verifiable();

            var compositeStringifier = new CompositeStringifier(childStringifierMock.Object);

            True(compositeStringifier.CanStringify(inputType));
            False(compositeStringifier.CanStringify(typeof(object)));

            Equal("1337", compositeStringifier.Stringify(1337));
            
            childStringifierMock.Verify();
        }
    }
}
