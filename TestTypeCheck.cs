using System;
using CommandLine;

class TestTypeCheck
{
    static void Main()
    {
        // Test the IsAssignableFrom logic
        Type errorType = typeof(HelpRequestedError);
        
        Console.WriteLine($"HelpRequestedError IsSealed: {errorType.IsSealed}");
        Console.WriteLine($"HelpRequestedError BaseType: {errorType.BaseType?.Name}");
        
        // Simulate what the code does
        Type t = typeof(HelpRequestedError);
        
        bool currentLogic = t.IsAssignableFrom(typeof(HelpRequestedError));
        bool correctLogic = typeof(HelpRequestedError).IsAssignableFrom(t);
        bool equalityCheck = t == typeof(HelpRequestedError);
        
        Console.WriteLine($"\nCurrent logic t.IsAssignableFrom(typeof(HelpRequestedError)): {currentLogic}");
        Console.WriteLine($"Correct logic typeof(HelpRequestedError).IsAssignableFrom(t): {correctLogic}");
        Console.WriteLine($"Equality check t == typeof(HelpRequestedError): {equalityCheck}");
        
        // Test with base type
        if (errorType.BaseType != null)
        {
            Type baseType = errorType.BaseType;
            Console.WriteLine($"\nTesting with base type: {baseType.Name}");
            Console.WriteLine($"baseType.IsAssignableFrom(typeof(HelpRequestedError)): {baseType.IsAssignableFrom(typeof(HelpRequestedError))}");
            Console.WriteLine($"typeof(HelpRequestedError).IsAssignableFrom(baseType): {typeof(HelpRequestedError).IsAssignableFrom(baseType)}");
        }
    }
}
