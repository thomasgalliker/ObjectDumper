# ObjectDumper.NET
[![Version](https://img.shields.io/nuget/v/ObjectDumper.NET.svg)](https://www.nuget.org/packages/ObjectDumper.NET)  [![Downloads](https://img.shields.io/nuget/dt/ObjectDumper.NET.svg)](https://www.nuget.org/packages/ObjectDumper.NET)

ObjectDumper is a utility which aims to serialize C# objects to string for debugging and logging purposes.

### Download and Install ObjectDumper.NET
This library is available on NuGet: https://www.nuget.org/packages/ObjectDumper.NET/
Use the following command to install ObjectDumper using NuGet package manager console:

    PM> Install-Package ObjectDumper.NET

You can use this library in any .NET project which is compatible to PCL (e.g. Xamarin Android, iOS, Windows Phone, Windows Store, Universal Apps, etc.)

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

### Strong-named assembly
This assembly is signed with the key ObjectDumper.snk in this repository.

Public key (hash algorithm: sha1):
```
00240000048000009400000006020000002400005253413100040000010001008da06ec8c6bd242c52102a9fc293b7af32f183da0d069f7c9522f063cacc3cc584668dfd6cf0560577380822b0c46fdb19e44fc78fad5e8d15b2c24a8766e2769c942705442926b3dcce385eac263893a4b6916976324544792ba1fb4697ab0d1bf28f3c8f0512234fa0a7b732141f7dc4b4a340bdaa95a6c1460c6a699e65c3
```

Public key token is `fcc359471136d8b8`.

In order to get these values, run following commands:
- Extract public key of snk: `sn -p ObjectDumper.snk public.key`
- Display public key: `sn -tp public.key`


### Links
- Standard numeric format strings
https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
- Standard date and time format strings
https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
- Standard timespan format strings
https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings
- C# Record Types
https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record
- C# Escape / Unescape
https://codebeautify.org/csharp-escape-unescape


### License
This project is Copyright &copy; 2023 [Thomas Galliker](https://ch.linkedin.com/in/thomasgalliker). Free for non-commercial use. For commercial use please contact the author.
