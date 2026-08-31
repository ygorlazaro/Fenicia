using System;
using System.Reflection;
class P {
    static void Main() {
        foreach(var asm in AppDomain.CurrentDomain.GetAssemblies()) {
            var t = asm.GetType("System.CompositeFormat");
            if(t != null) Console.WriteLine(t.FullName);
            t = asm.GetType("System.CompositeFormat.CompositeFormat");
            if(t != null) Console.WriteLine(t.FullName);
        }
    }
}
