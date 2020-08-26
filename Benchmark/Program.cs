using System.Text;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using JsonCodeGeneration;

namespace Benchmark
{
    class Program
    {
        private static readonly string s_benchmarkCommandSample = "e.g. dotnet run Benchmarks Serialize Default LoginViewModel";
        private static readonly string s_benchmarkArgumentIncorrect = "Benchmark arguments are incorrect.";

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException(s_benchmarkArgumentIncorrect);
            }

            string process = args[0];
            string mechanism = args[1];
            string type = args[2];

            if (process == "Serialize")
            {
                RunSerializationBenchmark(mechanism, type);
            }
            else if (process == "Deserialize")
            {
                RunDeserializationBenchmark(mechanism, type);
            }
            else
            {
                throw new ArgumentException(s_benchmarkArgumentIncorrect);
            }
        }

        // Deserialization methods.
        private static void RunDeserializationBenchmark_Default<T>()
        {
            string serialized = GetJson(typeof(T));

            var sw = new Stopwatch();
            sw.Start();
            JsonSerializer.Deserialize<T>(serialized);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        private static void RunDeserializationBenchmark_Jil<T>()
        {
            string serialized = GetJson(typeof(T));

            var sw = new Stopwatch();
            sw.Start();
            Jil.JSON.Deserialize<T>(serialized, Jil.Options.ISO8601);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        private static void RunDeserializationBenchmark_JsonNet<T>()
        {
            string serialized = GetJson(typeof(T));

            var sw = new Stopwatch();
            sw.Start();
            Newtonsoft.Json.JsonConvert.DeserializeObject<T>(serialized);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        private static void RunDeserializationBenchmark_Utf8Json<T>()
        {
            string serialized = GetJson(typeof(T));

            var sw = new Stopwatch();
            sw.Start();
            Utf8Json.JsonSerializer.Deserialize<T>(serialized);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        private static void RunDeserializationBenchmark_SourceGenerator<T>()
        {
            var sw = new Stopwatch();
            string value = GetJson(typeof(T));

            if (typeof(T) == typeof(Location))
            {
                sw.Start();
                Location obj = JsonSerializer.Deserialize(value, JsonContext.Instance.Location);
                sw.Stop();
            }
            else if (typeof(T) == typeof(LoginViewModel))
            {
                sw.Start();
                LoginViewModel obj = JsonSerializer.Deserialize(value, JsonContext.Instance.LoginViewModel);
                sw.Stop();
            }
            else if (typeof(T) == typeof(IndexViewModel))
            {
                sw.Start();
                IndexViewModel obj = JsonSerializer.Deserialize(value, JsonContext.Instance.IndexViewModel);
                sw.Stop();
            }

            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        // Serialization methods.
        private static void RunSerializationBenchmark_Default<T>()
        {
            T value = DataGenerator.Generate<T>();

            // System.Text.Json
            var options = new JsonSerializerOptions();

            var sw = new Stopwatch();
            sw.Start();
            JsonSerializer.Serialize(value, options);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        private static void RunSerializationBenchmark_SourceGenerator<T>()
        {
            var sw = new Stopwatch();

            if (typeof(T) == typeof(Location))
            {
                Location  value = DataGenerator.Generate<Location>();

                sw.Start();
                string json = JsonSerializer.Serialize(value, (JsonTypeInfo<Location>)JsonContext.Instance.Location);
                sw.Stop();
            }
            else if (typeof(T) == typeof(LoginViewModel))
            {
                LoginViewModel value = DataGenerator.Generate<LoginViewModel>();

                sw.Start();
                string json = JsonSerializer.Serialize(value, (JsonTypeInfo<LoginViewModel>)JsonContext.Instance.LoginViewModel);
                sw.Stop();
            }
            else if (typeof(T) == typeof(IndexViewModel))
            {
                IndexViewModel value = DataGenerator.Generate<IndexViewModel>();

                sw.Start();
                string json = JsonSerializer.Serialize(value, JsonContext.Instance.IndexViewModel);
                sw.Stop();
            }

            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        private static void RunSerializationBenchmark_Jil<T>()
        {
            T value = DataGenerator.Generate<T>();

            var sw = new Stopwatch();
            sw.Start();
            Jil.JSON.Serialize<T>(value, Jil.Options.ISO8601);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        private static void RunSerializationBenchmark_JsonNet<T>()
        {
            T value = DataGenerator.Generate<T>();

            var sw = new Stopwatch();
            sw.Start();
            Newtonsoft.Json.JsonConvert.SerializeObject(value);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        private static void RunSerializationBenchmark_Utf8Json<T>()
        {
            T value = DataGenerator.Generate<T>();

            var sw = new Stopwatch();
            sw.Start();
            Utf8Json.JsonSerializer.ToJsonString(value);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        // Helper methods.
        private static readonly string s_payloadDir = Path.Join(Directory.GetCurrentDirectory(), "payloads");
        private static string GetJson(Type type) => File.ReadAllText(Path.Join(s_payloadDir, $"{type.Name}.json"));

        // (De)serialization switchcases.
        private static void RunSerializationBenchmark(string mechanism, string type)
        {
            switch (mechanism)
            {
                case "System.Text.Json":
                    {
                        switch (type)
                        {
                            case "LoginViewModel":
                                {
                                    RunSerializationBenchmark_Default<LoginViewModel>();
                                }
                                break;
                            case "Location":
                                {
                                    RunSerializationBenchmark_Default<Location>();
                                }
                                break;
                            case "IndexViewModel":
                                {
                                    RunSerializationBenchmark_Default<IndexViewModel>();
                                }
                                break;
                            default:
                                throw new ArgumentException(s_benchmarkCommandSample);
                        }
                    }
                    break;
                case "Jil":
                    {
                        switch (type)
                        {
                            case "LoginViewModel":
                                {
                                    RunSerializationBenchmark_Jil<LoginViewModel>();
                                }
                                break;
                            case "Location":
                                {
                                    RunSerializationBenchmark_Jil<Location>();
                                }
                                break;
                            case "IndexViewModel":
                                {
                                    RunSerializationBenchmark_Jil<IndexViewModel>();
                                }
                                break;
                            default:
                                throw new ArgumentException(s_benchmarkCommandSample);
                        }
                    }
                    break;
                case "Json.NET":
                    {
                        switch (type)
                        {
                            case "LoginViewModel":
                                {
                                    RunSerializationBenchmark_JsonNet<LoginViewModel>();
                                }
                                break;
                            case "Location":
                                {
                                    RunSerializationBenchmark_JsonNet<Location>();
                                }
                                break;
                            case "IndexViewModel":
                                {
                                    RunSerializationBenchmark_JsonNet<IndexViewModel>();
                                }
                                break;
                            default:
                                throw new ArgumentException(s_benchmarkCommandSample);
                        }
                    }
                    break;
                case "Utf8Json":
                    {
                        switch (type)
                        {
                            case "LoginViewModel":
                                {
                                    RunSerializationBenchmark_Utf8Json<LoginViewModel>();
                                }
                                break;
                            case "Location":
                                {
                                    RunSerializationBenchmark_Utf8Json<Location>();
                                }
                                break;
                            case "IndexViewModel":
                                {
                                    RunSerializationBenchmark_Utf8Json<IndexViewModel>();
                                }
                                break;
                            default:
                                throw new ArgumentException(s_benchmarkCommandSample);
                        }
                    }
                    break;
                case "SourceGeneration":
                    {
                        switch (type)
                        {
                            case "LoginViewModel":
                                {
                                    RunSerializationBenchmark_SourceGenerator<LoginViewModel>();
                                }
                                break;
                            case "Location":
                                {
                                    RunSerializationBenchmark_SourceGenerator<Location>();
                                }
                                break;
                            case "IndexViewModel":
                                {
                                    RunSerializationBenchmark_SourceGenerator<IndexViewModel>();
                                }
                                break;
                            default:
                                throw new ArgumentException(s_benchmarkCommandSample);
                        }
                    }
                    break;
                default:
                    throw new ArgumentException(s_benchmarkCommandSample);
            }
        }

        private static void RunDeserializationBenchmark(string mechanism, string type)
        {
            switch (mechanism)
            {
                case "System.Text.Json":
                    {
                        switch (type)
                        {
                            case "LoginViewModel":
                                {
                                    RunDeserializationBenchmark_Default<LoginViewModel>();
                                }
                                break;
                            case "Location":
                                {
                                    RunDeserializationBenchmark_Default<Location>();
                                }
                                break;
                            case "IndexViewModel":
                                {
                                    RunDeserializationBenchmark_Default<IndexViewModel>();
                                }
                                break;
                            default:
                                throw new ArgumentException(s_benchmarkCommandSample);
                        }
                    }
                    break;
                case "Jil":
                    {
                        switch (type)
                        {
                            case "LoginViewModel":
                                {
                                    RunDeserializationBenchmark_Jil<LoginViewModel>();
                                }
                                break;
                            case "Location":
                                {
                                    RunDeserializationBenchmark_Jil<Location>();
                                }
                                break;
                            case "IndexViewModel":
                                {
                                    RunDeserializationBenchmark_Jil<IndexViewModel>();
                                }
                                break;
                            default:
                                throw new ArgumentException(s_benchmarkCommandSample);
                        }
                    }
                    break;
                case "Json.NET":
                    {
                        switch (type)
                        {
                            case "LoginViewModel":
                                {
                                    RunDeserializationBenchmark_JsonNet<LoginViewModel>();
                                }
                                break;
                            case "Location":
                                {
                                    RunDeserializationBenchmark_JsonNet<Location>();
                                }
                                break;
                            case "IndexViewModel":
                                {
                                    RunDeserializationBenchmark_JsonNet<IndexViewModel>();
                                }
                                break;
                            default:
                                throw new ArgumentException(s_benchmarkCommandSample);
                        }
                    }
                    break;
                case "Utf8Json":
                    {
                        switch (type)
                        {
                            case "LoginViewModel":
                                {
                                    RunDeserializationBenchmark_Utf8Json<LoginViewModel>();
                                }
                                break;
                            case "Location":
                                {
                                    RunDeserializationBenchmark_Utf8Json<Location>();
                                }
                                break;
                            case "IndexViewModel":
                                {
                                    RunDeserializationBenchmark_Utf8Json<IndexViewModel>();
                                }
                                break;
                            default:
                                throw new ArgumentException(s_benchmarkCommandSample);
                        }
                    }
                    break;
                case "SourceGeneration":
                    {
                        switch (type)
                        {
                            case "LoginViewModel":
                                {
                                    RunDeserializationBenchmark_SourceGenerator<LoginViewModel>();
                                }
                                break;
                            case "Location":
                                {
                                    RunDeserializationBenchmark_SourceGenerator<Location>();
                                }
                                break;
                            case "IndexViewModel":
                                {
                                    RunDeserializationBenchmark_SourceGenerator<IndexViewModel>();
                                }
                                break;
                            default:
                                throw new ArgumentException(s_benchmarkCommandSample);
                        }
                    }
                    break;
                default:
                    throw new ArgumentException(s_benchmarkCommandSample);
            }
        }
    }
}

