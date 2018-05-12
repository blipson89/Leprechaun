# Leprechaun
![Leprechaun](http://www.benlipson.net/wp-content/uploads/2018/04/leprechaun-logo_pipe-small.png)

Leprechaun is a universal, open API for code generation from [Rainbow](https://github.com/kamsar/Rainbow) serialized Sitecore templates. Leprechaun uses state-of-the-art [Roslyn code generation](https://msdn.microsoft.com/en-us/magazine/mt707527.aspx) technology instead of T4 templates for speedy generation that doesn't require Visual Studio.

## Why should I use Leprechaun?
### It's Really Flexible
Leprechaun uses [C# Script](https://blogs.msdn.microsoft.com/visualstudio/2011/10/19/introducing-the-microsoft-roslyn-ctp/) files as templates. If you're familiar with C#, it's a breeze. Out of the box, a [Synthesis script file](https://github.com/blipson89/Leprechaun/blob/master/src/Leprechaun.CodeGen.Roslyn/Scripts/Synthesis.csx) is provided, but it can easily be adapted to generate templates Glass, Fortis, or more! The template is essentially a big `StringBuilder` that you can build out however you like and Leprechaun will fill in the rest. 
### It Can Be Run at Build-Time
Sick of trying to resolve merge conflicts in gigantic model files? You don't need to do that anymore with Leprechaun. The outputted model files don't have to be checked into source control at all!

Why not? 

Leprechaun generates models based off of the yaml files outputted by Rainbow. Since these yaml files are on disk already, there's no need for Sitecore to be running or for Visual Studio to be open in order to generate the models. Without these dependencies, you can generate models as a pre-build step. 
### It's Helix-Friendly
Most everything in Leprechaun is config-based. The base [Leprechaun.config](https://github.com/blipson89/Leprechaun/blob/master/src/Leprechaun.Console/Leprechaun.config) file contains everything needed for Leprechaun to get started for a single project. However, these configs can be extended and overridden. For each module you have, create a `CodeGen.config` file and have it extend another config.

For example, a module named `Feature.Promo` would contain a `CodeGen.config` file like this:

```xml
<configuration name="Feature.Promo" extends="Sample.Base">

</configuration>
```
For more information, See [this part](https://github.com/blipson89/Leprechaun/blob/master/src/Leprechaun.Console/Leprechaun.config#L31-L46) of the `Leprechaun.config` file and check out [this Kam Figy blog post](https://kamsar.net/index.php/2017/02/Unicorn-4-Part-III-Configuration-Enhancements/) (for Unicorn, but the technique is the same).

## Getting Started
Ok, you're convinced. Now how do you set it up?
### Downloading Leprechaun
There are two ways to download Leprechaun:
1. The best way to download it is via the [Leprechaun-VERSION.zip](https://github.com/blipson89/Leprechaun/releases) file under Assets from the latest release. 
2. Alternatively, it is available on [NuGet](https://www.nuget.org/packages/Leprechaun.Console)

---

### Extraction
Leprechaun is a build tool. As such, once downloaded you can put it wherever you want in the project. For example, on my projects, I will extract the contents of the zip folder into `(project root)\lib\tools\Leprechaun`. 

***Note:** Leprechaun is a build tool. Nothing in this folder should be deployed to the web server.*

---

### Initial Configuration
Once extracted, copy the `Leprechaun.config` file somewhere close to the source code. In Helix solutions, I would recommend `src/`.

Open up `Leprechaun.config` and update settings where applicable. Pay close attention to the following:

1. `<configurations import=".....">`
    * This line is going to tell Leprechaun where to look for additional configurations, typically for modules. Wildcards accepted.
    * For **Helix** solutions: `.\*\*\code\CodeGen.config` *should* work.
2. `<configuration name="Sample.Base">`
    * Recommend changing this to `[SolutionName].Base`
3. `<codeGenerator scripts="..." outputFile="...">`
    * `scripts` are the CSX templates that will be used. Currently, Synthesis, GlassMapper and Habitat examples are provided.
    * `outputFile` - I recommend `$(configDirectory)\$(layer)\$(module)\code\Templates.cs` for Habitat
4. `<dataStore physicalRootPath="...">`
    * folder where Rainbow YAML files are.
    * For **Helix** solutions: `$(configDirectory)\$(layer)\$(module)\serialization` *should* work
5. `<templatePredicate rootNamespace="Sample.$(layer).$(module)">`
    * Replace `Sample` with the appropriate namespace


Now that the base configuration is setup, it's time to install the module-level configs. Create a config file in each module that contains a configuration name in the format `Layer.Module` and have it extend the base configuration (Step 2 from the Initial Configuration section).

Example:
```xml
<configuration name="Feature.Promo" extends="Sample.Base">

</configuration>
```

Inside this config block, you *can* override any configurations from the main configuration file. The majority of the time, this won't be necessary. 


*Note: `Leprechaun.config` is NOT a file that gets deployed to the web server. It's purely used in build.*

--- 

### Integration
There are a few ways possible you can integrate Leprechaun.

1. You can integrate it directly into your build as a pre-build event. Example:
```
  <PropertyGroup>
    <PreBuildEvent>$(SolutionDir)\lib\tools\Leprechaun\Leprechaun.console.exe /c $(SolutionDir)\src\Leprechaun.config</PreBuildEvent>
  </PropertyGroup>
```
2. You can integrate it into your build scripts - example: you can wrap it in a gulp task
```
gulp.task('_Code-Generation', function (cb) {
    exec('.\\lib\\tools\\Leprechaun\\Leprechaun.console.exe /c .\\src\\Leprechaun.config', function (err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        cb(err);
    });
})
```
3. You can create a project target
```
<Target Name="Leprechaun">  
    <Exec Command="$(SolutionDir)\lib\tools\Leprechaun\Leprechaun.console.exe /c $(SolutionDir)\src\Leprechaun.config"/>  
</Target>  
```

---

### Watch

Leprechaun has the ability to watch the yaml files and automatically regenerate models when there's a change. Run Leprechaun with the `/w` switch to turn this on