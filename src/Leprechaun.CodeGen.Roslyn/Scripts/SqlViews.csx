// Generates SQL views for relevant templates
// ===
// * The file that will be output using this script is a .sql file and can be run on a Sitecore master database.
// * After running the .sql script, you'll find a list of views that live in a separate schema and can be used to easily get access to Sitecore items with different templates.
// * The views will have columns for all the Sitecore fields.
// * Each record in the result of a query on the view will represent 1 version in 1 language of an item with the template of the view.
// ===
// Originally based on this: https://github.com/hermanussen/CompiledDomainModel/blob/master/CompiledDomainModel/CompiledDomainModel/sitecore%20modules/Shell/CompiledDomainModel/CustomGenerators/SqlViewsGenerator.ascx

Log.Debug($"Emitting SQL views for {ConfigurationName}...");

public string RenderFields(TemplateCodeGenerationMetadata template, string schemaName)
{
	if (template.OwnFields.Length == 0)
	{
		return string.Empty;
	}

	var localCode = new System.Text.StringBuilder();

	foreach (var field in template.OwnFields)
	{
		localCode.AppendLine($@", (select fl.Value from {schemaName}.AllFields fl where fl.FieldId = '{field.Id}' and fl.ItemId = it.ID and fl.Language = af.Language and fl.Version = af.Version) as '{field.CodeName}'");
	}

	return localCode.ToString();
}
public string RenderTemplates(string schemaName)
{
	var localCode = new System.Text.StringBuilder();
	
	foreach(var template in Templates)
	{
		localCode.AppendLine($@"
create view {schemaName}.{template.CodeName.Replace('.', '_').Trim(new [] { '_' })}
as
select it.ID as ItemID, it.Name as ItemName, it.Created as ItemCreated, it.Updated as ItemUpdated, af.Language as ItemLanguage, af.Version as ItemVersion, af.IsLatestVersion
{RenderFields(template, schemaName)} from Items it
    inner join (select allF.ItemID, allF.Language, allF.Version, allF.IsLatestVersion 
					 from {schemaName}.AllFields allF
					 where allF.Language != ''
					 group by allF.ItemID, allF.Language, allF.Version, allF.IsLatestVersion) as af on it.ID = af.ItemID
where it.TemplateID = '{template.Id}' and af.Version > 0
go
		");
	}

	return localCode.ToString();
}

string schemaName = $"Views_{ConfigurationName.Replace('.', '_')}";

Code.AppendLine($@"
-- First, remove the schema (including all the views in it) if it exists
if exists(select 1 from information_schema.schemata where
schema_name='{schemaName}')
begin

DECLARE @ViewName varchar(100)

DECLARE my_cursor CURSOR FOR
select TABLE_NAME from INFORMATION_SCHEMA.VIEWS where TABLE_SCHEMA = '{schemaName}'

OPEN my_cursor

FETCH NEXT FROM my_cursor
INTO @ViewName

WHILE @@FETCH_STATUS = 0
BEGIN
    EXEC ('drop view {schemaName}.' + @ViewName);

    FETCH NEXT FROM my_cursor
    INTO @ViewName
END

CLOSE my_cursor
DEALLOCATE my_cursor

EXEC('drop schema {schemaName}');

end

-- Create the schema
EXEC('create schema {schemaName}');
go

-- Helper view that unions all fields
create view {schemaName}.AllFields
as
SELECT     Id, ItemId, '' AS Language, FieldId, Value, Created, Updated, 0 as Version, NULL as IsLatestVersion
FROM         dbo.SharedFields
UNION ALL
SELECT     Id, ItemId, Language, FieldId, Value, Created, Updated, 0 as Version, NULL as IsLatestVersion
FROM         dbo.UnversionedFields
UNION ALL
SELECT     vf1.Id, vf1.ItemId, vf1.Language, vf1.FieldId, vf1.Value, 
                      vf1.Created, vf1.Updated, vf1.[Version], Case vf1.[Version] when (select max(vf2.Version) from dbo.VersionedFields vf2 where vf2.ItemId = vf1.ItemId and vf1.Language = vf2.Language)  then 'Yes' else 'No' end as IsLatestVersion
FROM         dbo.VersionedFields vf1
go


{RenderTemplates(schemaName)}
");