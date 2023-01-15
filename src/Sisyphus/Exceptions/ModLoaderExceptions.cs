using System;

namespace Sisyphus.Exceptions;

public class DuplicateModsException : Exception {
    public DuplicateModsException() { }

    public DuplicateModsException(string message) : base(message) { }

    public DuplicateModsException(string message, Exception inner)
        : base(message, inner) { }
}
