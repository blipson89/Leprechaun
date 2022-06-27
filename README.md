# Leprechaun
![Leprechaun](http://www.benlipson.net/wp-content/uploads/2018/04/leprechaun-logo_pipe-small.png)

Leprechaun is a universal, open API for code generation from [Rainbow](https://github.com/SitecoreUnicorn/Rainbow) serialized or Sitecore serialized Sitecore templates. Leprechaun uses state-of-the-art [Roslyn code generation](https://msdn.microsoft.com/en-us/magazine/mt707527.aspx) technology instead of T4 templates for speedy generation that doesn't require Visual Studio.

## Why should I use Leprechaun?
### It's Really Flexible
Leprechaun uses [C# Script](https://blogs.msdn.microsoft.com/visualstudio/2011/10/19/introducing-the-microsoft-roslyn-ctp/) files as templates. If you're familiar with C#, it's a breeze. Out of the box, a [Synthesis script file](https://github.com/blipson89/Leprechaun/blob/main/src/Leprechaun.CodeGen.Roslyn/Scripts/Synthesis.csx) is provided, but it can easily be adapted to generate templates Glass, Fortis, or more! The template is essentially a big `StringBuilder` that you can build out however you like and Leprechaun will fill in the rest. 
### It Can Be Run at Build-Time
Sick of trying to resolve merge conflicts in gigantic model files? You don't need to do that anymore with Leprechaun. The outputted model files don't have to be checked into source control at all!

Why not? 

Leprechaun generates models based off of the yaml files outputted by Rainbow. Since these yaml files are on disk already, there's no need for Sitecore to be running or for Visual Studio to be open in order to generate the models. Without these dependencies, you can generate models as a pre-build step. 
### It's Helix-Friendly
Most everything in Leprechaun is config-based. The base Leprechaun.config ([for Rainbow](https://github.com/blipson89/Leprechaun/blob/main/src/Leprechaun.InputProviders.Rainbow/Leprechaun.config), [for Sitecore Serialization](https://github.com/blipson89/Leprechaun/blob/main/src/Leprechaun.InputProviders.Sitecore/Leprechaun.config)) file contains everything needed for Leprechaun to get started for a single project. However, these configs can be extended and overridden. For each module you have, create a `CodeGen.config` file and have it extend another config.

For example, a module named `Feature.Promo` would contain a `CodeGen.config` file like this:

```xml
<configuration name="Feature.Promo" extends="Sample.Base">

</configuration>
```
For more information, read the comments in the `Leprechaun.config` file and check out [this Kam Figy blog post](https://kamsar.net/index.php/2017/02/Unicorn-4-Part-III-Configuration-Enhancements/) (for Unicorn, but the technique is the same).

## Getting Started
Ok, you're convinced. Now how do you set it up?

### Installing Leprechaun
Leprechaun is available as a dotnet tool. You can install it locally to a project, or globally. 

#### To install it globally:
Open a PowerShell window and execute the following command:
```powershell
dotnet tool install --global --no-cache Leprechaun.Cli
```
Then close and reopen PowerShell

#### To install it locally:
If your project does not have the following file: `.config\dotnet-tools.json`, go to the root of your project directory and type 
```
dotnet new tool-manifest
dotnet tool install --no-cache Leprechaun.Cli
```

### Updating Leprechaun
If you have Leprechaun installed locally, open a PowerShell window at the root of your project and run:
```
dotnet tool update --no-cache Leprechaun.Cli
```

If you have installed Leprechaun globally, open a PowerShell window anywhere you'd like and run:
```
dotnet tool update --global --no-cache Leprechaun.Cli
```

### Uninstalling Leprechaun
If you have Leprechaun installed locally, open a PowerShell window at the root of your project and run:
```
dotnet tool uninstall Leprechaun.Cli
```

If you have installed Leprechaun globally, open a PowerShell window anywhere you'd like and run:
```
dotnet tool uninstall --global Leprechaun.Cli
```

---

### Initial Configuration


#### General Steps
1. Grab the version of `Leprechaun.config` associated with your version of Leprechaun from GitHub:
    * [Leprechaun.config (for Rainbow)](https://github.com/blipson89/Leprechaun/blob/main/src/Leprechaun.InputProviders.Rainbow/Leprechaun.config)
    * [Leprechaun.config (for Sitecore Serialization)](https://github.com/blipson89/Leprechaun/blob/main/src/Leprechaun.InputProviders.Sitecore/Leprechaun.config)
1. Place the `Leprechaun.config` file somewhere close to the source code. In Helix solutions, I would recommend `src/`.
1. Grab a script template. Some pre-configured ones can be [found here (link)](https://github.com/blipson89/Leprechaun/tree/main/src/Leprechaun.CodeGen.Roslyn/Scripts).
1. Open up `Leprechaun.config` and update settings where applicable. Pay close attention to the following:
    1. `<configurations import=".....">`
        * This line is going to tell Leprechaun where to look for additional configurations, typically for modules. Wildcards accepted.
        * For **Helix** solutions with Rainbow: `.\*\*\code\CodeGen.config` *should* work.
        * For **Helix** Solutions with Sitecore Serialization: `**\*.module.json` *should* work.
    1. `<configuration name="Sample.Base">`
        * Recommend changing this to `[SolutionName].Base`
    1. `<codeGenerator scripts="..." outputFile="...">`
        * `scripts` are the script templates that will be used. Pre-configured ones can be downloaded from https://github.com/blipson89/Leprechaun/tree/main/src/Leprechaun.CodeGen.Roslyn/Scripts
        * `outputFile` - I recommend `$(configDirectory)\$(layer)\$(module)\code\Templates.cs` for Habitat
            * For **Helix** solutions using NuGet: `$(configDirectory)\..\..\..\$(layer)\$(module)\code\Models\Synthesis.Model.cs` *should* work. This is assuming you leave the .exe in the packages folder.
    1. `<templatePredicate rootNamespace="Sample.$(layer).$(module)">`
        * Replace `Sample` with the appropriate namespace
        * Make sure that the `<include>` matches the path your data templates. (see [Troubleshooting](#Troubleshooting))

#### Rainbow-Specific Configuration Steps
After following the General Steps:
1. Open up `Leprechaun.config` and update Rainbow-specific settings where applicable. Pay close attention to the following:
    1. `<dataStore physicalRootPath="...">`
        * folder where Rainbow YAML files are.
        * For **Helix** solutions: `$(configDirectory)\$(layer)\$(module)\serialization` *should* work
            * For **Helix** solutions using NuGet: `$(configDirectory)\..\..\..\$(layer)\$(module)\serialization`
    1. `<rainbowSettings type="Leprechaun.Console.LeprechaunRainbowSettings, Leprechaun.Console" ... />`
        * If you altered the path lengths in your Unicorn configuration, you will need to adjust them here as the comment mentions

#### Sitecore-Specific Configuration Steps
After following the General Steps:
1. Open up `Leprechaun.config` and update Rainbow-specific settings where applicable. Pay close attention to the following:
    1. `<moduleConfigReader type="..." configRootDirectory="..." singleInstance="true" />`
        * the `configRootDirectory` should be the path to your project's `sitecore.json` file

*Note: `Leprechaun.config` is NOT a file that gets deployed to the web server. It's purely used in build.*

---

### Module-Level Configuration

Now that the base configuration is setup, it's time to install the module-level configs. Create a config file in each module that contains a configuration name in the format `Layer.Module` and have it extend the base configuration (Step 2 from the Initial Configuration section).

Inside this config block, you *can* override any configurations from the main configuration file. The majority of the time, this won't be necessary. 

Examples:

#### Unicorn:
```xml
<configuration name="Feature.Promo" extends="Sample.Base">

</configuration>
```

#### Sitecore Serialization
For Sitecore Serialization, rather than create a specific module file, you can add a `leprechaun` node do the `.module.json` file. The `leprechaun` node is a json representation of the xml in the `Leprechaun.config` file.

In the following example, the "`items`" section is setup by Sitecore and was only included as a point of reference.

`Sample.module.json`
```json
{
  "namespace": "Feature.Sample",
  "items": {
    "includes": [
      {
        "name": "templates",
        "path": "/sitecore/templates/Feature/Sample"
      },
      {
        "name": "renderings",
        "path": "/sitecore/layout/Renderings/Feature/Sample"
      },
      {
        "name": "buttons",
        "database": "core",
        "path": "/sitecore/content/Applications/WebEdit/Custom Experience Buttons/Sample"
      }
    ]
  },
  "leprechaun": {
    "configuration": {
      "@extends": "Sample.Base",
      "@name": "Feature.Sample"
    },
  }
}
```

--- 

### Integration
There are a few ways possible you can integrate Leprechaun.

**NOTE: in all examples below, add the switch /r before the /c if you are using Rainbow**

**Example 1: Pre-Build Event in MSBuild**
```xml
  <PropertyGroup>
    <PreBuildEvent>dotnet leprechaun /c $(SolutionDir)\src\Leprechaun.config</PreBuildEvent>
  </PropertyGroup>
```

**Example 2: Create a gulp task from a custom install location**
```js
gulp.task('_Code-Generation', function (cb) {
    exec('dotnet leprechaun /c .\\src\\Leprechaun.config', function (err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        cb(err);
    });
})
```

**Example 3: You can create a project target**
```xml
<Target Name="Leprechaun">  
    <Exec Command="dotnet leprechaun /c $(SolutionDir)\src\Leprechaun.config"/>  
</Target>  
```

---

### Watch

Leprechaun has the ability to watch the yaml files and automatically regenerate models when there's a change. Run Leprechaun with the `/w` switch to turn this on.

Watch is *currently* not supported for Sitecore serialization.

---

### Migration from Synthesis Code Gen

If you are migrating from Synthesis Code Gen, have your models in source control, and Synthesis AutoRegenerate on non-local environments, you may want to note the following:

1. `<templatePredicate rootNamespace="Sample.$(layer).$(module)">`
	* You may want to use `$(layer).$(module).Models` if you used the [default namespace](https://github.com/blipson89/Synthesis/blob/main/Source/Synthesis/Standard%20Config%20Files/Synthesis.config#L29).
2. You will no longer need your models in source control. You should remove them from source control and add them to your ignores (e.g. `.gitignore` or similar functionality)
3. You should disable Synthesis AutoRegenerate in non-local environments by removing it or transforming its event nodes out of the config in those environments.
    * Recommendation: disable Synthesis AutoRegenerate on local environments as well, and use Leprechaun's watch instead

---

## Upgrading from Leprechaun 1.x

### Updating your Leprechaun.config
The best way to upgrade your `Leprechaun.config` is to compare your file against the latest from the repo. 

1. Add the following lines to your `<shared>` configuration:
    ```xml
    <orchestrator type="Leprechaun.Orchestrator, Leprechaun" singleInstance="true" />
    <inputProvider type="Leprechaun.InputProviders.Rainbow.RainbowInputProvider, Leprechaun.InputProviders.Rainbow" 
    singleInstance="true" />
    <watcher type="Leprechaun.InputProviders.Rainbow.RainbowWatcher, Leprechaun.InputProviders.Rainbow" singleInstance="true" />
    ```
1. Update namespaces for the following:
    1. `Leprechaun.Console.ConsoleLogger, Leprechaun.Console` becomes `Leprechaun.Execution.ConsoleLogger, Leprechaun`
    1. `Leprechaun.Console.LeprechaunRainbowSettings, Leprechaun.Console` becomes `Leprechaun.InputProviders.Rainbow.Modules.RainbowSettingsModule, Leprechaun.InputProviders.Rainbow`
    1. the `<rainbowSettings>` node has been renamed to `<rainbowSettingsModule>`
    1. `Leprechaun.Filters.StandardTemplatePredicate, Leprechaun` becomes `Leprechaun.InputProviders.Rainbow.Filters.StandardTemplatePredicate, Leprechaun.InputProviders.Rainbow`
    1. `Leprechaun.TemplateReaders.DataStoreTemplateReader, Leprechaun` becomes `Leprechaun.InputProviders.Rainbow.TemplateReaders.DataStoreTemplateReader`
    1. `Leprechaun.Filters.RainbowNullFieldFilter, Leprechaun` becomes `Leprechaun.InputProviders.Rainbow.Filters.RainbowNullFieldFilter, Leprechaun.InputProviders.Rainbow`

### Updating your build process
1. Install the `leprechaun.cli` dotnet tool (see installation section above)
1. Update build scripts to add the `/r` switch before the `/c` if you are using Rainbow (i.e. `dotnet leprechaun /r /c "path/to/config/file"`)

---

## Troubleshooting

### The model file is generated, but there are no templates in it!
The template predicate is probably not set correctly for your solution. In `Leprechaun.config`, take a peek at the `<include>` in this section:
```xml
<templatePredicate type="Leprechaun.Filters.StandardTemplatePredicate, Leprechaun" rootNamespace="$(layer).$(module)" singleInstance="true">
	<include name="Templates" path="/sitecore/templates/$(layer)/$(module)" />
</templatePredicate>
```
