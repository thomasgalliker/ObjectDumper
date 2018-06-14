# ObjectDumper.NET
[![Version](https://img.shields.io/nuget/v/ObjectDumper.NET.svg)](https://www.nuget.org/packages/ObjectDumper.NET)  [![Downloads](https://img.shields.io/nuget/dt/ObjectDumper.NET.svg)](https://www.nuget.org/packages/ObjectDumper.NET)

<img src="https://raw.githubusercontent.com/thomasgalliker/ObjectDumper/master/ObjectDumper.png" width="100" height="100" alt="ObjectDumper" align="right"></img>

ObjectDumper is a utility which aims to serialize C# objects to string for debugging and logging purposes.

### Download and Install ObjectDumper.NET
This library is available on NuGet: https://www.nuget.org/packages/ObjectDumper.NET/
Use the following command to install ObjectDumper using NuGet package manager console:

    PM> Install-Package ObjectDumper.NET

You can use this library in any .Net project which is compatible to PCL (e.g. Xamarin Android, iOS, Windows Phone, Windows Store, Universal Apps, etc.)

### API Usage
#### Dumping C# Objects to Console.WriteLine
The following sample program shows how ObjectDumper can be used to write C# objects to the console output:
```
static void Main(string[] args)
{
	var persons = new List<Person>
	{
		new Person { Name = "John", Age = 20, },
		new Person { Name = "Thomas", Age = 30, },
	};

	// Act
	var personsDump = ObjectDumper.Dump(persons);

	Console.WriteLine(personsDump);
	Console.ReadLine();
}
```
The output on the console looks like following:
```
{ObjectDumperSample.Netfx.Person}
  Name: "John"
  Age: 20
{ObjectDumperSample.Netfx.Person}
  Name: "Thomas"
  Age: 30
```

### License
This project is Copyright &copy; 2018 [Thomas Galliker](https://ch.linkedin.com/in/thomasgalliker). Free for non-commercial use. For commercial use please contact the author.
