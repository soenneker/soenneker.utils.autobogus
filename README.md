[![](https://img.shields.io/nuget/v/soenneker.utils.autobogus.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.autobogus/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.autobogus/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.autobogus/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.autobogus.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.autobogus/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.autobogus/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.autobogus/actions/workflows/codeql.yml)

# Soenneker.Utils.AutoBogus

Automatic object creation and population backed by [Bogus](https://github.com/bchavez/Bogus).

It generates primitives, collections, dictionaries, and populated object graphs without requiring a rule for every member. Use `AutoFaker<T>` when you also need normal Bogus rules for a particular model.

## Installation

```bash
dotnet add package Soenneker.Utils.AutoBogus
```

## Usage

```csharp
var faker = new AutoFaker(new AutoFakerConfig
{
    RepeatCount = 3,
    RecursiveDepth = 1
});

string randomWord = faker.Generate<string>();
Dictionary<int, string> dictionary = faker.Generate<Dictionary<int, string>>();
Order order = faker.Generate<Order>();
List<Order> orders = faker.Generate<Order>(10);
```

Reuse an `AutoFaker` where practical. Its reflection metadata and Bogus state are initialized lazily and then retained. To make generated values repeatable, seed that instance before generating:

```csharp
faker.UseSeed(12345);
```

For a runtime `Type`, use the non-generic overload:

```csharp
object value = faker.Generate(modelType);
```

## `AutoFakerOverride`

Use an override when every generated instance of a type needs custom population:

```csharp
public class OrderOverride : AutoFakerOverride<Order>
{
    public override void Generate(AutoFakerOverrideContext context)
    {
        var target = (Order) context.Instance!;
        target.Id = 123;
        
        // Faker is available
        target.Name = context.Faker.Random.Word();

        // AutoFaker is also available
        target.Customer = context.AutoFaker.Generate<Customer>();
     }
}
```

Register the override in the configuration before generating:

```csharp
var config = new AutoFakerConfig
{
    Overrides = [new OrderOverride()]
};

var faker = new AutoFaker(config);
```

## `AutoFaker<T>`

`AutoFaker<T>` inherits from `Bogus.Faker<T>`, so explicit Bogus rules and automatic population can be combined. Members covered by a rule are not overwritten by automatic population.

```csharp
var orderFaker = new AutoFaker<Order>();
orderFaker.RuleFor(x => x.Id, f => f.Random.Int(1, 10_000));

Order order = orderFaker.Generate();
```

## Interfaces/Abstracts

The base library does not generate interfaces or abstract objects, but these enable you to generate mocks of them:

- [soenneker.utils.autobogus.moq](https://github.com/soenneker/soenneker.utils.autobogus.moq)
- [soenneker.utils.autobogus.nsubstitute](https://github.com/soenneker/soenneker.utils.autobogus.nsubstitute)
- [soenneker.utils.autobogus.fakeiteasy](https://github.com/soenneker/soenneker.utils.autobogus.fakeiteasy)

## Generation boundaries

- Set `TreeDepth`, `RecursiveDepth`, `SkipTypes`, or `SkipPaths` when models contain cycles or members that should not be populated.
- Interfaces and abstract classes require one of the mock-framework adapters listed above.
- `GenerateStatic<T>()` creates a new faker on every call. Prefer a retained `AutoFaker` for repeated generation.

## Benchmarks

### Soenneker.Utils.AutoBogus - `AutoFaker`

| Method           | Mean        | Error     | StdDev    |
|----------------- |------------:|----------:|----------:|
| Generate_int     |    79.40 ns |  0.635 ns |  0.563 ns |
| Generate_string  |   241.35 ns |  3.553 ns |  3.324 ns |
| Generate_complex | 6,782.34 ns | 43.811 ns | 38.837 ns |

### Soenneker.Utils.AutoBogus - `AutoFaker<T>`

| Method           | Mean       | Error    | StdDev   |
|----------------- |-----------:|---------:|---------:|
| Generate_string  |   283.6 ns |  3.28 ns |  3.07 ns |
| Generate_complex | 8,504.0 ns | 76.58 ns | 67.89 ns |

### AutoBogus

| Method           | Mean      | Error    | StdDev   |
|----------------- |----------:|---------:|---------:|
| Generate_int     |   1.17 ms | 0.033 ms | 0.026 ms |
| Generate_complex |  10.91 ms | 0.181 ms | 0.236 ms |

### Bogus

| Method       | Mean          | Error        | StdDev       |
|------------- |--------------:|-------------:|-------------:|
| Bogus_int    |      19.70 ns |     0.176 ns |     0.165 ns |
| Bogus_string |     171.75 ns |     2.763 ns |     2.585 ns |
| Bogus_ctor   | 730,669.06 ns | 8,246.622 ns | 7,310.416 ns |
