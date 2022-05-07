# ObjectDumper.NET
[![Version](https://img.shields.io/nuget/v/ObjectDumper.NET.svg)](https://www.nuget.org/packages/ObjectDumper.NET)  [![Downloads](https://img.shields.io/nuget/dt/ObjectDumper.NET.svg)](https://www.nuget.org/packages/ObjectDumper.NET)

ObjectDumper is a utility which aims to serialize C# objects to string for debugging and logging purposes.

### Download and Install ObjectDumper.NET
This library is available on NuGet: https://www.nuget.org/packages/ObjectDumper.NET/
Use the following command to install ObjectDumper using NuGet package manager console:

    PM> Install-Package ObjectDumper.NET

You can use this library in any .Net project which is compatible to PCL (e.g. Xamarin Android, iOS, Windows Phone, Windows Store, Universal Apps, etc.)

### The Purpose of ObjectDumper
Serialization, the process of converting a complex object to a machine-readable or over-the-wire transmittable string, is a technique often used in software engineering. A well-known serializer is Newtonsoft.JSON which serializes .NET objects to the data representation format JSON.

ObjectDumper.NET provides two excellent ways to visualize in-memory .NET objects:
- **DumpStyle.Console**: serialize objects to human-readable strings, often used to write complex C# objects to log files.
- **DumpStyle.CSharp**: serialize objects to C# initializer code, which can be used to compile a C# object again.

### API Usage
#### Dumping C# Objects to Console.WriteLine
The following sample program uses **DumpStyle.Console** to write C# objects to the console output:
```C#
static void Main(string[] args)
{
    var persons = new List<Person>
    {
        new Person { Name = "John", Age = 20, },
        new Person { Name = "Thomas", Age = 30, },
    };

    var personsDump = ObjectDumper.Dump(persons);

    Console.WriteLine(personsDump);
    Console.ReadLine();
}

//CONSOLE OUTPUT:
{ObjectDumperSample.Netfx.Person}
  Name: "John"
  Age: 20
{ObjectDumperSample.Netfx.Person}
  Name: "Thomas"
  Age: 30
```

#### Dumping C# initializer code from in-memory objects to Console.WriteLine
The following sample program uses **DumpStyle.CSharp** to write C# initializer code from in-memory to the console output:
```C#
static void Main(string[] args)
{
    var persons = new List<Person>
    {
        new Person { Name = "John", Age = 20, },
        new Person { Name = "Thomas", Age = 30, },
    };

    var personsDump = ObjectDumper.Dump(persons, DumpStyle.CSharp);

    Console.WriteLine(personsDump);
    Console.ReadLine();
}

//CONSOLE OUTPUT:
var listOfPersons = new List<Person>
{
  new Person
  {
    Name = "John",
    Age = 20
  },
  new Person
  {
    Name = "Thomas",
    Age = 30
  }
};
```

### Links
C# Escape / Unescape
https://codebeautify.org/csharp-escape-unescape

### License
This project is Copyright &copy; 2022 [Thomas Galliker](https://ch.linkedin.com/in/thomasgalliker). Free for non-commercial use. For commercial use please contact the author.
