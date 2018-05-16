using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Nameless.Libraries.Aura.Utils.DiffMatchPatch {

    /// <summary>
    /// The data structure representing a diff is a List of Diff objects:
    /// {Diff(Operation.DELETE, "Hello"), Diff(Operation.INSERT, "Goodbye"),
    /// Diff(Operation.EQUAL, " world.")}
    /// which means: delete "Hello", add "Goodbye" and keep " world."
    /// </summary>
    public enum Operation {
        DELETE,
        INSERT,
        EQUAL
    }
}