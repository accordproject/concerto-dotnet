<h1 align="center">
  <a href="https://www.accordproject.org/projects/concerto">
    Concerto for .NET
  <a/>
</h1>

<p align="center">
  <a href="https://discord.gg/Zm99SKhhtA">
    <img src="https://img.shields.io/badge/Accord%20Project-Join%20Discord-blue" alt="Join the Accord Project Discord"/>
  </a>
</p>

## Introduction

This is a prototype implementation of the [Concerto Schema Language](https://docs.accordproject.org/docs/model-concerto.html) for .NET written in C#. The current [reference implementation](https://github.com/accordproject/concerto) is written in JavaScript.

This repository contains:
- Serialization and Deserialization utilities for `System.Text.Json` and `Newtonsoft.Json`.

## Basic Usage

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using Concerto.Serialization;
using Concerto.Models.org.test;
 
//...

    JsonSerializerOptions options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(),
            new ConcertoConverter(),
        }
    };

    Employee employee = new Employee()
    {
        firstName = "Matt",
        lastName = "Roberts",
        email = "test@example.com",
        _identifier = "test@example.com",
        department = Department.ENGINEERING,
        employeeId = "123"
    };

    string jsonString = JsonSerializer.Serialize(employee, options);

    // jsonString contains
    //{
    //     "$class": "org.test.Employee",
    //     "department": "ENGINEERING",
    //     "employeeId": "123",
    //     "email": "test@example.com",
    //     "firstName": "Matt",
    //     "lastName": "Roberts",
    //     "$identifier": "test@example.com"
    // }
  
```

> This repository uses statically built versions of the internal `Concerto.Models.concerto` and `Concerto.Models.concerto.metamodel` namespaces. In the future this should be build dynamically using the Concerto CLI. There is an [open branch with the changes needed to `concerto-tools` package](https://github.com/mttrbrts/composer-concerto/blob/mr-csharp-newtonsoft/packages/concerto-tools/lib/codegen/fromcto/csharp/csharpvisitor.js). You can build the C# classes from the Concerto source files (`.cto`) will the following command:
>
> `concerto compile --model ./models/concerto.metamodel.cto --model ./models/employee.cto --target csharp --output ./`

## Running Tests

```sh
  $ cd ConcertoJsonConverter.Tests
  $ dotnet restore
  $ dotnet build --configuration Release --no-restore
  $ dotnet test --no-restore --verbosity normal
```

## Limitations

Support for polymorphic deserialization & serialization is limited in the popular C# JSON libraries. We provide an exemplar Converter for both, each has different limitations.

### System.Text.Json

System.Text.Json does not have any support for Polymorphic deserialization or serialization, so we provide our own implementation. The current implementation cannot inherit the default behaviour for `JsonSerializerOptions`. We attempt to mirror the basic features needed for Concerto (such as `NullHandling` and `JsonProperty` annotations), however other features are not supported.

### Newtonsoft JSON.net

Newtonsoft does support basic polymorphic deserialization, however, it doesn't not support support deserialization to a supertype (see `SimpleObject_DeserializeToSuperType_Succeeds` in [./ConcertoJsonConverter.Tests/newtonsoft/Deserialize.cs](ConcertoJsonConverter.Tests/newtonsoft/Deserialize.cs)).


---

<p align="center">
  <a href="./LICENSE">
    <img src="https://img.shields.io/github/license/accordproject/cicero?color=bright-green" alt="GitHub license">
  </a>
  <a href="https://discord.gg/Zm99SKhhtA/">
    <img src="https://img.shields.io/badge/Accord%20Project-Join%20Discord-blue" alt="Join the Accord Project Discord"/>
  </a>
</p>

Accord Project is an open source, non-profit, initiative working to transform contract management and contract automation by digitizing contracts. Accord Project operates under the umbrella of the [Linux Foundation][linuxfound]. The technical charter for the Accord Project can be found [here][charter].

## Learn More About Accord Project

### [Overview][apmain]

### [Documentation][apdoc]

## Contributing

The Accord Project technology is being developed as open source. All the software packages are being actively maintained on GitHub and we encourage organizations and individuals to contribute requirements, documentation, issues, new templates, and code.

Find out what’s coming on our [blog][apblog].

Join the Accord Project Technology Working Group [Discord Community][apdiscord] to get involved!

For code contributions, read our [CONTRIBUTING guide][contributing] and information for [DEVELOPERS][developers].

### README Badge

Using Accord Project? Add a README badge to let everyone know: [![accord project](https://img.shields.io/badge/powered%20by-accord%20project-19C6C8.svg)](https://www.accordproject.org/)

```
[![accord project](https://img.shields.io/badge/powered%20by-accord%20project-19C6C8.svg)](https://www.accordproject.org/)
```

## License <a name="license"></a>

Accord Project source code files are made available under the [Apache License, Version 2.0][apache].
Accord Project documentation files are made available under the [Creative Commons Attribution 4.0 International License][creativecommons] (CC-BY-4.0).

Copyright 2018-2019 Clause, Inc. All trademarks are the property of their respective owners. See [LF Projects Trademark Policy](https://lfprojects.org/policies/trademark-policy/).

[linuxfound]: https://www.linuxfoundation.org
[charter]: https://github.com/accordproject/governance/blob/master/accord-project-technical-charter.md
[apmain]: https://accordproject.org/ 
[apblog]: https://medium.com/@accordhq
[apdoc]: https://docs.accordproject.org/
[apdiscord]: https://discord.com/invite/Zm99SKhhtA

[contributing]: https://github.com/accordproject/concerto/blob/master/CONTRIBUTING.md
[developers]: https://github.com/accordproject/concerto/blob/master/DEVELOPERS.md

[apache]: https://github.com/accordproject/concerto/blob/master/LICENSE
[creativecommons]: http://creativecommons.org/licenses/by/4.0/
