Feature: Sample
  A sample feature showing how to run the file from xunit

  Scenario: SDK project should restore
    Given Foo.csproj =
"""
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>netstandard2.0</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Newtonsoft.Json" Version="11.0.2" />
    </ItemGroup>
</Project>
"""
    And Class1.cs = 
"""
using System;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello");
        Console.ReadLine();
    }
}
"""
    When restoring packages
    Then restore succeeds