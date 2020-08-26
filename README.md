# JsonSerializer using Roslyn SourceGenerators Benchmark

This is a simple benchmark to showcase my project where I used Roslyn's SourceGenerators in order to create type metadata a compile time. This work was done as my Microsoft Summer Internship for DotNet Libraries Team.

For more information on this project, you can look at the github [issue](https://github.com/dotnet/runtime/issues/1568​) regarding this project along with the [design doc](https://github.com/dotnet/runtimelab/pull/16/files​) for this project.

## Benchmark details

### Main aspects
There were 3 main things that wanted to be benchmarked.

- Startup time reduction​

    Creation of metadata and compile-time allows us to not use runtime-reflection to create metadata.​

- Throughput efficiency​

    New overloads allow less dictionary lookups and direct usage of core (de)serialization methods making this method more efficient.​

- Private memory usage reduction​

    Not using reflection IL emit code along with the exposed metadata creation should reduce overall the memory usage.​

### How this was benchmarked

- Crossgen my changed System.Text.Json to avoid the benchmark from going into the Jit.​

    1. Verified it was crossgen'ed using (ILSpy) && size changes.​

    2. Verified it was being used by the benchmark app by using ProcessExplorer and see the path of the loaded DLL.​

- Created a simple console app benchmark.​

    1. Publish it using the following command

        ```dotnet publish -c Release -p:PublishReadyToRun=true --runtime win-x64​```

    2. Substitute the System.Text.Json DLL of the Benchmark project with the crossgen'd one created above.​

    3. Used Stopwatch to measure startup time and throughput.​

    4. Used ProcessExplorer for private memory usage check.​

## Steps to reproduce

For now I added all the necessary DLL where the System.Text.Json DLL is the crossgen'd version.

However, in order to crossgen it yourself you can run the following command (your paths will vary):

```
crossgen -nologo -readytorun -in C:\Users\t-kywo\Documents\runtimelab\artifacts\bin\System.Text.Json\net5.0-Debug\System.Text.Json.dll -out C:\Users\t-kywo\Documents\System.Text.Json.dll -platform_assemblies_paths C:\Users\t-kywo\Documents\runtimelab\artifacts\bin\runtime\net5.0-Windows_NT-Debug-x64\;C:\Users\t-kywo\Documents\runtimelab\artifacts\bin\coreclr\Windows_NT.x64.Release\;C:\Users\t-kywo\Documents\runtimelab\artifacts\bin\coreclr\ -JITPath C:\Users\t-kywo\Documents\runtimelab\artifacts\bin\coreclr\Windows_NT.x64.Release\clrjit.dll
```

Once crossgen'd DLL is created I put it inside the published app so the crossgen'd DLL is used.

## Results

I used a simple python script taken from a [similar project](https://github.com/layomia/jsonconvertergenerator/blob/master/run_benchmarks.py) by @layomia to run the tests. After running it multiple times the following approximations were made (using System.Text.Json as baseline):

StartupTime Comparisons Avg:

| Test                                  | Ratio |
|--------------------                   |-------|
| System.Text.Json.SourceGeneration     | 0.8265728301  |
| System.Text.Json                      | 1.00  |
| Json.NET                              | 2.134768646  |
| Utf8Json                              | 2.015112566  |
| Jil                                   | 3.036730074  |


Throughput Comparisons Avg:

| Ratio (System.Text.Json/SourceGeneration) |
|-------|
| 3.42 |
| 4.244  |
| 4  |
| 4.5  |
| 5  |
| 4.52  |
| 4.142  |
| 5.4  |
| 3.811  |
| 5.73  |
| 4.31  |
| 4.72  |
| 4.943  |
| 4.577  |
| 5.19  |

Average: 4.567x faster in ColdStart where we create new options instance every time.
