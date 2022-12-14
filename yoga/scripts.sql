Build started...
Build succeeded.
The Entity Framework tools version '6.0.0' is older than that of the runtime '7.0.0-preview.6.22329.4'. Update the tools for the latest features and bug fixes. See https://aka.ms/AAc1fbw for more information.
warn: Microsoft.EntityFrameworkCore.Model.Validation[30000]
      No store type was specified for the decimal property 'Price' on entity type 'MembershipCard'. This will cause values to be silently truncated if they do not fit in the default precision and scale. Explicitly specify the SQL server column type that can accommodate all the values in 'OnModelCreating' using 'HasColumnType', specify precision and scale using 'HasPrecision', or configure a value converter using 'HasConversion'.
warn: Microsoft.EntityFrameworkCore.Model.Validation[30000]
      No store type was specified for the decimal property 'LicenseFeesPrice' on entity type 'TechearMemberShip'. This will cause values to be silently truncated if they do not fit in the default precision and scale. Explicitly specify the SQL server column type that can accommodate all the values in 'OnModelCreating' using 'HasColumnType', specify precision and scale using 'HasPrecision', or configure a value converter using 'HasConversion'.
info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
      Entity Framework Core 7.0.0-preview.6.22329.4 initialized 'YogaAppDbContext' using provider 'Microsoft.EntityFrameworkCore.SqlServer:7.0.0-preview.6.22329.4' with options: None
BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TechearMemberShips]') AND [c].[name] = N'SchoolSocialMediaAccount');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [TechearMemberShips] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [TechearMemberShips] ALTER COLUMN [SchoolSocialMediaAccount] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20221030190113_sshollsocialnull', N'7.0.0-preview.6.22329.4');
GO

COMMIT;
GO


