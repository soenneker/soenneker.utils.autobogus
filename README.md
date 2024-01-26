[![](https://img.shields.io/nuget/v/soenneker.utils.autobogus.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.autobogus/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.autobogus/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.autobogus/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.autobogus.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.autobogus/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.AutoBogus
### The .NET Bogus autogenerator 

This project is a replacement for the abandoned [AutoBogus](https://github.com/nickdodd79/AutoBogus) library. It's mostly plug and play. It aims to be fast, and support the latest types in .NET.

.NET Standard 2.1 is required. 

## Installation

```
dotnet add package Soenneker.Utils.AutoBogus
```

A Bogus `Faker` takes a long time to initialize, so AutoFaker will mirror Faker in this sense. Thus, `AutoFaker<T>` was dropped from this package.

⚠️ It is recommended that a single instance of `AutoFaker` be used if possible. The static usage of `AutoFaker.Generate<>()` should be avoided (as it incurs a new `Faker` allocation), but is available. 

This is a work in progress. Contribution is welcomed.