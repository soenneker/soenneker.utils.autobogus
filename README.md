[![](https://img.shields.io/nuget/v/soenneker.utils.autobogus.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.autobogus/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.autobogus/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.autobogus/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.autobogus.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.autobogus/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.AutoBogus
### The .NET Autogenerator 

This project is an automatic creator and populator for the fake data generator [Bogus](https://github.com/bchavez/Bogus).

It's a replacement for the abandoned [AutoBogus](https://github.com/nickdodd79/AutoBogus) library. It's mostly plug and play. 

The goals are to be *fast*, and support the latest types in .NET.

.NET 6+ is supported. 

## Installation

```
dotnet add package Soenneker.Utils.AutoBogus
```

## Notes

This is a work in progress. Contribution is welcomed.

⚠️ A Bogus `Faker` takes a long time to initialize, so AutoFaker will mirror Faker in this sense.

It's recommended that a single instance of `AutoFaker` be used if possible. The static usage of `AutoFaker.Generate<>()` should be avoided (as it creates a new `Faker`), but is available. 

## Benchmarks

### Soenneker.Utils.AutoBogus

| Method           | Mean         | Error      | StdDev     |
|----------------- |-------------:|-----------:|-----------:|
| Generate_int     |     78.81 ns |   0.764 ns |   0.677 ns |
| Generate_string  |    235.03 ns |   1.267 ns |   1.185 ns |
| Generate_complex | 19,868.49 ns | 180.241 ns | 150.509 ns |

### AutoBogus

| Method           | Mean      | Error    | StdDev   |
|----------------- |----------:|---------:|---------:|
| Generate_int     |   1.17 ms | 0.033 ms | 0.026 ms |
| Generate_complex |  10.91 ms | 0.181 ms | 0.236 ms |

### Bogus

| Method       | Mean          | Error         | StdDev       |
|------------- |--------------:|--------------:|-------------:|
| Bogus_int    |      19.58 ns |      0.150 ns |     0.133 ns |
| Bogus_string |     172.25 ns |      2.510 ns |     2.347 ns |
| Bogus_ctor   | 717,799.56 ns | 10,086.875 ns | 9,435.269 ns |