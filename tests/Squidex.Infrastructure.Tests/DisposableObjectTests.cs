﻿// ==========================================================================
//  DisposableObjectTest.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using Xunit;

namespace Squidex.Infrastructure
{
    public class DisposableObjectTests
    {
        public sealed class MyDisposableObject : DisposableObject
        {
            public int DisposeCallCount { get; set; }

            protected override void DisposeObject(bool disposing)
            {
                DisposeCallCount++;
            }

            public void Ensure()
            {
                ThrowIfDisposed();
            }
        }

        [Fact]
        public void Should_not_throw_exception_when_not_disposed()
        {
            var sut = new MyDisposableObject();

            sut.Ensure();
        }

        [Fact]
        public void Should_dispose_once()
        {
            var sut = new MyDisposableObject();

            sut.Dispose();
            sut.Dispose();

            Assert.True(sut.IsDisposed);

            Assert.Equal(1, sut.DisposeCallCount);
        }

        [Fact]
        public void Should_throw_exception_when_disposed()
        {
            var sut = new MyDisposableObject();

            sut.Dispose();

            Assert.Throws<ObjectDisposedException>(() => sut.Ensure());
        }
    }
}
